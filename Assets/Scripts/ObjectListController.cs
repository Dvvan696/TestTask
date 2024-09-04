using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectListController : MonoBehaviour
{
    private GameObject _targetObject;
    private bool isMeshEnabled = true;
    
    [SerializeField] private Button Button;
    [SerializeField] private Sprite spriteOn;
    [SerializeField] private Sprite spriteOff;
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private MeshRenderer targetMeshRenderer;
    [SerializeField] private Toggle toggleButton;


    private void Start()
    {
        name.text = _targetObject.name;

        if (targetObject == null || Button == null || spriteOn == null || spriteOff == null)
        {
            Debug.LogError("Не все объекты назначены в Inspector!");
            return;
        }


        targetMeshRenderer = _targetObject.GetComponent<MeshRenderer>();


        if (targetMeshRenderer == null)
        {
            Debug.LogError("На целевом объекте не найден компонент MeshRenderer!");
            return;
        }


        Button.image.sprite = isMeshEnabled ? spriteOn : spriteOff;

        
        Button.onClick.AddListener(OnButtonClicked);
        if (_targetObject == null || toggleButton == null)
        {
            Debug.LogError("TargetObject or Button not assigned!");
            return;
        }

        
        toggleButton.isOn = _targetObject.activeSelf;

        
        toggleButton.onValueChanged.AddListener(OnToggleValueChanged);
    }

    public void OuterButtonPress(bool turn)
    {
        isMeshEnabled = !turn;
        OnButtonClicked();
    }

    public void OuterTogglePress(bool turn)
    {
        toggleButton.isOn = turn;
        OnToggleValueChanged(turn);
    }

    private void OnButtonClicked()
    {
        isMeshEnabled = !isMeshEnabled;
        
        targetMeshRenderer.enabled = isMeshEnabled;
        
        Button.image.sprite = isMeshEnabled ? spriteOn : spriteOff;
    }

    private void OnToggleValueChanged(bool isOn)
    {
        _targetObject.SetActive(isOn);
    }

    public GameObject targetObject
    {
        get { return _targetObject; }
        set { _targetObject = value; }
    }
}