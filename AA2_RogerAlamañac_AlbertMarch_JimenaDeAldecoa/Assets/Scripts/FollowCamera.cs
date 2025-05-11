using UnityEngine;

public class FollowCamera: MonoBehaviour
{
    public Transform target; 
    public Vector3 offset = new Vector3(0, 3, -6);
    public float smoothSpeed = 0.125f;
    public float rotationSpeed = 5f;

    private float currentYaw = 0f;

    void LateUpdate()
    {
        if (target == null) return;

    
        if (Input.GetMouseButton(1)) // Click dret del ratolí
        {
            float mouseX = Input.GetAxis("Mouse X");
            currentYaw += mouseX * rotationSpeed;
        }
 
        Quaternion rotation = Quaternion.Euler(0, currentYaw, 0);
        Vector3 desiredPosition = target.position + rotation * offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothedPosition;
        transform.LookAt(target);
    }
}
