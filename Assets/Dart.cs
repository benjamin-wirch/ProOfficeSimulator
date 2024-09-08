using UnityEngine;

public class Dart : MonoBehaviour
{
    private Rigidbody rb;
    private bool hasHit = false;

    public void Setup(Rigidbody rigidbody)
    {
        rb = rigidbody;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!hasHit && collision.transform.CompareTag("Dartboard"))
        {
            Debug.Log("Dart hit the dartboard!");

            hasHit = true;

            // Stop the dart's movement
            rb.isKinematic = true;

            // Align dart to the surface
            ContactPoint contact = collision.contacts[0];
            Vector3 normal = contact.normal;
            transform.position = contact.point;
            transform.rotation = Quaternion.LookRotation(-normal, Vector3.up);

            // Optionally, parent the dart to the dartboard to ensure it stays attached
            transform.SetParent(collision.transform);
        }
    }
}
