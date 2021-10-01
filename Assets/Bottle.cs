using UnityEngine;

public class Bottle : MonoBehaviour
{
    [SerializeField] private ParticleSystem fluid;
    [SerializeField] private GameObject wrapper;
    [SerializeField] private GameObject paint;

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
            fluid.gameObject.SetActive(false);
            wrapper.GetComponent<Rigidbody>().isKinematic = false;
            wrapper.transform.parent = null;
            paint.SetActive(false);
            Destroy(this);
        }
    }

    private void Update()
    {
    }
}