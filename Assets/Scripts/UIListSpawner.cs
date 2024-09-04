using UnityEngine;
using UnityEngine.UI;

public class UIListSpawner : MonoBehaviour
{
    public GameObject uiPrefab; 
    public RectTransform content; 
    public ChildCounter childCounter; 
    public float spacing = 60f; 

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
        
        int objectCount = childCounter.childObjects.Count;
        
        print(childCounter.childObjects.Count);
        
        for (int i = 0; i < objectCount; i++)
        {
            
            // Создаем новый UI элемент из префаба и указываем родителя
            GameObject newElement = Instantiate(uiPrefab, content);
            newElement.GetComponent<ObjectListController>().targetObject = childCounter.childObjects[i];
            print(newElement);
            UIManager.Instantiate.UI_GO_List_fill(newElement);
            // Получаем RectTransform нового UI элемента
            RectTransform elementRect = newElement.GetComponent<RectTransform>();

            // Рассчитываем вертикальный сдвиг
            float verticalOffset = i * spacing;

            // Устанавливаем позицию UI элемента
            elementRect.anchoredPosition = new Vector2(0f, -verticalOffset);
            
            
        }
    }
}