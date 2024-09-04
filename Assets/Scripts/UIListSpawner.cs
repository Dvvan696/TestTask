using UnityEngine;
using UnityEngine.UI;

public class UIListSpawner : MonoBehaviour
{
    public GameObject uiPrefab; // Префаб UI элемента
    public RectTransform content; // RectTransform компонента Content у VerticalLayoutGroup
    public ChildCounter childCounter; // Ссылка на ваш скрипт, возвращающий список GO
    public float spacing = 60f; // Отступ между UI элементами по вертикали

    void Start()
    {
        // Проверяем, что все объекты назначены в Inspector
        if (uiPrefab == null || content == null || childCounter == null)
        {
            Debug.LogError("Не все объекты назначены в Inspector!");
            return;
        }

        SpawnUIElements();
    }

    void SpawnUIElements()
    {
        // Получаем количество объектов в списке
        int objectCount = childCounter.childObjects.Count;
        print(childCounter.childObjects.Count);
        // Создаем UI элементы
        for (int i = 0; i < objectCount; i++)
        {
            
            // Создаем новый UI элемент из префаба и указываем родителя
            GameObject newElement = Instantiate(uiPrefab, content); 
            
           
            // Получаем RectTransform нового UI элемента
            RectTransform elementRect = newElement.GetComponent<RectTransform>();

            // Рассчитываем вертикальный сдвиг
            float verticalOffset = i * spacing;

            // Устанавливаем позицию UI элемента
            elementRect.anchoredPosition = new Vector2(0f, -verticalOffset);
        }
    }
}