using System.Collections;
using Amir.UI;
using DG.Tweening;
using Lean.Transition.Method;
using UnityEngine;

public class CupTower : MonoBehaviour
{
    private float yOffset = 0;
    private float xOffset = 0;
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
        bool fiveRow = (StaticManager.GetLevelID() + 1) % 3 == 0;

        int i = 0;
        foreach (Transform cup in StaticManager.instance.levelManager.currentLevel.transform.GetChild(0).transform)
        {
            if (cup.GetComponent<Cup>().filled)
            {
                i++;
                cup.position = cupSpawnPos.position;
                StaticManager.instance.filledWineGlasses--;
                UIManager.instance.inGameGroup.SetWineGlassesCountText(StaticManager.instance.filledWineGlasses);

                if (fiveRow)
                {
                    cup.transform.position = transform.position + new Vector3(xOffset, yOffset, 0);
                    cup.GetComponent<BoxCollider>().enabled = true;
                    cup.GetComponent<Cup>().RunEffect();
                    xOffset += 0.3f;
                    if (i % 6 == 0)
                    {
                        Debug.Log("i = " + i);
                        Camera.main.transform.DOMoveY((transform.position + new Vector3(0, yOffset, 0)).y + 3f, 0.5f);
                        yOffset += 0.4f;
                        xOffset = 0;
                        yield return new WaitForSeconds(0.02f);
                    }
                }
                else
                {
                    cup.DOMove(transform.position + new Vector3(0, yOffset, 0), 0.3f).OnComplete(() =>
                    {
                        cup.GetComponent<BoxCollider>().enabled = true;
                        cup.GetComponent<Cup>().RunEffect();
                    });
                    yOffset += 0.4f;
                    Camera.main.transform.DOMoveY((transform.position + new Vector3(0, yOffset, 0)).y + 3f, 0.2f);
                    yield return new WaitForSeconds(0.2f);
                }
            }
        }

        yield return new WaitForSeconds(1f);
        UIManager.instance.ShowWinScreen();
    }
}