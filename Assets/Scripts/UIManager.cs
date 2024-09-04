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
    [SerializeField] private MouseCameraController _cameraController;
    [SerializeField] private Slider _transparent_slider;
    private Renderer _targetRenderer;
    [SerializeField] private FlexibleColorPicker fcp;

    private List<GameObject> _UI_GO_List = new List<GameObject>();

    public static UIManager Instantiate;

    private Coroutine colorChangeCoroutine;
    public Button Button;
    public Button SaveButton;
    public Button LoadButton;


    [SerializeField] private Sprite spriteOn;
    [SerializeField] private Sprite spriteOff;
    private bool isMeshEnabled = true;

    public Toggle toggleButton;


    private void Start()
    {
        if (Instantiate == null)
        {
            Instantiate = this;
        }
        else
        {
            Destroy(Instantiate);
        }

        if (_transparent_slider == null)
        {
            Debug.LogError("Transparency Slider not assigned!");
            return;
        }

        _transparent_slider.onValueChanged.AddListener(OnSliderValueChanged);

        Button.image.sprite = isMeshEnabled ? spriteOn : spriteOff;
        Button.onClick.AddListener(OnButtonClicked);
        SaveButton.onClick.AddListener(SaveScene);
        LoadButton.onClick.AddListener(LoadScene);

        // Добавляем слушатель события изменения значения Toggle
        toggleButton.onValueChanged.AddListener(OnToggleValueChanged);


    }

    void Update()
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

        Button.image.sprite = isMeshEnabled ? spriteOn : spriteOff;
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

    //-------------------------------------SceneSave--------------------------------------------------------------------
public void SaveScene()
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

        // Проверяем активность GameObject и MeshRenderer
        bool isActive = go.activeSelf;
        bool isMeshEnabled = meshFilter != null && meshFilter.sharedMesh != null && go.GetComponent<MeshRenderer>().enabled;

        if (interaction != null && isActive && isMeshEnabled) // Сохраняем только активные объекты с включенным мешем
        {
            // Определяем тип примитива
            PrimitiveType primitiveType = PrimitiveType.Cube;
            if (meshFilter != null && meshFilter.sharedMesh != null)
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

            // Выводим в консоль информацию о сохраняемом объекте и его типе
            Debug.Log("Saving object: " + go.name + ", type: " + primitiveType + ", isActive: " + isActive + ", isMeshEnabled: " + isMeshEnabled);

            sceneData.objectsData[validObjectCount] = new ObjectData
            {
                name = go.name,
                colorR = interaction.objectRenderer.material.color.r,
                colorG = interaction.objectRenderer.material.color.g,
                colorB = interaction.objectRenderer.material.color.b,
                colorA = interaction.objectRenderer.material.color.a,
                posX = go.transform.position.x,
                posY = go.transform.position.y,
                posZ = go.transform.position.z,
                parentName = go.transform.parent ? go.transform.parent.name : "",
                primitiveType = primitiveType,
                isActive = go.activeSelf,
                isMeshEnabled = meshFilter != null && meshFilter.sharedMesh != null && go.GetComponent<MeshRenderer>().enabled
            };

            validObjectCount++;
        }
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


        public void LoadScene()
{
    string path = Application.persistentDataPath + "/sceneData.dat";
    Debug.Log("Loading scene from: " + path);
    if (File.Exists(path))
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Open);

        SceneData sceneData = formatter.Deserialize(stream) as SceneData;
        stream.Close();

        // Обновляем или создаем объекты
        foreach (ObjectData objData in sceneData.objectsData)
        {
            Debug.Log("Loading object: " + objData.name + ", type: " + objData.primitiveType + ", isActive: " + objData.isActive + ", isMeshEnabled: " + objData.isMeshEnabled);

            // Ищем объект с таким именем
            GameObject newObject = GameObject.Find(objData.name);

            // Если объект не найден, создаем новый
            if (newObject == null)
            {
                newObject = GameObject.CreatePrimitive(objData.primitiveType);
                newObject.name = objData.name;
                newObject.tag = "InteractiveObject";

                // Добавляем ObjectInteraction, если его нет
                ObjectInteraction interaction = newObject.GetComponent<ObjectInteraction>();
                if (interaction == null)
                {
                    interaction = newObject.AddComponent<ObjectInteraction>();
                }

                // Инициализируем objectRenderer
                if (newObject.TryGetComponent<Renderer>(out Renderer renderer))
                {
                    interaction.objectRenderer = renderer;
                }
            }

            // Настраиваем цвет 
            if (newObject.TryGetComponent<Renderer>(out Renderer objectRenderer))
            {
                Color loadedColor = new Color(objData.colorR, objData.colorG, objData.colorB, objData.colorA);
                objectRenderer.material.color = loadedColor;
            }

            // Установка позиции
            newObject.transform.position = new Vector3(objData.posX, objData.posY, objData.posZ);

            // Устанавливаем родителя, если он был сохранен
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

            // Устанавливаем активность GameObject
            newObject.SetActive(objData.isActive);

            // Устанавливаем активность MeshRenderer
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


