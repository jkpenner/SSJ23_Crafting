using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SSJ23_Crafting
{
    [RequireComponent(typeof(CanvasGroup))]
    public class MainMenuStateController : GameStateController
    {
        [SerializeField] InputActionAsset inputs;
        [SerializeField] string screenTapActionPath = "UI/ScreenTap";
        
        [SerializeField] AudioSource clickSound;

        private CanvasGroup canvasGroup;
        private InputAction screenPressAction;

        private bool defaultInteractable;
        private bool defaultBlocksRaycasts;

        public override GameState[] ControlledStates => new GameState[] {
            GameState.MainMenu
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
            
        }

        public override IEnumerator OnEnterState(GameState state)
        {
            yield return Show();
            screenPressAction.performed += OnScreenPressed;
            // inputs.Enable();
        }

        public override IEnumerator OnExitState(GameState state)
        {
            // inputs.Disable();
            screenPressAction.performed -= OnScreenPressed;
            yield return Hide();
        }

        private void OnScreenPressed(InputAction.CallbackContext context)
        {
            gameManager.SetGameState(GameState.Starting);
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