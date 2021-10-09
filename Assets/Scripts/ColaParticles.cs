using System.Collections.Generic;
using UnityEngine;

public class ColaParticles : MonoBehaviour
{
    [SerializeField] private int followDistance;
    private List<Vector3> storedPositions;
    private float nextActionTime;
    public float period = 0.1f;
    private Vector3 followPosition;

    void Awake()
    {
        storedPositions = new List<Vector3>(); //create a blank list
    }

    void Update()
    {
        Vector3 playerPos = transform.position;

        if (storedPositions.Count == 0) //check if the list is empty
        {
            storedPositions.Add(playerPos); //store the players currect position
            return;
        }

        if (storedPositions[storedPositions.Count - 1] != playerPos)
        {
            storedPositions.Add(playerPos); //store the position every frame
        }

        else if (transform.position.y != playerPos.y) //check if the ally is airborne and correct it if necessary
        {
            storedPositions.Add(playerPos);
        }

        if (storedPositions.Count > followDistance)
        {
            followPosition = new Vector3(storedPositions[0].x, 0, transform.position.z);
            storedPositions.RemoveAt(0); //delete the position that player just move to
        }

        if (Time.time > nextActionTime)
        {
            nextActionTime += period * Time.deltaTime;
            LiquidEffect.onParticleHit.Invoke(followPosition);
        }
    }
}