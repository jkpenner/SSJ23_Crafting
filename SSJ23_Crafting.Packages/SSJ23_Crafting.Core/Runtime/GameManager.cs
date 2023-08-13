using System.Collections;
using UnityEngine;

namespace SSJ23_Crafting
{
    public enum GameState
    {
        Starting,
        Active,
        Complete,
    }

    public class GameManager : MonoBehaviour
    {
        [SerializeField] CardDeckData playerOneDeckData;
        [SerializeField] CardDeckData playerTwoDeckData;

        public GameState State { get; private set; }
        public Player PlayerOne { get; private set; }
        public Player PlayerTwo { get; private set; }


        private void Awake()
        {
            PlayerOne = new Player(PlayerId.One, new UserInputController());
            PlayerOne.Deck.SetSource(playerOneDeckData);

            PlayerTwo = new Player(PlayerId.Two, new ComputerController());
            PlayerTwo.Deck.SetSource(playerTwoDeckData);
        }

        private IEnumerator Start()
        {
            Debug.Log("Game Starting");

            Debug.Log("Drawing Player Hand");
            while (PlayerOne.Hand.CardCount < GameSettings.MaxHandSize && !PlayerOne.Deck.IsEmpty)
            {
                if (PlayerOne.Deck.TryDraw(out var card))
                {
                    PlayerOne.Hand.AddCard(card);
                    Debug.Log($"Player Drawing Card {PlayerOne.Hand.CardCount}");

                    // Todo: Wait for draw animation to finish
                    yield return new WaitForSeconds(0.1f);
                }
            }

            Debug.Log("Drawing Enemy Hand");
            while (PlayerTwo.Hand.CardCount < GameSettings.MaxHandSize && !PlayerTwo.Deck.IsEmpty)
            {
                if (PlayerTwo.Deck.TryDraw(out var card))
                {
                    PlayerTwo.Hand.AddCard(card);
                }
            }

            for (int i = 3; i > 0; i--)
            {
                Debug.Log($"Count Down: {i}");
                yield return new WaitForSeconds(1.0f);
            }

            Debug.Log("Game Started");
            SetGameState(GameState.Active);
        }

        private void Update()
        {
            if (State == GameState.Active)
            {
                PlayerOne.Update();
                PlayerTwo.Update();

                if (PlayerOne.Score >= GameSettings.ScoreToWin || PlayerTwo.Score >= GameSettings.ScoreToWin)
                {
                    SetGameState(GameState.Complete);
                }
            }
        }

        private void SetGameState(GameState state)
        {
            if (state != State)
            {
                State = state;
                Debug.Log($"Entered {State}");

                switch (State)
                {
                    case GameState.Active:
                        PlayerOne.Enable();
                        PlayerTwo.Enable();
                        break;
                    case GameState.Complete:
                        PlayerOne.Disable();
                        PlayerTwo.Disable();
                        break;
                }
            }
        }
    }
}