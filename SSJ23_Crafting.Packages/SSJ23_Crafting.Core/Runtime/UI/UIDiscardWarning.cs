using System;
using System.Collections;
using UnityEngine;

namespace SSJ23_Crafting
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIDiscardWarning : MonoBehaviour
    {
        private GameEvents events;
        private CanvasGroup canvasGroup;
        private Coroutine coroutine;

        private float transitionRate = 0.2f;

        public bool IsVisible { get; private set; } = false;


        private void Awake()
        {
            events = GameEvents.FindOrCreateInstance();
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
        }

        private void OnEnable()
        {
            events.ShowDiscard.Register(OnShowDiscard);
            events.HideDiscard.Register(OnHideDiscard);
        }

        private void OnDisable()
        {
            events.ShowDiscard.Unregister(OnShowDiscard);
            events.HideDiscard.Unregister(OnHideDiscard);
        }

        private void OnHideDiscard()
        {
            IsVisible = false;
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(Hide());
        }

        private void OnShowDiscard()
        {
            IsVisible = true;
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(Show());
        }

        private IEnumerator Show()
        {
            var speed = 1.0f / transitionRate;
            while (canvasGroup.alpha < 1f)
            {
                canvasGroup.alpha += speed * Time.deltaTime;
                canvasGroup.alpha = Mathf.Min(canvasGroup.alpha, 1f);
                yield return null;
            }
            coroutine = null;
        }

        private IEnumerator Hide()
        {
            var speed = 1.0f / transitionRate;
            while (canvasGroup.alpha > 0f)
            {
                canvasGroup.alpha -= speed * Time.deltaTime;
                canvasGroup.alpha = Mathf.Max(canvasGroup.alpha, 0f);
                yield return null;
            }
            coroutine = null;
        }
    }
}