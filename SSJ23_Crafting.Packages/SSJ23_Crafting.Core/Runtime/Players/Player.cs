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

            Score = 0;

            gameManager = GameManager.FindOrCreateInstance();
            events = GameEvents.FindOrCreateInstance();
        }

        public bool IsCardUsable(CardData card)
        {
            return card.IsUsable(this);
        }

        public bool UseCard(CardData card)
        {
            if (!IsCardUsable(card) || !Hand.RemoveCard(card))
            {
                return false;
            }

            // Card is usable and was in hand
            events.CardUsed.Emit(new CardEventArgs
            {
                playerId = Id,
                card = card
            });

            card.OnCardUse(this);

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

            card.OnCardDiscard(this);

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

            Controller.OnUpdate(this);

            var launchPosition = CalculateLaunchPosition();
            Debug.DrawRay(launchPosition, Vector3.up * 2f, Color.red);
        }

        public Vector3 CalculateLaunchPosition()
        {
            var launcher = gameManager.GetLauncher(Id);
            if (launcher == null)
            {
                return Vector3.zero;
            }

            var origin = launcher.Spawn.position;
            origin += launcher.Spawn.forward * gameManager.Settings.LaunchDistance;

            if (Physics.Raycast(origin, Vector3.down, out var hit))
            {
                return hit.point;
            }
            else
            {
                return Vector3.zero;
            }
        }

        public void Disable()
        {
            if (!IsEnabled)
            {
                return;
            }

            IsEnabled = false;
            Controller.OnDisable(this);

            if (Robot != null)
            {
                EjectRobot();
                Robot = null;
            }
        }

        public void FillHand()
        {
            while (Hand.CardCount < gameManager.Settings.MaxHandSize)
            {
                if (Deck.IsEmpty && gameManager.Settings.RepopulateEmptyDeck)
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

        public bool LaunchRobot(bool withPullback = false)
        {
            if (Robot == null)
            {
                return false;
            }

            var launcher = gameManager.GetLauncher(Id);
            if (launcher == null)
            {
                return false;
            }

            launcher.Launched += OnLaunch;
            if (withPullback)
            {
                launcher.LaunchWithPullback();
            }
            else
            {
                launcher.Launch();
            }
            return true;
        }

        private void OnLaunch()
        {
            var launcher = gameManager.GetLauncher(Id);
            if (launcher == null)
            {
                return;
            }

            launcher.Launched -= OnLaunch;

            var launchTarget = CalculateLaunchPosition();

            if (Robot != null)
            {
                gameManager.RegisterRobot(Robot);

                Robot.Launch(launchTarget);
                Robot = null;
            }

            events.RobotChanged.Emit(new RobotEventArgs
            {
                playerId = Id,
                robot = null
            });
        }

        public void ChangeRobot(Robot robotPrefab)
        {
            EjectRobot();

            robotPrefab.gameObject.SetActive(false);
            var instance = GameObject.Instantiate(robotPrefab);

            var launcher = gameManager.GetLauncher(Id);
            if (launcher != null)
            {
                instance.transform.SetParent(launcher.Spawn);
                instance.transform.position = launcher.Spawn.position;
                instance.transform.rotation = launcher.Spawn.rotation;
            }

            Robot = instance;
            Robot.name = robotPrefab.name;
            instance.SetOwner(Id);
            instance.gameObject.SetActive(true);

            events.RobotChanged.Emit(new RobotEventArgs
            {
                playerId = Id,
                robot = Robot
            });
        }

        public void EjectRobot()
        {
            if (Robot == null)
            {
                return;
            }

            Robot.Explode(false);
            Robot = null;

            events.RobotChanged.Emit(new RobotEventArgs
            {
                playerId = Id,
                robot = null
            });
        }

        public void EmptyHand()
        {
            Hand.Clear();
        }

        public void ClearScore()
        {
            Score = 0;
        }

        public void GivePoints(int points)
        {
            Score += points;
        }
    }
}