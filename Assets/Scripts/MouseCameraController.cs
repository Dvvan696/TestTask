using UnityEngine;

public class MouseCameraController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;        // Скорость перемещения камеры
    [SerializeField] float rotationSpeed = 50f;     // Скорость вращения камеры
    [SerializeField] float zoomSpeed = 5f;          // Скорость приближения/отдаления

    private Vector3 lastMousePosition;     // Позиция мыши в предыдущем кадре
    public GameObject focusedObject;      // Объект, на котором фокус

    private void Update()
    {
        // ---------------------------------------------------ПЕРЕМЕЩЕНИЕ КАМЕРЫ----------------------------------
        if (Input.GetMouseButton(1)) 
        {
            print("m1");
            Vector3 delta = Input.mousePosition - lastMousePosition;

            
            transform.Translate(-delta.x * moveSpeed * Time.deltaTime, 
                                -delta.y * moveSpeed * Time.deltaTime, 
                                0f, Space.World); 
        }
        lastMousePosition = Input.mousePosition; 

        // ВРАЩЕНИЕ КАМЕРЫ-------------------------------------------------------------------------------
        if (Input.GetMouseButton(2) && focusedObject != null) 
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            transform.RotateAround(focusedObject.transform.position, Vector3.up, mouseX * rotationSpeed * Time.deltaTime);

            Vector3 right = transform.TransformDirection(Vector3.right);
            
            transform.RotateAround(focusedObject.transform.position, right, -mouseY * rotationSpeed * Time.deltaTime);
        }

        // ПРИБЛИЖЕНИЕ/ОТДАЛЕНИЕ-------------------------------------------------------------------------------------
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        transform.Translate(transform.forward * scrollInput * zoomSpeed * Time.deltaTime, Space.World);
    }

    // Установка фокуса-----------------------------------------------------------------------------------------------------------
    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                focusedObject = hit.collider.gameObject;
            }
        }
    }
}