namespace SSJ23_Crafting
{
    /// <summary>
    /// A simple game event with no arguments.
    /// </summary>
    public class GameEvent
    {
        private event System.Action _event;

        public GameEvent() => _event = null;

        /// <summary>
        /// Emit the event to all registered callbacks.
        /// </summary>

        public void Emit() => _event?.Invoke();

        /// <summary>
        /// Remove all registered callbacks to the event.
        /// </summary>
        public void Clear() => _event = null;

        /// <summary>
        /// Register a callback to the event.
        /// </summary>
        public void Register(System.Action callback) => _event += callback;

        /// <summary>
        /// Unregister a callback to the event.
        /// </summary>
        public void Unregister(System.Action callback) => _event -= callback;
    }

    /// <summary>
    /// A simple game event with a single typed arguments.
    /// </summary>
    public class GameEvent<T>
    {
        private event System.Action<T> _event;

        public GameEvent() => _event = null;

        /// <summary>
        /// Emit the event to all registered callbacks.
        /// </summary>
        public void Emit(T data) => _event?.Invoke(data);

        /// <summary>
        /// Remove all registered callbacks to the event.
        /// </summary>
        public void Clear() => _event = null;

        /// <summary>
        /// Register a callback to the event.
        /// </summary>
        public void Register(System.Action<T> callback) => _event += callback;

        /// <summary>
        /// Unregister a callback to the event.
        /// </summary>
        public void Unregister(System.Action<T> callback) => _event -= callback;
    }
}