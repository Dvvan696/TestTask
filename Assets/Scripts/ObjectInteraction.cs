using System;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    
    
    private Renderer _objectRenderer;
    private Color defaultColor;


    private void Start()
    {
        _objectRenderer = GetComponent<Renderer>();
        if (!gameObject.CompareTag("InteractiveObject"))
        {
            Debug.LogWarning("ObjectInteraction Tag not found on object: " + this.name);
        }
    }

    private void OnMouseEnter()
    {
        defaultColor = _objectRenderer.material.color;
    }

    private void OnMouseOver()
    {
        _objectRenderer.material.color = new Color(1f, 1f, 0f, 1f);
        ;
        if (Input.GetMouseButton(0))
        {
            MakeMaterialTransparent(_objectRenderer.material);
        }
    }

    private void OnMouseExit()
    {
        _objectRenderer.material.color = defaultColor;
    }

    void MakeMaterialTransparent(Material material)
    {
        if (material == null)
        {
            Debug.LogError("Material is null!");
            return;
        }
    }

    public Renderer ObjectRenderer
    {
        get { return _objectRenderer;}
        set { _objectRenderer = value; }
    }
}