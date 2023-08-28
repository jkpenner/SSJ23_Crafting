using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

namespace SSJ23_Crafting
{
    [RequireComponent(typeof(CanvasGroup))]
    public class GameOverStateController : GameStateController
    {
        [SerializeField] float timeout = 10f;
        [SerializeField] InputActionAsset inputs;
        [SerializeField] string screenTapActionPath = "UI/ScreenTap";
        
        [SerializeField] AudioSource clickSound;

        [Header("UI")]
        [SerializeField] TMP_Text playerOneScore;
        [SerializeField] TMP_Text playerTwoScore;

        private CanvasGroup canvasGroup;
        private InputAction escapeAction;
        private InputAction screenPressAction;

        private bool defaultInteractable;
        private bool defaultBlocksRaycasts;
        private float counter = 0f;

        public override GameState[] ControlledStates => new GameState[] {
            GameState.GameOver
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

        private void Update()
        {
            if (gameManager.State != GameState.GameOver)
            {
                return;
            }

            counter += Time.deltaTime;
            if (counter >= timeout)
            {
                gameManager.SetGameState(GameState.MainMenu);
            }
        }

        public override IEnumerator OnEnterState(GameState state)
        {
            playerOneScore.SetText(gameManager.PlayerOne.Score.ToString());
            playerTwoScore.SetText(gameManager.PlayerTwo.Score.ToString());

            yield return Show();
            escapeAction.performed += OnEscape;
            screenPressAction.performed += OnScreenPressed;
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
            Application.Quit();
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