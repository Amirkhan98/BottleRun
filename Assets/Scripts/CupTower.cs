using System.Collections;
using Amir.UI;
using DG.Tweening;
using UnityEngine;

public class CupTower : MonoBehaviour
{
    private float offset = 0;
    [SerializeField] private Transform cupSpawnPos;

    private void Start()
    {
        PlayerController.OnFinish += StartBuildCoroutine;
    }

    private void OnDestroy()
    {
        PlayerController.OnFinish -= StartBuildCoroutine;
    }

    void StartBuildCoroutine()
    {
        StartCoroutine("Build");
    }

    IEnumerator Build()
    {
        foreach (Transform cup in StaticManager.instance.levelManager.currentLevel.transform.GetChild(0).transform)
        {
            Debug.Log("Cup name = " + cup.name);
            if (cup.GetComponent<Cup>().filled)
            {
                cup.position = cupSpawnPos.position;
                StaticManager.instance.filledWineGlasses--;
                UIManager.instance.inGameGroup.SetWineGlassesCountText(StaticManager.instance.filledWineGlasses);
                cup.DOMove(transform.position + new Vector3(0, offset, 0), 0.3f).OnComplete(() =>
                {
                    cup.GetComponent<BoxCollider>().enabled = true;
                });
                offset += 0.4f;
                Camera.main.transform.DOMoveY((transform.position + new Vector3(0, offset, 0)).y + 3f, 0.2f);
                yield return new WaitForSeconds(0.2f);
            }
        }

        yield return new WaitForSeconds(1f);
        UIManager.instance.ShowWinScreen();
    }
}