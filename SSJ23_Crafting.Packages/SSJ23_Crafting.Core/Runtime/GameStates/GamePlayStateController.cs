using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SSJ23_Crafting
{
    [RequireComponent(typeof(CanvasGroup))]
    public class GamePlayStateController : GameStateController
    {
        [SerializeField] InputActionAsset inputs;
        
        [SerializeField] TMP_Text playerOneScore;
        [SerializeField] TMP_Text playerTwoScore;
        [SerializeField] TMP_Text gameTime;

        private CanvasGroup canvasGroup;

        private bool defaultInteractable;
        private bool defaultBlocksRaycasts;

        private Coroutine endRoutine;

        private InputAction escapeAction;

        public float GameTime { get; private set; }

        public override GameState[] ControlledStates => new GameState[] {
            GameState.Starting, GameState.GamePlay
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

            escapeAction = inputs.FindAction("UI/Escape");
        }

        public override IEnumerator OnEnterState(GameState state)
        {
            escapeAction.performed += OnEscape;

            if (state == GameState.Starting)
            {
                GameTime = gameManager.Settings.GameRoundDuration;

                gameManager.PlayerOne.ClearScore();
                gameManager.PlayerTwo.ClearScore();

                gameTime.SetText(Mathf.RoundToInt(GameTime).ToString());
                playerOneScore.SetText("0");
                playerTwoScore.SetText("0");

                yield return Show();

                gameManager.PlayerOne.Deck.Populate();
                gameManager.PlayerOne.Deck.Shuffle();
                gameManager.PlayerOne.FillHand();

                gameManager.PlayerTwo.Deck.Populate();
                gameManager.PlayerTwo.Deck.Shuffle();
                gameManager.PlayerTwo.FillHand();



                yield return null;
            }
            else if (state == GameState.GamePlay)
            {
                gameManager.PlayerOne.Enable();
                gameManager.PlayerTwo.Enable();

                
            }
        }

        public override IEnumerator OnExitState(GameState state)
        {
            escapeAction.performed -= OnEscape;

            if (state == GameState.GamePlay)
            {
                gameManager.PlayerOne.Disable();
                gameManager.PlayerTwo.Disable();


                gameManager.PlayerOne.EmptyHand();
                gameManager.PlayerTwo.EmptyHand();

                foreach (var robot in GameObject.FindObjectsByType<Robot>(FindObjectsSortMode.None))
                {
                    robot.Explode();
                }

                gameEvents.GameOver.Emit();

                yield return Hide();

                // Handle Clean up here...
            }

            
        }

        private void OnEscape(InputAction.CallbackContext context)
        {
            gameManager.SetGameState(GameState.GameOver);
        }

        private void Update()
        {
            if (gameManager.State == GameState.Starting)
            {
                gameManager.SetGameState(GameState.GamePlay);
            }

            if (gameManager.State != GameState.GamePlay)
            {
                return;
            }

            GameTime -= Time.deltaTime;
            if (GameTime <= 0f)
            {
                gameManager.SetGameState(GameState.GameOver);
            }

            gameManager.PlayerOne.Update();
            gameManager.PlayerTwo.Update();

            gameTime.SetText(Mathf.Max(Mathf.RoundToInt(GameTime), 0).ToString());
            playerOneScore.SetText(gameManager.PlayerOne.Score.ToString());
            playerTwoScore.SetText(gameManager.PlayerTwo.Score.ToString());

            if (gameManager.PlayerOne.Score >= gameManager.Settings.ScoreToWin
                || gameManager.PlayerTwo.Score >= gameManager.Settings.ScoreToWin)
            {
                gameManager.SetGameState(GameState.GameOver);
            }
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