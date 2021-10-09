using DG.Tweening;
using UnityEngine;

public class ResultMultiplyer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GetComponent<BoxCollider>().enabled = false;
        var oldColor = GetComponent<Renderer>().material.color;
        GetComponent<Renderer>().material.DOColor(Color.yellow, 0.5f).OnComplete(() =>
        {
            GetComponent<Renderer>().material.DOColor(oldColor, 0.5f);
        });
    }
}