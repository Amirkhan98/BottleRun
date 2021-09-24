using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float speed;

    void Update()
    {
        transform.Translate(0, 0, speed * Time.deltaTime);
    }
}