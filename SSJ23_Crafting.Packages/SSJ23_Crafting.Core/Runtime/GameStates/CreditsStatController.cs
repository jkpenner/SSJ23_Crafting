using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SSJ23_Crafting
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CreditsStateController : GameStateController
    {
        [SerializeField] InputActionAsset inputs;
        [SerializeField] string screenTapActionPath = "UI/ScreenTap";
        
        [SerializeField] AudioSource clickSound;

        private CanvasGroup canvasGroup;
        private InputAction escapeAction;
        private InputAction screenPressAction;

        private bool defaultInteractable;
        private bool defaultBlocksRaycasts;

        public override GameState[] ControlledStates => new GameState[] {
            GameState.Credits
        };

        protected override void Awake()
        {
            base.Awake();

            canvasGroup = GetComponent<CanvasGroup>();

            defaultBlocksRaycasts = canvasGroup.blocksRaycasts;
            defaultInteractable = canvasGroup.interactable;

            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            screenPressAction = inputs.FindAction(screenTapActionPath);
            escapeAction = inputs.FindAction("UI/Escape");
        }

        public override IEnumerator OnEnterState(GameState state)
        {
            yield return Show();
            screenPressAction.performed += OnScreenPressed;
            escapeAction.performed += OnEscape;
            // inputs.Enable();
        }

        public override IEnumerator OnExitState(GameState state)
        {
            // inputs.Disable();
            escapeAction.performed -= OnEscape;
            screenPressAction.performed -= OnScreenPressed;
            yield return Hide();
        }

        private void OnEscape(InputAction.CallbackContext context)
        {
            gameManager.SetGameState(GameState.MainMenu);
            clickSound.Play();
        }

        private void OnScreenPressed(InputAction.CallbackContext context)
        {
            gameManager.SetGameState(GameState.MainMenu);
            clickSound.Play();
        }

        private IEnumerator Show()
        {
            while (canvasGroup.alpha < 1f)
            {
                canvasGroup.alpha += Time.deltaTime;
                yield return null;
            }

            canvasGroup.alpha = 1f;
            canvasGroup.interactable = defaultInteractable;
            canvasGroup.blocksRaycasts = defaultBlocksRaycasts;
        }

        private IEnumerator Hide()
        {
            while (canvasGroup.alpha > 0f)
            {
                canvasGroup.alpha -= Time.deltaTime;
                yield return null;
            }

            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}