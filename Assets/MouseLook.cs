using UnityEngine;
using UnityEngine.UI; // Required for UI components

public class MouseLook : MonoBehaviour
{
    float rotationX = 0f;
    float rotationY = 0f;

    public Vector2 sensitivity = Vector2.one * 360f;

    public Camera playerCamera; // Assign the main camera to this in the inspector
    public Color highlightColor = Color.yellow; // Color to use for highlighting
    private Color[] originalColors;
    private Material[] materials;
    private Transform highlightedObject;

    private GameObject currentPanel; // Reference to the current UI panel
    private bool isPanelOpen = false; // Tracks whether the panel is open

    // Arrays to map objects to their panels
    public GameObject[] interactableObjects; // Assign interactable objects in the inspector
    public GameObject[] uiPanels; // Assign corresponding UI panels in the inspector

    public GameObject crosshair; // Reference to the crosshair UI element

    // Zoom variables
    public float zoomSpeed = 2f;
    public float minFOV = 15f;
    public float maxFOV = 90f;

    // Movement variables
    public float moveSpeed = 5f;
    private Rigidbody rb;

    // Height variables
    public float fixedHeight = 1.0f; // Fixed height at which the camera should stay

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // Ensure all panels are initially hidden
        foreach (var panel in uiPanels)
        {
            panel.SetActive(false);
        }

        // Make sure the crosshair is visible initially
        if (crosshair != null)
        {
            crosshair.SetActive(true);
        }

        // Initialize the Rigidbody component
        rb = GetComponent<Rigidbody>();

        // Lock rotation and Y position to prevent the player from tipping over and falling
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
    }

    void Update()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        // Mouse look
        rotationY += Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity.x;
        rotationX += Input.GetAxis("Mouse Y") * Time.deltaTime * -1 * sensitivity.y;
        transform.localEulerAngles = new Vector3(rotationX, rotationY, 0);

        // Zoom functionality
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0f)
        {
            float newFOV = playerCamera.fieldOfView - scrollInput * zoomSpeed;
            playerCamera.fieldOfView = Mathf.Clamp(newFOV, minFOV, maxFOV);
        }

        // Move player using WASD keys
        float moveHorizontal = Input.GetAxis("Horizontal"); // A/D keys
        float moveVertical = Input.GetAxis("Vertical"); // W/S keys

        Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical).normalized * moveSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + transform.TransformDirection(movement));

        // Raycast to detect objects
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // If the object has a collider and is interactable
            if (hit.transform.CompareTag("Interactable"))
            {
                // If it's a new object, update the highlight
                if (highlightedObject != hit.transform)
                {
                    if (highlightedObject != null)
                    {
                        ResetHighlight();
                    }

                    highlightedObject = hit.transform;
                    Renderer renderer = highlightedObject.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        materials = renderer.materials; // Get all materials
                        originalColors = new Color[materials.Length];

                        for (int i = 0; i < materials.Length; i++)
                        {
                            originalColors[i] = materials[i].color; // Store original color
                            materials[i].color = highlightColor; // Set highlight color
                        }
                    }
                }

                // Handle clicking the object to toggle the UI panel
                if (Input.GetMouseButtonDown(0)) // Left mouse button click
                {
                    TogglePanel(hit.transform);
                }
            }
        }
        else
        {
            // Reset highlight if nothing is hit
            ResetHighlight();
        }
    }

    void ResetHighlight()
    {
        if (highlightedObject != null && materials != null)
        {
            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i] != null)
                {
                    materials[i].color = originalColors[i]; // Reset to original color
                }
            }
            highlightedObject = null;
        }

        // Close any open panel when no object is highlighted
        if (isPanelOpen)
        {
            ClosePanel();
        }
    }

    void TogglePanel(Transform obj)
    {
        int index = System.Array.IndexOf(interactableObjects, obj.gameObject);
        if (index >= 0 && index < uiPanels.Length)
        {
            if (currentPanel != null)
            {
                currentPanel.SetActive(false); // Hide current panel
            }

            isPanelOpen = !isPanelOpen;
            currentPanel = uiPanels[index];
            currentPanel.SetActive(isPanelOpen);

            // Toggle crosshair visibility
            if (crosshair != null)
            {
                crosshair.SetActive(!isPanelOpen);
            }
        }
    }

    void ClosePanel()
    {
        if (currentPanel != null)
        {
            currentPanel.SetActive(false);
            isPanelOpen = false;
        }

        // Ensure the crosshair is visible again when closing the panel
        if (crosshair != null)
        {
            crosshair.SetActive(true);
        }
    }
}
