using System.Collections;
using UnityEngine;

namespace SSJ23_Crafting
{
    public abstract class GameStateController : MonoBehaviour
    {
        protected GameManager gameManager;
        protected GameEvents gameEvents;

        public abstract GameState[] ControlledStates { get; }

        protected virtual void Awake()
        {
            gameManager = GameManager.FindOrCreateInstance();
            foreach(var state in ControlledStates)
            {
                gameManager.RegisterGameStateController(state, this);
            }
            gameEvents = GameEvents.FindOrCreateInstance();
        }

        public virtual IEnumerator OnEnterState(GameState state)
        {
            yield break;
        }

        public virtual IEnumerator OnExitState(GameState state)
        {
            yield break;
        }
    }
}