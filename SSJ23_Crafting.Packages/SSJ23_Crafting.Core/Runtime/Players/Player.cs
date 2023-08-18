using System;
using UnityEngine;

namespace SSJ23_Crafting
{
    public enum PlayerId
    {
        One,
        Two,
    }

    public class Player
    {
        public PlayerId Id { get; set; }
        public bool IsEnabled { get; private set; } = false;

        public int Score { get; private set; }
        public float Resource { get; private set; }

        public CardDeck Deck { get; private set; }
        public CardHand Hand { get; private set; }
        public Robot Robot { get; private set; }

        public PlayerController Controller { get; private set; }

        private GameManager gameManager;
        private GameEvents events;

        public event Action<CardData> CardDrawn;
        public event Action<CardData> CardDiscarded;
        public event Action<CardData> CardUsed;

        public Player(PlayerId id, PlayerController controller)
        {
            Id = id;
            Controller = controller;
            Deck = new CardDeck();
            Hand = new CardHand();

            Resource = 0;
            Score = 0;

            gameManager = GameManager.FindOrCreateInstance();
            events = GameEvents.FindOrCreateInstance();
        }

        public bool IsCardUsable(CardData card)
        {
            if (Resource < card.ResourceCost)
            {
                return false;
            }

            return true;
        }

        public bool UseCard(CardData card)
        {
            if (!IsCardUsable(card) || !Hand.RemoveCard(card))
            {
                return false;
            }

            // Card is usable and was in hand
            Resource -= card.ResourceCost;
            events.CardUsed.Emit(new CardEventArgs
            {
                playerId = Id,
                card = card
            });

            card.OnUse(this);

            FillHand();
            return true;
        }

        public bool DiscardCard(CardData card)
        {
            if (!Hand.RemoveCard(card))
            {
                return false;
            }

            events.CardDiscarded.Emit(new CardEventArgs
            {
                playerId = Id,
                card = card
            });

            card.OnDiscard(this);

            FillHand();
            return true;
        }

        public void Enable()
        {
            if (IsEnabled)
            {
                return;
            }

            IsEnabled = true;
            Controller.OnEnable(this);
        }

        public void Update()
        {
            if (!IsEnabled)
            {
                return;
            }

            if (Resource < GameSettings.MaxResource)
            {
                float amount = Mathf.Clamp(
                    GameSettings.ResourceRegenRate * Time.deltaTime,
                    0f,
                    GameSettings.MaxResource - Resource
                );

                Resource += amount;
                events.ResourceChanged.Emit(new ResourceEventArgs
                {
                    playerId = Id,
                    amount = amount,
                    totalValue = Resource
                });
            }

            Controller.OnUpdate(this);
        }

        public void Disable()
        {
            if (!IsEnabled)
            {
                return;
            }

            IsEnabled = false;
            Controller.OnDisable(this);
        }

        public void FillHand()
        {
            while (Hand.CardCount < GameSettings.MaxHandSize)
            {
                if (Deck.IsEmpty && GameSettings.RepopulateEmptyDeck)
                {
                    Deck.Populate();
                    Deck.Shuffle();
                }

                if (Deck.TryDraw(out var card))
                {
                    Hand.AddCard(card);
                    events.CardDrawn.Emit(new CardEventArgs
                    {
                        playerId = Id,
                        card = card
                    });
                }
                else
                {
                    // Failed to draw card from deck, deck may be empty
                    break;
                }
            }
        }

        public bool LaunchRobot()
        {
            return false;
        }

        public void ChangeRobot(Robot robotPrefab)
        {
            EjectRobot();

            robotPrefab.gameObject.SetActive(false);
            var instance = GameObject.Instantiate(robotPrefab);
            
            var parent = gameManager.GetRobotParent(Id);
            if (parent != null)
            {
                instance.transform.position = parent.transform.position;
                instance.transform.rotation = parent.transform.rotation;
            }

            Robot = instance;
            instance.gameObject.SetActive(true);
        }

        public void EjectRobot()
        {
            if (Robot == null)
            {
                return;
            }

            // Future Eject robot code here
            GameObject.Destroy(Robot.gameObject);
        }
    }
}