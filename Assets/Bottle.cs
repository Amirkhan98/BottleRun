using UnityEngine;

public class Bottle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            transform.parent = null;
            gameObject.AddComponent<Rigidbody>();
        }
    }
}