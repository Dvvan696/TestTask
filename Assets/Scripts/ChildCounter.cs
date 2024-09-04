using System.Collections.Generic;
using UnityEngine;

public class ChildCounter : MonoBehaviour
{
    public List<GameObject> childObjects = new List<GameObject>(); // Список для хранения дочерних объектов

    private void Awake()
    {
        RefreshChildList();
    }

    // Метод для обновления списка дочерних объектов
    public void RefreshChildList()
    {
        childObjects.Clear(); // Очищаем список перед обновлением

        foreach (Transform child in transform)
        {
            childObjects.Add(child.gameObject);
        }

        Debug.Log("Список дочерних объектов обновлен. Количество: " + childObjects.Count);
    }
}