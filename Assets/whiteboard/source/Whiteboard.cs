using UnityEngine;

public class Whiteboard : MonoBehaviour
{
    public Camera playerCamera; // Assign your main camera here
    public Texture2D whiteboardTexture; // Assign a blank texture in the inspector
    public Color drawColor = Color.black; // Default color to draw with
    public int brushSize = 5; // Initial size of the brush
    public int maxBrushSize = 20; // Maximum brush size
    public int minBrushSize = 1; // Minimum brush size

    private Renderer whiteboardRenderer;
    private RaycastHit hit;
    private Vector2 textureCoord;

    // Default color for erasing
    private Color defaultColor = Color.white;

    void Start()
    {
        whiteboardRenderer = GetComponent<Renderer>();
        if (whiteboardTexture == null)
        {
            // Initialize a blank white texture if none is assigned
            whiteboardTexture = new Texture2D(1024, 1024, TextureFormat.RGBA32, false);
            whiteboardTexture.wrapMode = TextureWrapMode.Clamp;
            ResetWhiteboard(); // Ensure it starts as a blank whiteboard
        }

        // Assign the texture to the renderer
        whiteboardRenderer.material.mainTexture = whiteboardTexture;
    }

    void Update()
    {
        if (Physics.Raycast(playerCamera.ScreenPointToRay(Input.mousePosition), out hit))
        {
            if (hit.transform == transform)
            {
                // Debugging: Log when ray hits the whiteboard
                Debug.Log("Raycast hit the whiteboard");

                if (Input.GetMouseButton(0))
                {
                    // Get the texture coordinate of the hit point
                    textureCoord = hit.textureCoord;
                    DrawOnTexture(textureCoord);
                }
            }
        }

        // Check for erase input
        if (Input.GetKeyDown(KeyCode.R))
        {
            EraseWhiteboard();
        }

        // Change brush color based on key input
        if (Input.GetKeyDown(KeyCode.T))
        {
            drawColor = Color.red;
            Debug.Log("Brush color changed to Red");
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            drawColor = Color.green;
            Debug.Log("Brush color changed to Green");
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            drawColor = Color.blue;
            Debug.Log("Brush color changed to Blue");
        }

        // Adjust brush size
        if (Input.GetKeyDown(KeyCode.Equals)) // "+" key (shift + "=")
        {
            brushSize = Mathf.Min(brushSize + 1, maxBrushSize);
            Debug.Log($"Brush size increased to {brushSize}");
        }
        else if (Input.GetKeyDown(KeyCode.Minus)) // "-" key
        {
            brushSize = Mathf.Max(brushSize - 1, minBrushSize);
            Debug.Log($"Brush size decreased to {brushSize}");
        }
    }

    void DrawOnTexture(Vector2 textureCoord)
    {
        int x = (int)(textureCoord.x * whiteboardTexture.width);
        int y = (int)(textureCoord.y * whiteboardTexture.height);

        // Debugging: Log drawing coordinates
        Debug.Log($"Drawing at coordinates: {x}, {y}");

        // Set pixels in a square area to the draw color
        for (int i = -brushSize; i <= brushSize; i++)
        {
            for (int j = -brushSize; j <= brushSize; j++)
            {
                if (x + i >= 0 && x + i < whiteboardTexture.width && y + j >= 0 && y + j < whiteboardTexture.height)
                {
                    whiteboardTexture.SetPixel(x + i, y + j, drawColor);
                }
            }
        }
        whiteboardTexture.Apply(); // Apply changes to the texture
    }

    void EraseWhiteboard()
    {
        Debug.Log("Erasing whiteboard");
        ResetWhiteboard();
    }

    void ResetWhiteboard()
    {
        // Fill the texture with the default color
        Color[] fillPixels = new Color[whiteboardTexture.width * whiteboardTexture.height];
        for (int i = 0; i < fillPixels.Length; i++)
        {
            fillPixels[i] = defaultColor;
        }

        whiteboardTexture.SetPixels(fillPixels);
        whiteboardTexture.Apply();
    }
}
