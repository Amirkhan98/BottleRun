using UnityEngine;

public class Bottle : MonoBehaviour
{
    [SerializeField] private ParticleSystem fluid;
    [SerializeField] private GameObject wrapper;
    [SerializeField] private GameObject paint;
    public bool canTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (!canTrigger) return;
        if (other.CompareTag("Bottle"))
        {
            Debug.Log(other.name);
            transform.parent.GetComponent<PlayerController>().OnTriggerEnterChild(other);
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

        if (other.name == "Finish")
        {
            PlayerController.OnFinish.Invoke();
        }
    }

    public void SetActivePaintAndFluid()
    {
        // paint.SetActive(true);
        fluid.gameObject.SetActive(true);
    }

    private void Update()
    {
    }
}