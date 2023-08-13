using UnityEngine;

namespace SSJ23_Crafting
{
    /// <summary>
    /// Logger that will display GameEvents through Debug.Log when emitted.
    /// </summary>
    public class GameEventLogger : MonoBehaviour
    {
        private GameEvents events;

        private void OnEnable()
        {
            events = GameEvents.FindOrCreateInstance();
            events.UILaunchPressed.Register(() => Log("Launch Button Pressed"));
            events.UIUseCard.Register((index) => Log($"Used Card at Index {index}"));
        }

        private void OnDisable()
        {
            
        }

        private void Log(string message)
        {
            Debug.Log($"[{nameof(GameEvent)}]: {message}");
        }


    }
}