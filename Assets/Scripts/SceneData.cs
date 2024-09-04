using System;
using UnityEngine;

[Serializable]
public class SceneData
{
    public ObjectData[] objectsData;
}

[Serializable]
public class ObjectData
{
    public string name;
    public float colorR;
    public float colorG;
    public float colorB;
    public float colorA;
    public float transparency; 
    public bool isVisible;
    public float posX;
    public float posY;
    public float posZ;
    public string parentName; // Имя родительского объекта
    public PrimitiveType primitiveType;
    public bool isActive;
    public bool isMeshEnabled;
}
