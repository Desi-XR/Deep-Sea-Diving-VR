using UnityEngine;

public class FreeFlyScript : MonoBehaviour
{
    public float moveSpeed = 15f;
    public float lookSpeed = 2f;

    private float pitch = 0f;
    private float yaw = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        Vector3 rot = transform.localRotation.eulerAngles;
        yaw = rot.y;
        pitch = rot.x;
    }

    void Update()
    {
        // 1. Handle Mouse Look
        yaw += lookSpeed * Input.GetAxis("Mouse X");
        pitch -= lookSpeed * Input.GetAxis("Mouse Y");
        pitch = Mathf.Clamp(pitch, -90f, 90f);

        transform.eulerAngles = new Vector3(pitch, yaw, 0f);

        // 2. Handle Inputs (Only WASD)
        float x = 0f;
        float z = 0f;

        if (Input.GetKey(KeyCode.W)) z += 1f;
        if (Input.GetKey(KeyCode.S)) z -= 1f;
        if (Input.GetKey(KeyCode.D)) x += 1f;
        if (Input.GetKey(KeyCode.A)) x -= 1f;

        // Create a basic local direction vector (No Y-axis manipulation)
        Vector3 moveDirection = new Vector3(x, 0f, z);

        // Normalize prevents moving faster when holding two keys (W + D)
        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }

        // 3. Move the camera
        // Because we don't specify "Space.World", Unity defaults to "Space.Self".
        // This guarantees W pushes you exactly where the lens is pointing.
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        // 4. Press ESC to unlock the mouse
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}