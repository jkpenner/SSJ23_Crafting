using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSJ23_Crafting
{
    public class UIBuild : MonoBehaviour
    {
        [SerializeField] UIBuildElement elementPrefab;
        [SerializeField] RectTransform elementParent;

        [SerializeField] CanvasGroup invalidGroup;
        [SerializeField] CanvasGroup activeGroup;

        private GameManager gameManager;
        private GameEvents gameEvents;

        private List<UIBuildElement> elements;
        private Coroutine transition;

        private void Awake()
        {
            invalidGroup.alpha = 1f;
            activeGroup.alpha = 0f;

            gameManager = GameManager.FindOrCreateInstance();
            gameEvents = GameEvents.FindOrCreateInstance();

            elements = new List<UIBuildElement>();
        }

        private void OnEnable()
        {
            
            gameEvents.RobotChanged.Register(OnRobotChanged);
            gameEvents.CardAttached.Register(OnCardAttached);
            gameEvents.CardDetached.Register(OnCardDetached);
        }

        private void OnDisable()
        {
            gameEvents.RobotChanged.Unregister(OnRobotChanged);
            gameEvents.CardAttached.Unregister(OnCardAttached);
            gameEvents.CardDetached.Unregister(OnCardDetached);
        }

        private void OnRobotChanged(RobotEventArgs args)
        {
            if (args.playerId != PlayerId.One)
            {
                return;
            }

            if (args.robot != null)
            {
                UpdateDisplayedElements();
                ShowActiveGroup();
            }
            else
            {
                ShowInvalidGroup();
            }
        }

        private void OnCardAttached(AttachmentEventArgs args)
        {
            if (args.playerId != PlayerId.One)
            {
                return;
            }

            foreach (var element in elements)
            {
                element.UpdateValues();
            }
        }

        private void OnCardDetached(AttachmentEventArgs args)
        {
            if (args.playerId != PlayerId.One)
            {
                return;
            }

            foreach (var element in elements)
            {
                element.UpdateValues();
            }
        }

        private void UpdateDisplayedElements()
        {
            foreach (var element in elements)
            {
                Destroy(element.gameObject);
            }
            elements.Clear();


            foreach (var slot in gameManager.PlayerOne.Robot.Slots)
            {
                var element = Instantiate(elementPrefab, elementParent);
                element.SetSlot(slot);
                element.UpdateValues();
                elements.Add(element);
            }
        }

        private void ShowInvalidGroup()
        {
            if (transition != null)
            {
                StopCoroutine(transition);
            }

            transition = StartCoroutine(ShowInvalidGroupRoutine());
        }

        private void ShowActiveGroup()
        {
            if (transition != null)
            {
                StopCoroutine(transition);
            }

            transition = StartCoroutine(ShowActiveGroupRoutine());
        }

        private IEnumerator ShowInvalidGroupRoutine()
        {
            while (activeGroup.alpha >= 0f)
            {
                activeGroup.alpha -= Time.deltaTime;
                yield return null;
            }

            while (invalidGroup.alpha <= 1f)
            {
                invalidGroup.alpha += Time.deltaTime;
                yield return null;
            }

            activeGroup.alpha = 0f;
            invalidGroup.alpha = 1f;
            transition = null;
        }

        private IEnumerator ShowActiveGroupRoutine()
        {
            while (invalidGroup.alpha > 0)
            {
                invalidGroup.alpha -= Time.deltaTime;
                yield return null;
            }

            while (activeGroup.alpha < 1)
            {
                activeGroup.alpha += Time.deltaTime;
                yield return null;
            }

            activeGroup.alpha = 1f;
            invalidGroup.alpha = 0f;
            transition = null;
        }
    }
}