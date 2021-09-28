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

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
            {
                Vector3 touchedPos =
                    Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 10));
                transform.position = Vector3.Lerp(transform.position, touchedPos, Time.deltaTime);
            }
        }
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

            other.transform.DORotate(new Vector3(120, 90, 0), 1f).OnComplete(() =>
            {
                other.transform.GetChild(0).gameObject.SetActive(true);
            });
        }
    }
}