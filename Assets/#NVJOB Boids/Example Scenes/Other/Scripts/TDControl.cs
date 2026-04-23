using UnityEngine;


[AddComponentMenu("#NVJOB/Tools/DeepSeaController_FixedPivot")]
public class TDControl : MonoBehaviour
{
    [Header("Vision Settings")]
    public float sensitivity = 2.0f;
    public Vector2 verticalClamp = new Vector2(-85, 85);
    public float smoothRotation = 10f;


    [Header("Movement Settings")]
    public float swimSpeed = 7f;


    [Header("Camera & Zoom")]
    public Transform camTransform;
    public Vector2 zoomLimit = new Vector2(-15, 0); // Keep near 0 to avoid revolving
    public float smoothZoom = 5f;


    private Transform tr;
    private float yaw, pitch;
    private float targetZoom, currentZoom, zoomVel;


    void Awake()
    {
        tr = transform;


        // Initialize angles
        Vector3 rot = tr.eulerAngles;
        yaw = rot.y;


        if (camTransform != null)
        {
            pitch = camTransform.localEulerAngles.x;
            // CRITICAL: Set camera local position to 0 to stop revolving
            camTransform.localPosition = Vector3.zero;
        }


        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    void LateUpdate()
    {
        // 1. SPLIT ROTATION (Prevents Revolving)
        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;
        pitch = Mathf.Clamp(pitch, verticalClamp.x, verticalClamp.y);


        // Rotate Rig horizontally (Yaw)
        tr.rotation = Quaternion.Slerp(tr.rotation, Quaternion.Euler(0, yaw, 0), Time.deltaTime * smoothRotation);


        // Rotate Camera vertically (Pitch)
        if (camTransform != null)
        {
            camTransform.localRotation = Quaternion.Slerp(camTransform.localRotation, Quaternion.Euler(pitch, 0, 0), Time.deltaTime * smoothRotation);
        }


        // 2. 3D MOVEMENT (Moves where you LOOK)
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");


        // We use camTransform.forward so if you look down and press W, you go DOWN.
        Vector3 moveDir = (camTransform.forward * v) + (tr.right * h);
        tr.position += moveDir * swimSpeed * Time.deltaTime;


        // 3. ZOOM (Strictly Z-axis offset)
        HandleZoom();


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }


    void HandleZoom()
    {
        if (camTransform == null) return;


        targetZoom += Input.mouseScrollDelta.y;
        targetZoom = Mathf.Clamp(targetZoom, zoomLimit.x, zoomLimit.y);
        currentZoom = Mathf.SmoothDamp(currentZoom, targetZoom, ref zoomVel, 1f / smoothZoom);


        // Offset the camera slightly back for zoom, but keep X and Y at 0
        camTransform.localPosition = new Vector3(0, 0, currentZoom);
    }
}



