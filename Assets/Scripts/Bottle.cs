using System;
using UnityEngine;

public class Bottle : MonoBehaviour
{
    [SerializeField] private ParticleSystem fluid;
    public bool canTrigger;

    private void Start()
    {
        PlayerController.OnFinish += DisbleFluid;
    }

    private void OnDisable()
    {
        PlayerController.OnFinish -= DisbleFluid;
    }

    private void DisbleFluid()
    {
        fluid.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canTrigger) return;
        if (other.CompareTag("Bottle"))
        {
            transform.parent.GetComponent<PlayerController>().OnTriggerEnterChild(other);
        }

        if (other.CompareTag("Obstacle"))
        {
            GetComponent<MeshRenderer>().materials[1].color = new Color32(49, 13, 12, 255);
            PlayerController.onObstacleHit(gameObject);
            Debug.Log("Postion = " + transform.position);
            transform.parent = null;
            GetComponent<Rigidbody>().isKinematic = false;
            fluid.gameObject.SetActive(false);
            Destroy(this);
        }

        if (other.name == "Finish" && !StaticManager.levelFinished)
        {
            PlayerController.OnFinish.Invoke();
            StaticManager.levelFinished = true;
        }
    }

    public void SetActivePaintAndFluid()
    {
        // paint.SetActive(true);
        fluid.gameObject.SetActive(true);
        var space = fluid.main;
        space.customSimulationSpace = Camera.main.transform;
    }

    private void Update()
    {
    }
}