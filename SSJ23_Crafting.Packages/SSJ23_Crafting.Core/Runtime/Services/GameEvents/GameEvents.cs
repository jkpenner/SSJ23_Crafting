namespace SSJ23_Crafting
{
    public class CardEventArgs
    {
        public PlayerId playerId;
        public CardData card;
    }

    public class ResourceEventArgs
    {
        public PlayerId playerId;
        public float totalValue;
        public float amount;
    }

    /// <summary>
    /// Used for dispatching global level game events. When events
    /// for an individual object is required, subscribe to the 
    /// local level event.
    /// </summary>
    public class GameEvents : Singleton<GameEvents>
    {
        public GameEvent<ResourceEventArgs> ResourceChanged { get; private set; } = new GameEvent<ResourceEventArgs>();        
        
        /// <summary>
        /// Invoked when the user interacts with the launch button.
        /// </summary>
        public GameEvent UILaunchPressed { get; private set; } = new GameEvent();

        /// <summary>
        /// Invoked when the user interacts with a specific card in their hand.
        /// </summary>
        public GameEvent<int> UIUseCard { get; private set; } = new GameEvent<int>();

        
        public GameEvent<CardEventArgs> CardUsed { get; private set; } = new GameEvent<CardEventArgs>();
        public GameEvent<CardEventArgs> CardDiscarded { get; private set; } = new GameEvent<CardEventArgs>();
        public GameEvent<CardEventArgs> CardDrawn { get; private set; } = new GameEvent<CardEventArgs>();



        public GameEvent ShowDiscard { get; private set; } = new GameEvent();
        public GameEvent HideDiscard { get; private set; } = new GameEvent();
    }
}