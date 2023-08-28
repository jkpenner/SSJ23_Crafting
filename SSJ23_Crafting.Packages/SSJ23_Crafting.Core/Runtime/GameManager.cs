using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSJ23_Crafting
{
    public enum GameState
    {
        Initializing,
        MainMenu,
        Starting,
        GamePlay,
        GameOver,
        Credits,
    }

    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] GameSettings settings;

        [SerializeField] CardDeckData playerOneDeckData;
        [SerializeField] CardDeckData playerTwoDeckData;

        [SerializeField] Launcher playerOneLauncher;
        [SerializeField] Launcher playerTwoLauncher;

        [SerializeField] Transform playerOneRobotParent;
        [SerializeField] Transform playerTwoRobotParent;

        public GameSettings Settings => settings;

        public GameState State { get; private set; }
        public Player PlayerOne { get; private set; }
        public Player PlayerTwo { get; private set; }

        public bool IsStateChanging { get; private set; }
        public List<Robot> ActiveRobots { get; private set; } = new List<Robot>();

        private GameEvents events;

        private Dictionary<GameState, GameStateController> stateControllers = new Dictionary<GameState, GameStateController>();

        public override void Awake()
        {
            base.Awake();

            events = GameEvents.FindOrCreateInstance();

            PlayerOne = new Player(PlayerId.One, new UserInputController());
            PlayerOne.Deck.SetSource(playerOneDeckData);


            PlayerTwo = new Player(PlayerId.Two, new ComputerController());
            PlayerTwo.Deck.SetSource(playerTwoDeckData);
        }

        private void Start()
        {
            State = GameState.Initializing;

            // Initialize data here

            SetGameState(GameState.MainMenu);
        }

        public void RegisterGameStateController(GameState state, GameStateController controller)
        {
            stateControllers[state] = controller;
            if (State == state)
            {
                IsStateChanging = true;
                StartCoroutine(StateEnterRoutine(state));
            }
        }

        public void SetGameState(GameState nextState)
        {
            if (nextState != State && !IsStateChanging)
            {
                IsStateChanging = true;
                StartCoroutine(StateChangeRoutine(nextState, State));
            }
        }

        private IEnumerator StateChangeRoutine(GameState nextState, GameState prevState)
        {
            yield return StateExitRoutine(prevState);
            State = nextState;
            yield return StateEnterRoutine(nextState);

            IsStateChanging = false;
            events.GameStateChanged.Emit(nextState);
        }

        private IEnumerator StateExitRoutine(GameState state)
        {
            if (stateControllers.TryGetValue(state, out var stateController))
            {
                yield return stateController.OnExitState(state);
            }
        }

        private IEnumerator StateEnterRoutine(GameState state)
        {
            if (stateControllers.TryGetValue(state, out var stateController))
            {
                yield return stateController.OnEnterState(state);
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

        public Launcher GetLauncher(PlayerId playerId)
        {
            if (playerId == PlayerId.One)
            {
                return playerOneLauncher;
            }
            else
            {
                return playerTwoLauncher;
            }
        }

        public void RegisterRobot(Robot robot)
        {
            if (ActiveRobots.Contains(robot))
            {
                return;
            }

            ActiveRobots.Add(robot);
        }

        public void UnregisterRobot(Robot robot)
        {
            ActiveRobots.Remove(robot);
        }

        public void GivePoint(PlayerId playerId)
        {
            if (playerId == PlayerId.One)
            {
                PlayerOne.GivePoints(1);
            }
            else
            {
                PlayerTwo.GivePoints(1);
            }
        }
    }
}