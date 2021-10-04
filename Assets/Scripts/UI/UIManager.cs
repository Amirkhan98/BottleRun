using System;
using System.Collections.Generic;
using Amir.Level;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

//using Deslab.Scripts.Deslytics;

namespace Amir.UI
{
    [Serializable]
    public struct UpgradePanel
    {
        public TMP_Text level;
        public TMP_Text cost;
    }

    public class UIManager : MonoBehaviour
    {
        public static UIManager instance;

        public LevelManager levelManager;
        public CanvasGroupWindow menuGroup;
        public CanvasGroupWindow inGameGroup;
        public CanvasGroupWindow winScreen;
        public CanvasGroupWindow loseScreen;

        public List<UpgradePanel> upgradePanels = new List<UpgradePanel>();


        private void Awake()
        {
            instance = this;
        }

        /// <summary>
        /// Start game by click UI element
        /// </summary>
        public void StartGame()
        {
            StaticManager.instance.gameStatus = GameStatus.Ride;
            StaticManager.reloadLevel = false;
            levelManager?.StartLevel();
            menuGroup.HideWindow();
            inGameGroup.ShowWindow();
            //DeslyticsManager.LevelStart(StaticManager.levelID);
        }

        /// <summary>
        /// Show Menu Screen after Win|Lose
        /// </summary>
        public void ShowMenu()
        {
            StaticManager.instance.gameStatus = GameStatus.Menu;
            menuGroup.ShowWindow();
            winScreen.HideWindow();
            loseScreen.HideWindow();
            //DeslyticsManager.LevelWin();
        }

        /// <summary>
        /// Show Win Screen after complete level
        /// </summary>
        public void ShowWinScreen()
        {
            StaticManager.instance.gameStatus = GameStatus.Menu;
            inGameGroup.HideWindow();
            winScreen.ShowWindow();
            //HideFlyTutorial();
        }

        /// <summary>
        /// Ident to Win Screen but lose
        /// </summary>
        public void ShowLoseSrceen()
        {
            StaticManager.instance.gameStatus = GameStatus.Menu;
            inGameGroup.HideWindow();
            loseScreen.ShowWindow();
            //DeslyticsManager.LevelFailed();
        }

        /// <summary>
        /// For debug screens
        /// </summary>
        private void Update()
        {
            if (StaticManager.instance.debugMode)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    StaticManager.levelID++;
                    ShowWinScreen();
                }
                if (Input.GetKeyDown(KeyCode.E))
                    ShowLoseSrceen();
            }
        }

        //////////////////////////////////
        //
        //  That region for tutorial objects.
        //
        //////////////////////////////////

        //public GameObject flyTutorial;
        //public void ShowFlyTutorial()
        //{
        //    flyTutorial.SetActive(true);
        //}
        //public void HideFlyTutorial()
        //{
        //    flyTutorial.SetActive(false);
        //}

        public GameObject roadTutorial;
        public void HideRoadTutorial()
        {
            roadTutorial.SetActive(false);
        }



        /// <summary>
        /// Upgrade some Player stats by unique upgrade ID
        /// </summary>
        /// <param name="upgradeID"></param>
        public void Upgrade(int upgradeID)
        {
            PlayerUpgradeData pUpgradeData = StaticManager.instance.playerData.playerUpgradeData[upgradeID];
            if (StaticManager.instance.playerData.money >= pUpgradeData.upgradeCost)
            {
                StaticManager.SubstractMoney(pUpgradeData.upgradeCost);
                pUpgradeData.Upgrade();
                upgradePanels[upgradeID].cost.text = pUpgradeData.upgradeCost.ToString();
                upgradePanels[upgradeID].level.text = "LVL " + pUpgradeData.upgradeLevel;

                StaticManager.instance.playerData.playerUpgradeData[upgradeID] = pUpgradeData;
                StaticManager.SavePlayerData();
            }
        }

        /// <summary>
        /// Set value from loaded Player Stats to Upgrade UI elements
        /// </summary>
        /// <param name="playerUpgradeData"></param>
        public void SetUpgradesStats(List<PlayerUpgradeData> playerUpgradeData)
        {
            List<PlayerUpgradeData> pUpgradeData = playerUpgradeData;
            for (int i = 0; i < pUpgradeData.Count; i++)
            {
                upgradePanels[i].cost.text = pUpgradeData[i].upgradeCost.ToString();
                upgradePanels[i].level.text = "LVL " + pUpgradeData[i].upgradeLevel;
            }
        }
    }

    #region InDev


    //[Serializable]
    //public class LevelStars
    //{
    //    public Image starImage;
    //    Sequence starSequence;
    //    public void ShowStar()
    //    {
    //        starSequence = DOTween.Sequence();

    //        starSequence.Append(starImage.transform.transform.DOLocalMoveY(-650, 1.3f)
    //                                                         .SetEase(Ease.InBack))
    //            .Join(
    //                    starImage.transform.DOLocalRotate(new Vector3(0, 359, 0), 0.7f, RotateMode.FastBeyond360)
    //                                       .SetLoops(-1))
    //            .Join(
    //                    starImage.DOFade(0, 0.7f)
    //                             .SetDelay(0.8f));
    //    }

    //    public void ResetStar()
    //    {
    //        starSequence.Kill();
    //        starImage.color = new Color32(255, 255, 255, 255);
    //        starImage.transform.localRotation = Quaternion.Euler(Vector3.zero);
    //        starImage.transform.localPosition = new Vector3(starImage.transform.localPosition.x, -36, 0);
    //    }
    //}
    #endregion
}
