using UnityEngine;
using UnityEngine.EventSystems;

public class LookAtOrbitalRotation : MonoBehaviour
{
    private Camera mainCamera;
    public Transform target; // Assign the target in Inspector
    public float radius = 5f;
    private Vector3 targetPosition;

    // Orbital rotation angles
    private float orbitX = 30f;
    private float orbitY = 45f;

    // Input sensitivity
    public float mouseSensitivity = 2f;
    public float touchSensitivity = 1f;

    // Touch tracking
    private Vector2 lastTouchPosition = Vector2.zero;
    private bool isTouching = false;

    void Start()
    {
        mainCamera = Camera.main;

        // If target not assigned, find the QuantumCube
        if (target == null)
        {
            target = transform; // Use the cube this script is attached to
        }

        targetPosition = target.position;
        UpdateCameraPosition();
    }

    void Update()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        // Handle keyboard input (Horizontal and Vertical axis)
        HandleKeyboardInput();

        // Handle mouse input
        HandleMouseInput();

        // Handle touch input
        HandleTouchInput();

        // Update camera position based on orbital angles
        UpdateCameraPosition();
    }

    void HandleKeyboardInput()
    {
        // Get input from Horizontal (A/D or Arrow Keys) and Vertical (W/S or Arrow Keys)
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Rotate orbit angles based on keyboard input
        orbitY += horizontalInput * mouseSensitivity;
        orbitX -= verticalInput * mouseSensitivity;

        // Clamp vertical rotation to prevent flipping
        orbitX = Mathf.Clamp(orbitX, -80f, 80f);
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButton(0)) // Left mouse button held
        {
            float deltaX = Input.GetAxis("Mouse X");
            float deltaY = Input.GetAxis("Mouse Y");

            orbitY += deltaX * mouseSensitivity;
            orbitX -= deltaY * mouseSensitivity;

            // Clamp vertical rotation
            orbitX = Mathf.Clamp(orbitX, -80f, 80f);
        }
    }

    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                lastTouchPosition = touch.position;
                isTouching = true;
            }
            else if (touch.phase == TouchPhase.Moved && isTouching)
            {
                Vector2 delta = touch.position - lastTouchPosition;

                orbitY += delta.x * touchSensitivity * 0.1f;
                orbitX -= delta.y * touchSensitivity * 0.1f;

                orbitX = Mathf.Clamp(orbitX, -80f, 80f);

                lastTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isTouching = false;
            }
        }
    }

    void UpdateCameraPosition()
    {
        // Convert orbital angles to Euler angles
        Quaternion rotation = Quaternion.Euler(orbitX, orbitY, 0f);

        // Calculate camera position orbiting around the target
        Vector3 cameraPos = rotation * new Vector3(0f, 0f, -radius);
        mainCamera.transform.position = targetPosition + cameraPos;

        // Make camera look at the target
        mainCamera.transform.LookAt(targetPosition);
    }
}