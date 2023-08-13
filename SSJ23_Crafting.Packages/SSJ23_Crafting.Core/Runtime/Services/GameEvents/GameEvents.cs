namespace SSJ23_Crafting
{
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



    }
}