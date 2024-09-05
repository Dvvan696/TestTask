using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instantiate;


    [SerializeField] private MouseCameraController _cameraController;
    [SerializeField] private Slider _transparent_slider;
    [SerializeField] private FlexibleColorPicker fcp;
    [SerializeField] private Toggle toggleButton;
    [SerializeField] private Button _VisibleButton;
    [SerializeField] private Button _SaveButton;
    [SerializeField] private Button _LoadButton;
    [SerializeField] private Sprite spriteOn;
    [SerializeField] private Sprite spriteOff;


    private bool isMeshEnabled = true;
    private Coroutine colorChangeCoroutine;
    private List<GameObject> _UI_GO_List = new List<GameObject>();
    private Renderer _targetRenderer;


    private void Awake()
    {
        if (Instantiate == null)
        {
            Instantiate = this;
        }
        else if (Instantiate != this)
        {
            Destroy(gameObject);
        }

        if (_transparent_slider == null)
        {
            Debug.LogError("Transparency Slider not assigned!");
            return;
        }

        _transparent_slider.onValueChanged.AddListener(OnSliderValueChanged);

        _VisibleButton.image.sprite = isMeshEnabled ? spriteOn : spriteOff;
        _VisibleButton.onClick.AddListener(OnButtonClicked);
        _SaveButton.onClick.AddListener(SaveScene);
        _LoadButton.onClick.AddListener(LoadScene);

        toggleButton.onValueChanged.AddListener(OnToggleValueChanged);
    }

    private void Update()
    {
        if (_cameraController.focusedObject != _targetRenderer?.gameObject)
        {
            _targetRenderer = _cameraController.focusedObject?.GetComponent<Renderer>();

            if (_targetRenderer != null)
            {
                if (colorChangeCoroutine != null)
                {
                    StopCoroutine(colorChangeCoroutine);
                }

                colorChangeCoroutine = StartCoroutine(ColorChange());

                // Устанавливаем значение слайдера ПОСЛЕ запуска корутины
                _transparent_slider.value = _targetRenderer.material.color.a;
            }
            else
            {
                if (colorChangeCoroutine != null)
                {
                    StopCoroutine(colorChangeCoroutine);
                    colorChangeCoroutine = null;
                }
            }
        }
    }

    private void OnSliderValueChanged(float newValue)
    {
        if (_targetRenderer != null)
        {
            print("TransparentChange");
            Color newColor = _targetRenderer.material.color;
            newColor.a = newValue;
            _targetRenderer.material.color = newColor;
        }
    }

    private void OnButtonClicked()
    {
        isMeshEnabled = !isMeshEnabled;
        for (int i = 0; i < _UI_GO_List.Count; i++)
        {
            Renderer mesh = _UI_GO_List[i].GetComponent<ObjectListController>().targetObject.GetComponent<Renderer>();
            mesh.enabled = isMeshEnabled;
            _UI_GO_List[i].GetComponent<ObjectListController>().OuterButtonPress(isMeshEnabled);
        }

        _VisibleButton.image.sprite = isMeshEnabled ? spriteOn : spriteOff;
    }

    private void OnToggleValueChanged(bool isOn)
    {
        for (int i = 0; i < _UI_GO_List.Count; i++)
        {
            _UI_GO_List[i].GetComponent<ObjectListController>().targetObject.SetActive(isOn);
            _UI_GO_List[i].GetComponent<ObjectListController>().OuterTogglePress(isOn);
        }
    }

    public void UI_GO_List_fill(GameObject i)
    {
        if (!_UI_GO_List.Contains(i))
        {
            _UI_GO_List.Add(i);
        }
    }

    IEnumerator ColorChange()
    {
        while (true)
        {
            if (_targetRenderer != null)
            {
                //--------------------MAKE NEW TRANSPARENT MATERIAL---------------------------------------
                _targetRenderer.material.SetFloat("_Mode", 3f);
                _targetRenderer.material.renderQueue = 2501;


                _targetRenderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                _targetRenderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                _targetRenderer.material.SetInt("_ZWrite", 0);
                _targetRenderer.material.DisableKeyword("_ALPHATEST_ON");
                _targetRenderer.material.EnableKeyword("_ALPHABLEND_ON");
                _targetRenderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");


                _targetRenderer.material = new Material(_targetRenderer.material);
                //-----------------------------------------------------------

                Color newColor = _targetRenderer.material.color;
                newColor.r = fcp.color.r;
                newColor.g = fcp.color.g;
                newColor.b = fcp.color.b;
                _targetRenderer.material.color = newColor;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    //----------------------------------------------------------SceneSave--------------------------------------------------------------------------------------
    private void SaveScene()
    {
        SceneData sceneData = new SceneData();
        GameObject[] allInteractiveObjects = GameObject.FindGameObjectsWithTag("InteractiveObject");
        sceneData.objectsData = new ObjectData[allInteractiveObjects.Length];
        int validObjectCount = 0;

        for (int i = 0; i < allInteractiveObjects.Length; i++)
        {
            GameObject go = allInteractiveObjects[i];
            ObjectInteraction interaction = go.GetComponent<ObjectInteraction>();
            MeshFilter meshFilter = go.GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();

            // Проверяем наличие необходимых компонентов
            if (interaction == null)
            {
                Debug.LogWarning("ObjectInteraction component not found on object: " + go.name);
                continue;
            }

            if (meshFilter == null)
            {
                Debug.LogWarning("MeshFilter component not found on object: " + go.name);
                continue;
            }

            if (meshRenderer == null)
            {
                Debug.LogWarning("MeshRenderer component not found on object: " + go.name);
                continue;
            }

            bool isActive = go.activeSelf;
            bool isMeshEnabled = meshRenderer.enabled;

            // Тип примитива
            PrimitiveType primitiveType = PrimitiveType.Cube;
            if (meshFilter.sharedMesh != null)
            {
                Mesh currentMesh = meshFilter.sharedMesh;
                switch (currentMesh.name)
                {
                    case "Sphere":
                        primitiveType = PrimitiveType.Sphere;
                        break;
                    case "Capsule":
                        primitiveType = PrimitiveType.Capsule;
                        break;
                    case "Cylinder":
                        primitiveType = PrimitiveType.Cylinder;
                        break;
                    case "Quad":
                        primitiveType = PrimitiveType.Quad;
                        break;
                    case "Plane":
                        primitiveType = PrimitiveType.Plane;
                        break;
                    default:
                        Debug.LogWarning("Unknown primitive type: " + currentMesh.name);
                        break;
                }
            }

            Debug.Log("Saving object: " + go.name + ", type: " + primitiveType + ", isActive: " + isActive +
                      ", isMeshEnabled: " + isMeshEnabled);
            
                var material = interaction.ObjectRenderer.material;
                var position = go.transform.position;
                sceneData.objectsData[validObjectCount] = new ObjectData
            {
                name = go.name,
                colorR = material.color.r,
                colorG = material.color.g,
                colorB = material.color.b,
                colorA = material.color.a,
                posX = position.x,
                posY = position.y,
                posZ = position.z,
                parentName = go.transform.parent ? go.transform.parent.name : "",
                primitiveType = primitiveType,
                isActive = isActive,
                isMeshEnabled = isMeshEnabled
            };

            validObjectCount++;
        }

        // Урезаем массив до фактического количества сохраненных объектов
        Array.Resize(ref sceneData.objectsData, validObjectCount);

        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/sceneData.dat";
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, sceneData);
        stream.Close();

        Debug.Log("Scene saved to: " + path);
    }


    private void LoadScene()
    {
        string path = Application.persistentDataPath + "/sceneData.dat";
        Debug.Log("Loading scene from: " + path);
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SceneData sceneData = formatter.Deserialize(stream) as SceneData;
            stream.Close();

            foreach (ObjectData objData in sceneData.objectsData)
            {
                Debug.Log("Loading object: " + objData.name + ", type: " + objData.primitiveType + ", isActive: " +
                          objData.isActive + ", isMeshEnabled: " + objData.isMeshEnabled);

                GameObject newObject = GameObject.Find(objData.name);

                if (newObject == null)
                {
                    newObject = GameObject.CreatePrimitive(objData.primitiveType);
                    newObject.name = objData.name;
                    newObject.tag = "InteractiveObject";

                    ObjectInteraction interaction = newObject.GetComponent<ObjectInteraction>();
                    if (interaction == null)
                    {
                        interaction = newObject.AddComponent<ObjectInteraction>();
                    }

                    if (newObject.TryGetComponent<Renderer>(out Renderer renderer))
                    {
                        interaction.ObjectRenderer = renderer;
                    }
                }

                if (newObject.TryGetComponent<Renderer>(out Renderer objectRenderer))
                {
                    Color loadedColor = new Color(objData.colorR, objData.colorG, objData.colorB, objData.colorA);
                    objectRenderer.material.color = loadedColor;
                }

                newObject.transform.position = new Vector3(objData.posX, objData.posY, objData.posZ);

                if (!string.IsNullOrEmpty(objData.parentName))
                {
                    GameObject parentObject = GameObject.Find(objData.parentName);
                    if (parentObject != null)
                    {
                        newObject.transform.SetParent(parentObject.transform);
                    }
                    else
                    {
                        Debug.LogWarning("Parent object not found: " + objData.parentName);
                    }
                }

                newObject.SetActive(objData.isActive);

                if (newObject.TryGetComponent<MeshRenderer>(out MeshRenderer meshRenderer))
                {
                    meshRenderer.enabled = objData.isMeshEnabled;
                }
            }

            Debug.Log("Scene loaded from: " + path);
        }
        else
        {
            Debug.LogError("Save file not found in: " + path);
        }
    }
}