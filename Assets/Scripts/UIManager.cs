using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private MouseCameraController _cameraController;
    [SerializeField] private Slider _transparent_slider;
    private Renderer _targetRenderer;
    [SerializeField] private FlexibleColorPicker fcp;

    private Coroutine colorChangeCoroutine;

    private void Start()
    {
        if (_transparent_slider == null)
        {
            Debug.LogError("Transparency Slider not assigned!");
            return;
        }
        _transparent_slider.onValueChanged.AddListener(OnSliderValueChanged);
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
            print("sedgdafhsdtfrjhdfgedsryhe5rt");
            Color newColor = _targetRenderer.material.color;
            newColor.a = newValue;
            _targetRenderer.material.color = newColor;
        }
    }

    IEnumerator ColorChange()
    {
        while (true)
        {
            if (_targetRenderer != null)
            {
                Color newColor = _targetRenderer.material.color;
                newColor.r = fcp.color.r;
                newColor.g = fcp.color.g;
                newColor.b = fcp.color.b;
                _targetRenderer.material.color = newColor;
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
