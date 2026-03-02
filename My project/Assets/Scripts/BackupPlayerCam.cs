using UnityEngine;
public class BackupPlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;
    public Transform orientation;
    public float xRotation;
    public float yRotation;
    public bool clampView = false; 
    public float minX = -90f; 
    public float maxX = 90f; 
    public float minY = -360f; 
    public float maxY = 360f;
    public float tiltZ = 0f;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;
        yRotation += mouseX;
        xRotation -= mouseY;
        if (clampView) 
        { 
            xRotation = Mathf.Clamp(xRotation, minX, maxX); 
            yRotation = Mathf.Clamp(yRotation, minY, maxY); 
        } 
        else 
        { 
            xRotation = Mathf.Clamp(xRotation, -90f, 90f); 
        }
        transform.rotation = Quaternion.Euler(xRotation, yRotation, tiltZ);

        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
    public void SetRotation(float x, float y)
    {
        xRotation = x;
        yRotation = y;
    }

}