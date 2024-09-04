using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectListController : MonoBehaviour
{
    private GameObject _targetObject; // Объект с MeshRenderer, который нужно включать/выключать
    public Button Button; // Кнопка для переключения
    public Sprite spriteOn; // Спрайт для включенного состояния
    public Sprite spriteOff; // Спрайт для выключенного состояния
    [SerializeField]private TextMeshProUGUI name;
    private MeshRenderer targetMeshRenderer; // Компонент MeshRenderer объекта
    private bool isMeshEnabled = true; // Флаг состояния MeshRenderer
    public Toggle toggleButton;
    

    
    void Start()
    {
        
        name.text = _targetObject.name;
        // Проверяем, что объекты назначены в Inspector
        if (targetObject == null || Button == null || spriteOn == null || spriteOff == null)
        {
            Debug.LogError("Не все объекты назначены в Inspector!");
            return;
        }

        // Получаем компонент MeshRenderer целевого объекта
        targetMeshRenderer = _targetObject.GetComponent<MeshRenderer>();

        // Проверяем, найден ли компонент MeshRenderer
        if (targetMeshRenderer == null)
        {
            Debug.LogError("На целевом объекте не найден компонент MeshRenderer!");
            return;
        }

        // Устанавливаем начальный спрайт кнопки
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

    // Метод, вызываемый при нажатии на кнопку
    private void OnButtonClicked()
    {
        // Инвертируем состояние MeshRenderer
        isMeshEnabled = !isMeshEnabled;

        // Включаем/выключаем MeshRenderer
        targetMeshRenderer.enabled = isMeshEnabled;

        // Меняем спрайт кнопки
        Button.image.sprite = isMeshEnabled ? spriteOn : spriteOff;
    }
    
    private void OnToggleValueChanged(bool isOn)
    {
        // Включаем или выключаем объект в зависимости от состояния Toggle
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