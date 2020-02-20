using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Parameters")]
    public float speed;
    public float sensitivity;

    private float xRotation = 0F;
    private float yRotation = 0F;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
        float vertical = Input.GetAxis("Vertical") * Time.deltaTime * speed;
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;

        xRotation -= mouseY;
        yRotation += mouseX;
        xRotation = Mathf.Clamp(xRotation, 0F, 90F);

        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0F);
        transform.position += transform.right * horizontal + transform.forward * vertical;
    }
}