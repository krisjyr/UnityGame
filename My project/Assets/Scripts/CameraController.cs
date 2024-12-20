using UnityEngine;

public class PlaneCameraController : MonoBehaviour
{
    public Transform target; // The plane object the camera should follow
    public float followSpeed = 5f; // The speed at which the camera follows the target
        public float fovChangeSpeed = 5f; // The speed at which the camera changes its field of view
    public float rotationSpeed = 2f; // The speed at which the camera rotates
    public float cameraDistance = 10f; // The distance from the camera to the target
    public float cameraHeight = 5f; // The height of the camera above the target
    public float cameraAngle = 30f; // The angle of the camera relative to the plane
    public float cameraFov = 60f; // The field of view of the camera
    public float cameraMaxFov = 80f; // The maximum field of view of the camera
    private Vector3 cameraOffset;
    private float targetFov;

    void Start()
    {
        // Calculate the initial camera offset
        cameraOffset = Quaternion.AngleAxis(cameraAngle, Vector3.right) * (-Vector3.forward * cameraDistance);
        cameraOffset.y = cameraHeight;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // Calculate the desired camera position
            Vector3 desiredPosition = target.position + (target.rotation * cameraOffset);

            // Smoothly move the camera to the desired position
            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

            // Rotate the camera to follow the plane's orientation
            Quaternion targetRotation = Quaternion.LookRotation(target.forward, target.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            targetFov = Mathf.Lerp(targetFov, cameraMaxFov, fovChangeSpeed * Time.deltaTime);
            Camera.main.fieldOfView = targetFov; // Update the camera's FOV
        }
    }
}