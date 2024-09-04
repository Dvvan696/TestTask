using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectListController : MonoBehaviour
{
    private GameObject _targetObject;
    public Button Button; 
    public Sprite spriteOn; 
    public Sprite spriteOff; 
    [SerializeField]private TextMeshProUGUI name;
    private MeshRenderer targetMeshRenderer; 
    private bool isMeshEnabled = true; 
    public Toggle toggleButton;
    

    
    void Start()
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

        // Добавляем слушатель события нажатия на кнопку
        Button.onClick.AddListener(OnButtonClicked);
        if (_targetObject == null || toggleButton == null)
        {
            Debug.LogError("TargetObject or Button not assigned!");
            return;
        }

        // Синхронизируем начальное состояние кнопки Toggle с видимостью объекта
        toggleButton.isOn = _targetObject.activeSelf;

        // Добавляем слушатель события изменения значения Toggle
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

    // Метод, вызываемый при нажатии на кнопку
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
        get
        {
            return _targetObject;
        }
        set
        {
            _targetObject = value;
        }
    }
    
}