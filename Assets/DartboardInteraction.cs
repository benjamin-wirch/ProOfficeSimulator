using UnityEngine;
using System.Collections.Generic;

public class DartboardInteraction : MonoBehaviour
{
    public GameObject dartPrefab; // Assign a prefab of the dart
    public float offsetDistance = 0.5f; // Distance from the center of the dart to its tip

    private List<GameObject> spawnedDarts = new List<GameObject>(); // List to keep track of spawned darts

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Check if the clicked object is the dartboard
                if (hit.transform.CompareTag("Dartboard"))
                {
                    SpawnDart(hit.point, hit.normal);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab)) // Tab key
        {
            RemoveAllDarts();
        }
    }

    void SpawnDart(Vector3 position, Vector3 normal)
    {
        // Instantiate a new dart at the hit position
        GameObject dart = Instantiate(dartPrefab, position, Quaternion.identity);

        // Rotate the dart to face the dartboard
        dart.transform.rotation = Quaternion.LookRotation(-normal, Vector3.up);

        // Offset the dart so that only the tip is embedded
        Vector3 offset = dart.transform.forward * offsetDistance;
        dart.transform.position -= offset; // Move the dart backward

        // Parent the dart to the dartboard to ensure it stays attached
        dart.transform.SetParent(transform);

        // Add the dart to the list of spawned darts
        spawnedDarts.Add(dart);
    }

    void RemoveAllDarts()
    {
        foreach (GameObject dart in spawnedDarts)
        {
            if (dart != null)
            {
                Destroy(dart);
            }
        }

        // Clear the list after destroying all darts
        spawnedDarts.Clear();

        Debug.Log("All darts removed from the board.");
    }
}
