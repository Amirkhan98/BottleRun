using System;
using DG.Tweening;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float speed;
    private bool putToRight = true;
    private float offset = 0.16f;
    private int count = 1;

    void Update()
    {
        transform.Translate(0, 0, speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bottle"))
        {
            other.transform.parent = transform;
            if (putToRight)
            {
                other.transform.DOLocalMove(new Vector3(offset * count, 0, 0), 1f);
                putToRight = false;
            }
            else
            {
                other.transform.DOLocalMove(new Vector3(-offset * count, 0, 0), 1f);
                putToRight = true;
                count++;
            }
        }
    }
}