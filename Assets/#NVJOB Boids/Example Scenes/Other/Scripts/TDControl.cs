using UnityEngine;

[AddComponentMenu("#NVJOB/Tools/DeepSeaController_FixedPivot")]
public class TDControl : MonoBehaviour
{
    [Header("VR Movement Settings")]
    public float swimSpeed = 7f;
    public Transform camTransform; // Drag Camera 0 here

    [Header("Engine Toggle (Stop / Go)")]
    [Tooltip("Press this key on PC to stop/start the engine.")]
    public KeyCode pcToggleKey = KeyCode.Space;
    [Tooltip("Press this button in VR to stop/start. JoystickButton0 is usually the 'A' button on Meta Quest.")]
    public KeyCode vrToggleKey = KeyCode.JoystickButton0;
    [Tooltip("How smoothly the submarine brakes and accelerates. Higher = faster stop.")]
    public float acceleration = 3.0f;

    [Header("Gaze Steering (3-Second Rule)")]
    public float requiredGazeTime = 3.0f;
    public float steadyThreshold = 12f;
    public float turnSmoothness = 1.5f;

    [Header("Depth Limits (Sand Only)")]
    [Tooltip("Keeps you above the Y=-34 Sand")]
    public float minDepthY = -32f; 

    [Header("PC Testing Tools")]
    public bool enableMouseLook = true;
    public float mouseSensitivity = 2.0f;
    public Vector2 verticalClamp = new Vector2(-85, 85);

    private Vector3 currentMoveDirection;
    private Vector3 trackedGazeDirection;
    private float gazeTimer = 0f;
    
    // Engine State Variables
    private bool isEngineOn = true;
    private float currentSpeed;

    // PC Testing Variables
    private float yaw, pitch;

    void Start()
    {
        // Start at full speed
        currentSpeed = swimSpeed;

        if (camTransform != null)
        {
            currentMoveDirection = camTransform.forward;
            trackedGazeDirection = camTransform.forward;
        }
        else
        {
            currentMoveDirection = transform.forward;
            trackedGazeDirection = transform.forward;
        }

        if (enableMouseLook)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            Vector3 rot = transform.eulerAngles;
            yaw = rot.y;
            if (camTransform != null) pitch = camTransform.localEulerAngles.x;
        }
    }

    void Update()
    {
        if (camTransform == null) return;

        // --- 1. ENGINE TOGGLE LOGIC ---
        // Listen for the PC Spacebar OR the VR 'A' Button OR the default VR Trigger ("Fire1")
        if (Input.GetKeyDown(pcToggleKey) || Input.GetKeyDown(vrToggleKey) || Input.GetButtonDown("Fire1"))
        {
            isEngineOn = !isEngineOn; // Flip the switch!
        }

        // Smoothly calculate the target speed (either full speed or zero)
        float targetSpeed = isEngineOn ? swimSpeed : 0f;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * acceleration);


        // --- 2. PC MOUSE TESTING OVERRIDE ---
        if (enableMouseLook)
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, verticalClamp.x, verticalClamp.y);

            transform.rotation = Quaternion.Euler(0, yaw, 0);
            camTransform.localRotation = Quaternion.Euler(pitch, 0, 0);
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        // --- 3. THE VR GAZE LOGIC ---
        float angleChange = Vector3.Angle(camTransform.forward, trackedGazeDirection);

        if (angleChange < steadyThreshold)
        {
            gazeTimer += Time.deltaTime;

            if (gazeTimer >= requiredGazeTime)
            {
                currentMoveDirection = Vector3.Slerp(currentMoveDirection, trackedGazeDirection, Time.deltaTime * turnSmoothness);
            }
        }
        else
        {
            trackedGazeDirection = camTransform.forward;
            gazeTimer = 0f;
        }

        // --- 4. APPLY MOVEMENT ---
        // Now we use 'currentSpeed' instead of 'swimSpeed' so the brakes work!
        transform.position += currentMoveDirection.normalized * currentSpeed * Time.deltaTime;

        // --- 5. APPLY DEPTH LIMIT (THE FLOOR CLAMP) ---
        Vector3 clampedPos = transform.position;
        if (clampedPos.y < minDepthY) 
        {
            clampedPos.y = minDepthY;
        }
        transform.position = clampedPos;
    }
}