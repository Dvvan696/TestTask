using UnityEngine;
using UnityEngine.UI;

public class UIListSpawner : MonoBehaviour
{
    [SerializeField] private GameObject uiPrefab;
    [SerializeField] private RectTransform content;
    [SerializeField] private ChildCounter childCounter;
    [SerializeField] private float spacing = 60f;

    void Start()
    {
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
            GameObject newElement = Instantiate(uiPrefab, content);
            newElement.GetComponent<ObjectListController>().targetObject = childCounter.childObjects[i];
            print(newElement);
            UIManager.Instantiate.UI_GO_List_fill(newElement);
            RectTransform elementRect = newElement.GetComponent<RectTransform>();

            float verticalOffset = i * spacing;

            elementRect.anchoredPosition = new Vector2(0f, -verticalOffset);
        }
    }
}