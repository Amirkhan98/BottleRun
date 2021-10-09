using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Amir.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupWindow : MonoBehaviour
    {
        private CanvasGroup canvasGroup;
        [SerializeField] private bool hasCloseButton;

        /// <summary>
        /// If Screen|Window have close button set it in inspector 
        /// and set onClick Listener in Start.
        /// </summary>
        [ShowIf("hasCloseButton")] [SerializeField]
        Button closeButton;

        /// <summary>
        /// Tween animation ease.
        /// </summary>
        [SerializeField] private Ease ease;

        [SerializeField] internal float fadeTime;
        [SerializeField] internal float showDelay = 0f;
        [SerializeField] private float hideDelay = 0f;
        [SerializeField] private bool fullInteractable = true;
        [SerializeField] private TextMeshProUGUI wineGlassesCountText;

        void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (hasCloseButton)
                closeButton.onClick.AddListener(() => { HideWindow(); });
        }

        public void SetWineGlassesCountText(int number)
        {
            wineGlassesCountText.text = number.ToString();
        }


        public virtual void ShowWindow(Action onCompleted = null)
        {
            //Debug.LogError("ShowWindow CanvasGroupWindow");
            canvasGroup.DOFade(1f, fadeTime).SetEase(ease).OnComplete(() =>
            {
                if (fullInteractable)
                {
                    canvasGroup.blocksRaycasts = true;
                    canvasGroup.interactable = true;
                }

                onCompleted?.Invoke();
            }).SetUpdate(true).SetDelay(showDelay);
        }

        /// <summary>
        /// For UnityEvent in inspector
        /// </summary>
        public void SimpleShowWindow()
        {
            canvasGroup.DOFade(1f, fadeTime).SetEase(ease).OnComplete(() =>
            {
                if (fullInteractable)
                {
                    canvasGroup.blocksRaycasts = true;
                    canvasGroup.interactable = true;
                }
            }).SetUpdate(true).SetDelay(showDelay);
        }

        public virtual void HideWindow()
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            canvasGroup.DOFade(0f, fadeTime).SetEase(ease).SetDelay(hideDelay).SetUpdate(true);
        }

        public virtual void HideWindow(Action onCompleted = null)
        {
            //Debug.LogError("ShowWindow CanvasGroupWindow");
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            canvasGroup.DOFade(0f, fadeTime).SetEase(ease).OnComplete(() => { onCompleted?.Invoke(); }).SetUpdate(true)
                .SetDelay(hideDelay);
        }

        public virtual void DisableWindow()
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            canvasGroup.alpha = 0;
        }

        private void Update()
        {
        }
    }
}