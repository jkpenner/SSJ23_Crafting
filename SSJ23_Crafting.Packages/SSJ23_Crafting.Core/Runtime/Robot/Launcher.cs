using System;
using UnityEngine;

namespace SSJ23_Crafting
{
    public class Launcher : MonoBehaviour
    {
        public enum LauncherState
        {
            Idle,
            Pullback,
            Launch,
            Restore,
            Controlled
        }

        [SerializeField] float maxDistance;
        [SerializeField] Transform visual;
        [SerializeField] Transform spawn;
        [SerializeField] float restoreSpeed = 5f;
        [SerializeField] float launchSpeed = 10f;

        [SerializeField] RandomSound sounds;

        public LauncherState State { get; private set; }
        public Transform Spawn => spawn;
        public bool IsLaunching => State == LauncherState.Launch;

        public event Action Launched;

        public void SetLaunchPercent(float percent)
        {
            State = LauncherState.Controlled;
            visual.position = transform.position - transform.up * (maxDistance * percent);
        }

        public void Restore()
        {
            State = LauncherState.Restore;
        }

        public void Launch()
        {
            State = LauncherState.Launch;
            sounds?.PlayRandom();
        }

        public void LaunchWithPullback()
        {
            State = LauncherState.Pullback;
        }

        private void Update()
        {   
            if (State == LauncherState.Pullback)
            {
                if (PullbackLauncher(restoreSpeed))
                {
                    Debug.Log("Pullback complete");
                    Launch();
                }
            }
            else if (State == LauncherState.Launch)
            {
                if (MoveTowardsOrigin(launchSpeed))
                {
                    State = LauncherState.Idle;
                    Launched?.Invoke();
                }
            }
            else if (State == LauncherState.Restore)
            {
                if (MoveTowardsOrigin(restoreSpeed))
                {
                    State = LauncherState.Idle;
                }
            }
        }

        private bool PullbackLauncher(float speed)
        {
            var target = transform.position - transform.up * maxDistance;
            var toTarget = target - visual.position;
            var distance = toTarget.magnitude;
            var direction = toTarget.normalized;

            var movement = speed * Time.deltaTime;
            if (distance < movement)
            {
                visual.position = target;
                return true;
            }
            else
            {
                visual.position += direction * movement;
                return false;
            }
        }

        private bool MoveTowardsOrigin(float speed)
        {
            var toTarget = transform.position - visual.position;
            var distance = toTarget.magnitude;
            var direction = toTarget.normalized;

            var movement = speed * Time.deltaTime;
            if (distance < movement)
            {
                visual.position = transform.position;
                return true;
            }
            else
            {
                visual.position += direction * movement;
                return false;
            }
        }
    }
}