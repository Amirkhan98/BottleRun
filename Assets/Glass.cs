using DG.Tweening;
using UnityEngine;

public class Glass : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        GetComponent<Collider>().enabled = false;
        transform.DOMoveY(transform.position.y + 0.1f, 0.1f)
            .OnComplete(() =>
            {
                transform.DOScale(transform.localScale * 10f, 0.2f);
                Debug.Log("Do scale");
            })
            .OnComplete(() =>
            {
                transform.GetChild(0).gameObject.SetActive(true);
                Debug.Log("asdfasdf");
            })
            .OnComplete(() => transform.DOScale(transform.localScale / 10f, 0.2f))
            .OnComplete(() => transform.DOMoveY(transform.position.y - 0.1f, 0.1f));
    }
}