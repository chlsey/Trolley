using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{
    public float openAngle = 90f;
    public float openSpeed = 2f;

    private bool isOpen = false;
    private bool isMoving = false;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(
            transform.eulerAngles + new Vector3(0, -openAngle, 0)
        );
    }

    public void OpenDoor()
    {
        if (!isOpen && !isMoving)
            StartCoroutine(OpenCoroutine());
    }

    private IEnumerator OpenCoroutine()
    {
        isMoving = true;

        yield return new WaitForSeconds(3);

        float t = 0;
        while (t < 3)
        {
            t += Time.deltaTime * openSpeed;
            transform.rotation = Quaternion.Slerp(closedRotation, openRotation, t);
            yield return null;
        }

        isOpen = true;
        isMoving = false;
    }
}