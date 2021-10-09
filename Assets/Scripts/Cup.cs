using DG.Tweening;
using UnityEngine;

public class Cup : MonoBehaviour
{
    [SerializeField] private GameObject liquid;
    [SerializeField] private GameObject particles;
    [SerializeField] private Transform surface;
    public bool filled;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bottle"))
        {
            filled = true;
            PlayerController.OnWineGlassFill.Invoke();
            GetComponent<Collider>().enabled = false;
            liquid.SetActive(true);
            liquid.transform.DOLocalMoveY(surface.transform.localPosition.y, 0.5f);
            liquid.transform.DOScale(surface.transform.localScale, 0.5f).OnComplete(() =>
            {
                particles.gameObject.SetActive(true);
            });

            transform.DOMoveY(transform.position.y + 0.1f, 0.1f)
                .OnComplete(() =>
                    transform.DOScale(transform.localScale * 1.3f, 0.2f)
                        .OnComplete(() => transform.DOScale(transform.localScale / 1.3f, 0.2f)
                            .OnComplete(() => { transform.DOMoveY(transform.position.y - 0.1f, 0.1f); }))
                );
        }
    }
}