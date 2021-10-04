using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
//using Deslab.Scripts.Deslytics;

namespace Amir.UI
{
    public class LoseScreen : CanvasGroupWindow
    {
        [SerializeField] private Button claimButton;
        [SerializeField] private TMP_Text failedLevelLabel;
        [SerializeField] private TMP_Text earningsMoneyCount;

        private void Awake()
        {
            claimButton.onClick.AddListener(() =>
            {
                claimButton.interactable = false;
                StaticManager.AddMoney(StaticManager.moneyCollectedOnLevel);
                StaticManager.Restart();
                StaticManager.reloadLevel = true;
                //DeslyticsManager.LevelRestart();
                UIManager.instance.ShowMenu();
                StaticManager.LoadNextLevel();
        });
        }

        public override void ShowWindow(Action onCompleted = null)
        {
            int failedLevel = StaticManager.levelID;
            earningsMoneyCount.text = "0";
            failedLevelLabel.text = "LEVEL " + failedLevel + "\n FAILED";
            base.ShowWindow(() =>
            {
                ClaimCoins();
                StaticManager.SuccessVibrate();
                claimButton.interactable = true;
            });
        }

        public override void HideWindow()
        {
            base.HideWindow(() =>
            {
                claimButton.transform.localScale = Vector3.zero;
            });
        }

        private void ClaimCoins()
        {
            SetEarnings(StaticManager.moneyCollectedOnLevel);
        }

        private void SetEarnings(int moneyValue, float multiplier = 1)
        {
            float money = moneyValue;
            if (multiplier > 1)
            {
                money *= multiplier;
            }

            float currentCoinsVal = int.Parse(earningsMoneyCount.text);
            StaticManager.moneyCollectedOnLevel = (int)money;
            DOTween.To(() => currentCoinsVal, x => currentCoinsVal = x, money, 0.3f).OnUpdate(() =>
            {
                currentCoinsVal = (int)Mathf.Round(currentCoinsVal);
                earningsMoneyCount.text = currentCoinsVal + "";
            }).SetDelay(fadeTime);
            claimButton.transform.DOScale(1, 0.2f).SetDelay(fadeTime).SetEase(Ease.InOutSine);
        }
    }
}
