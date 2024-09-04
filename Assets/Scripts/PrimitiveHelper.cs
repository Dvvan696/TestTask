using UnityEngine;

public static class PrimitiveHelper
{
    public static Mesh GetPrimitiveMesh(PrimitiveType primitiveType)
    {
        GameObject tempObject = GameObject.CreatePrimitive(primitiveType);
        Mesh mesh = tempObject.GetComponent<MeshFilter>().sharedMesh;
        UnityEngine.Object.Destroy(tempObject);
        return mesh;
    }
}