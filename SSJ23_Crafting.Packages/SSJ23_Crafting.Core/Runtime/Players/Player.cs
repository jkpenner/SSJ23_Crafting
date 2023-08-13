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

        public PlayerController Controller { get; private set; }

        private GameEvents events;

        public Player(PlayerId id, PlayerController controller)
        {
            Id = id;
            Controller = controller;
            Deck = new CardDeck();
            Hand = new CardHand();

            Resource = 0;
            Score = 0;

            events = GameEvents.FindOrCreateInstance();
        }

        public void UseCard(CardData card)
        {

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
    }
}