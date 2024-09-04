using System;
using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    public Renderer objectRenderer;
    public Color defaultColor;
    public float transparencySpeed = 5f;


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
        
        
        objectRenderer.material.color = new Color(1f, 1f, 0f, 1f);;

        if (Input.GetMouseButton(0))
        {
            
            MakeMaterialTransparent(objectRenderer.material);
        }
    }

    void OnMouseExit()
    {
        
        objectRenderer.material.color = defaultColor;
        
    }






    void MakeMaterialTransparent(Material material)
    {
        if (material == null)
        {
            Debug.LogError("Material is null!");
            return;
        }

        
        
    }
}