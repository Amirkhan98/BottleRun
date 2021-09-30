using UnityEngine;

public class Bottle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bottle"))
        {
            Debug.Log(other.name);
            transform.parent.GetComponent<Movement>().OnTriggerEnterChild(other);
        }

        if (other.CompareTag("Obstacle"))
        {
            transform.parent = null;
            GetComponent<Rigidbody>().isKinematic = false;
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).GetComponent<Rigidbody>().isKinematic = false;
            transform.GetChild(1).parent = null;
            Destroy(this);
        }
    }

    private void Update()
    {
    }
}