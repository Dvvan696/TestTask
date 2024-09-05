using UnityEngine;
using UnityEngine.UI;

public class UIListSpawner : MonoBehaviour
{
    [SerializeField] private GameObject uiPrefab;
    [SerializeField] private ScrollRect scrollRect; 
    [SerializeField] private RectTransform content; 
    [SerializeField] private ChildCounter childCounter;
    [SerializeField] private float spacing = 60f;

    void Start()
    {
        if (uiPrefab == null || scrollRect == null || content == null || childCounter == null)
        {
            Debug.LogError("Не все объекты назначены в Inspector!");
            return;
        }

        SpawnUIElements();
    }

    void SpawnUIElements()
    {
        int objectCount = childCounter.childObjects.Count;
        float elementHeight = uiPrefab.GetComponent<RectTransform>().rect.height;
        print(childCounter.childObjects.Count);
        float initialOffset = -spacing*2;

        for (int i = 0; i < objectCount; i++)
        {
            GameObject newElement = Instantiate(uiPrefab, content);
            if (newElement == null)
            {
                Debug.LogError("Failed to instantiate UI element.");
                continue;
            }

            ObjectListController listController = newElement.GetComponent<ObjectListController>();
            if (listController == null)
            {
                Debug.LogError("ObjectListController component not found on UI element.");
                Destroy(newElement);
                continue;
            }

            listController.targetObject = childCounter.childObjects[i];
            print(newElement);

            UIManager.Instantiate.UI_GO_List_fill(newElement);

            RectTransform elementRect = newElement.GetComponent<RectTransform>();
            if (elementRect == null)
            {
                Debug.LogError("RectTransform component not found on UI element: " + newElement.name);
                Destroy(newElement);
                continue;
            }

            float verticalOffset =  i * (elementHeight + spacing);
            elementRect.anchoredPosition = new Vector2(170f, verticalOffset);
        }

        UpdateContentSize();
    }

    void UpdateContentSize()
    {
        int objectCount = childCounter.childObjects.Count+1;
        if (objectCount == 0) return;

        RectTransform prefabRect = uiPrefab.GetComponent<RectTransform>();
        if (prefabRect == null)
        {
            Debug.LogError("RectTransform component not found on uiPrefab.");
            return;
        }

        float elementHeight = prefabRect.rect.height;

        float totalHeight = objectCount * (elementHeight + spacing) - spacing;

        Debug.Log($"Total content height: {totalHeight}, content container height: {content.rect.height}");

        content.sizeDelta = new Vector2(content.sizeDelta.x, totalHeight);

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 1;
    }
}