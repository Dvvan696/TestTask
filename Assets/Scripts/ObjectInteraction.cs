using System;
using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    public Renderer objectRenderer;
    public Color defaultColor;
    public float transparencySpeed = 5f;

    internal bool isHidden = false;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        
    }

    private void OnMouseEnter()
    {
        defaultColor = objectRenderer.material.color;
        
    }

    void OnMouseOver()
    {
        
        
        objectRenderer.material.color = Color.yellow;

        if (Input.GetMouseButton(0))
        {
            float newAlpha = objectRenderer.material.color.a - Time.deltaTime * transparencySpeed;
            Color newColor = objectRenderer.material.color;
            newColor.a = Mathf.Clamp(newAlpha, 0f, 1f);
            objectRenderer.material.color = newColor;
        }
    }

    void OnMouseExit()
    {
        
        objectRenderer.material.color = defaultColor;
        
    }

    public void ToggleVisibility()
    {
        isHidden = !isHidden;
        objectRenderer.enabled = !isHidden;
    }

    public void SetColor(Color newColor)
    {
        objectRenderer.material.color = newColor;
    }

    public void SetTransparency(float value)
    {
        Color newColor = objectRenderer.material.color;
        newColor.a = value;
        objectRenderer.material.color = newColor;
    }
}