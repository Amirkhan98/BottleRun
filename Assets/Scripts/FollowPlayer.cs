using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform player;

    void Update()
    {
        if (PlayerController.moving)
            transform.position = new Vector3(transform.position.x, transform.position.y, player.position.z - 4);
    }
}