using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

namespace Amir.UI
{
    [Serializable]
    public class EndGameStars
    {
        public ParticleSystem sparkParticle;
        public Image coloredStarImage;
        public float showDuration = 0.5f;
        public float showDelay = 0.5f;

        public void ShowStar()
        {
            coloredStarImage.transform.DOScale(1, showDuration)
                .SetEase(Ease.OutBounce)
                .SetDelay(showDelay)
                .OnComplete(() =>
                {
                    sparkParticle.Play();
                    StaticManager.LightVibrate();
                });
        }

        public void ResetStar()
        {
            coloredStarImage.transform.localScale = Vector3.zero;
        }
    }

    public class WinScreen : CanvasGroupWindow
    {
        public List<EndGameStars> endGameStars = new List<EndGameStars>();
        [SerializeField] private Button claimButton;
        [SerializeField] private TMP_Text completedLevelLabel;
        [SerializeField] private TMP_Text earningsMoneyCount;
        [SerializeField] private ParticleSystem confettiParticles;

        private void Awake()
        {
            claimButton.onClick.AddListener(() =>
            {
                StaticManager.levelID++;
                claimButton.interactable = false;
                claimButton.transform.localScale = Vector3.zero;
                StaticManager.AddMoney(StaticManager.moneyCollectedOnLevel);
                StaticManager.SavePlayerData();
                StaticManager.Restart();
                UIManager.instance.ShowMenu();
                // StaticManager.LoadNextLevel();
                Destroy(StaticManager.instance.gameObject);
                PlayerController.moving = true;
                SceneManager.LoadScene(0);
            });
        }

        /// <summary>
        /// Show received stars on a level
        /// </summary>
        public void ShowEndGameStars()
        {
            if (StaticManager.starsCollected > -1)
                for (int i = 0; i <= StaticManager.starsCollected; i++)
                {
                    endGameStars[i].ShowStar();
                    if (i == StaticManager.starsCollected)
                        confettiParticles.Play();
                }
        }

        public override void ShowWindow(Action onCompleted = null)
        {
            int decrementLevel =
                StaticManager.levelID + 1; // because after level completed we increase levelID immediately
            earningsMoneyCount.text = "0";
            completedLevelLabel.text = "LEVEL " + decrementLevel + "\n COMPLETED";
            base.ShowWindow(() =>
            {
                ShowEndGameStars();
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
                for (int i = 0; i <= endGameStars.Count; i++)
                {
                    endGameStars[i].ResetStar();
                }
            });
        }

        private void ClaimCoins()
        {
            SetEarnings(StaticManager.moneyCollectedOnLevel);
            StartCoroutine(WaitAndShowMultiplier());
        }

        public Transform multiplierPanel;
        public TMP_Text multiplierText;
        public ParticleSystem multiplierParticles;

        IEnumerator WaitAndShowMultiplier()
        {
            multiplierText.text = "x" + StaticManager.currentMoneyMultiplier;
            yield return new WaitForSeconds(1f);
            multiplierPanel.DOScale(1, 0.3f)
                .SetEase(Ease.OutBounce);

            yield return new WaitForSeconds(0.5f);
            multiplierParticles.Play();
            SetEarnings(StaticManager.moneyCollectedOnLevel, StaticManager.currentMoneyMultiplier);
            yield return new WaitForSeconds(1f);
            multiplierPanel.DOScale(0, 0.3f)
                .SetEase(Ease.InBounce);
        }

        private void SetEarnings(int moneyValue, float multiplier = 1)
        {
            float money = moneyValue;
            if (multiplier > 1)
            {
                money *= multiplier;
            }

            float currentCoinsVal = int.Parse(earningsMoneyCount.text);
            StaticManager.moneyCollectedOnLevel = (int) money;
            DOTween.To(() => currentCoinsVal, x => currentCoinsVal = x, money, 0.3f).OnUpdate(() =>
            {
                currentCoinsVal = (int) Mathf.Round(currentCoinsVal);
                earningsMoneyCount.text = currentCoinsVal + "";
            }).SetDelay(fadeTime);

            // claimButton.transform.DOScale(1, 0.2f)
            //     .SetDelay(fadeTime + 3.5f)
            //     .SetEase(Ease.InOutSine);
        }
    }
}