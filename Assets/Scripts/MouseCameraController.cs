using UnityEngine;

public class MouseCameraController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;        // Скорость перемещения камеры
    [SerializeField] float rotationSpeed = 50f;     // Скорость вращения камеры
    [SerializeField] float zoomSpeed = 5f;          // Скорость приближения/отдаления

    private Vector3 lastMousePosition;     // Позиция мыши в предыдущем кадре
     public GameObject focusedObject;      // Объект, на котором фокус

    void Update()
    {
        // ПЕРЕМЕЩЕНИЕ КАМЕРЫ
        if (Input.GetMouseButton(1)) 
        {
            print("m1");
            Vector3 delta = Input.mousePosition - lastMousePosition;

            // Перемещаем камеру в глобальных координатах
            transform.Translate(-delta.x * moveSpeed * Time.deltaTime, 
                                -delta.y * moveSpeed * Time.deltaTime, 
                                0f, Space.World); 
        }
        lastMousePosition = Input.mousePosition; 

        // ВРАЩЕНИЕ КАМЕРЫ (без изменений)
        if (Input.GetMouseButton(2) && focusedObject != null) 
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // Поворачиваем камеру вокруг объекта по горизонтали
            transform.RotateAround(focusedObject.transform.position, Vector3.up, mouseX * rotationSpeed * Time.deltaTime);

            // Вычисляем направление "вправо" относительно камеры
            Vector3 right = transform.TransformDirection(Vector3.right);

            // Поворачиваем камеру вокруг оси "вправо" на угол, зависящий от движения мыши по вертикали
            transform.RotateAround(focusedObject.transform.position, right, -mouseY * rotationSpeed * Time.deltaTime);
        }

        // ПРИБЛИЖЕНИЕ/ОТДАЛЕНИЕ (без изменений)
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        transform.Translate(transform.forward * scrollInput * zoomSpeed * Time.deltaTime, Space.World);
    }

    // Установка фокуса (без изменений)
    void LateUpdate()
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