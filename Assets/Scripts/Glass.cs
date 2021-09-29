using DG.Tweening;
using UnityEngine;

public class Glass : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        GetComponent<Collider>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(true);
        transform.DOMoveY(transform.position.y + 0.1f, 0.1f)
            .OnComplete(() =>
                transform.DOScale(transform.localScale * 1.5f, 0.2f)
                    .OnComplete(() => transform.DOScale(transform.localScale / 1.5f, 0.2f)
                        .OnComplete(() => { transform.DOMoveY(transform.position.y - 0.1f, 0.1f); }))
            );
    }
}