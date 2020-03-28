using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Parameters")]
    public float speed;
    public float sensitivity;

    public float xRotation = 0F;
    public float yRotation = 0F;

    private void Start()
    {
        xRotation = transform.localRotation.eulerAngles.x;
        yRotation = transform.localRotation.eulerAngles.y;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Cursor.lockState = CursorLockMode.Locked;
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
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}