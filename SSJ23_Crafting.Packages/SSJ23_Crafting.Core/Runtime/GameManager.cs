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

    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] CardDeckData playerOneDeckData;
        [SerializeField] CardDeckData playerTwoDeckData;

        public GameState State { get; private set; }
        public Player PlayerOne { get; private set; }
        public Player PlayerTwo { get; private set; }

        private GameEvents events;


        public override void Awake()
        {
            base.Awake();

            events = GameEvents.FindOrCreateInstance();

            PlayerOne = new Player(PlayerId.One, new UserInputController());
            PlayerOne.Deck.SetSource(playerOneDeckData);
            PlayerOne.Deck.Populate();
            PlayerOne.Deck.Shuffle();

            PlayerTwo = new Player(PlayerId.Two, new ComputerController());
            PlayerTwo.Deck.SetSource(playerTwoDeckData);
            PlayerTwo.Deck.Populate();
            PlayerTwo.Deck.Shuffle();
        }

        private IEnumerator Start()
        {
            Debug.Log("Game Starting");

            Debug.Log("Drawing Player Hand");
            while (PlayerOne.Hand.CardCount < GameSettings.MaxHandSize)
            {
                if (PlayerOne.Deck.IsEmpty)
                {
                    // Temp: When deck is empty re populate and shuffle
                    // Debug.Log("Can not draw more cards. Deck is empty");
                    // break;

                    PlayerOne.Deck.Populate();
                    PlayerOne.Deck.Shuffle();
                }

                if (PlayerOne.Deck.TryDraw(out var card))
                {
                    PlayerOne.Hand.AddCard(card);
                    events.CardDrawn.Emit(new CardEventArgs
                    {
                        playerId = PlayerOne.Id,
                        card = card
                    });
                    Debug.Log($"Player Drawing Card {PlayerOne.Hand.CardCount}");

                    // Todo: Wait for draw animation to finish
                    yield return new WaitForSeconds(0.1f);
                }
                else
                {
                    Debug.Log("Failed to draw card from deck");
                }
            }

            Debug.Log("Drawing Enemy Hand");
            while (PlayerTwo.Hand.CardCount < GameSettings.MaxHandSize)
            {
                if (PlayerTwo.Deck.IsEmpty)
                {
                    PlayerTwo.Deck.Populate();
                    PlayerTwo.Deck.Shuffle();
                }

                if (PlayerTwo.Deck.TryDraw(out var card))
                {
                    PlayerTwo.Hand.AddCard(card);
                    events.CardDrawn.Emit(new CardEventArgs
                    {
                        playerId = PlayerTwo.Id,
                        card = card
                    });
                }
            }

            // for (int i = 3; i > 0; i--)
            // {
            //     Debug.Log($"Count Down: {i}");
            //     yield return new WaitForSeconds(1.0f);
            // }

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

        public bool LaunchRobot(PlayerId playerId)
        {
            if (playerId == PlayerId.One)
            {
                return PlayerOne.LaunchRobot();
            }
            else
            {
                return PlayerTwo.LaunchRobot();
            }
        }

        public bool UseCard(PlayerId playerId, CardData card)
        {
            if (playerId == PlayerId.One)
            {
                return PlayerOne.UseCard(card);
            }
            else
            {
                return PlayerTwo.UseCard(card);
            }
        }

        public bool DiscardCard(PlayerId playerId, CardData card)
        {
            if (playerId == PlayerId.One)
            {
                return PlayerOne.DiscardCard(card);
            }
            else
            {
                return PlayerTwo.DiscardCard(card);
            }
        }
    }
}