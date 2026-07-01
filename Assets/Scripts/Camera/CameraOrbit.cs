using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target; // Trascina qui il centro del tavolo (o il DeckSlot)
    public float sensitivity = 2f;

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            float rotX = Input.GetAxis("Mouse X") * sensitivity;
            float rotY = -Input.GetAxis("Mouse Y") * sensitivity;

            transform.RotateAround(target.position, Vector3.up, rotX);
            transform.RotateAround(target.position, transform.right, rotY);
        }
    }
}