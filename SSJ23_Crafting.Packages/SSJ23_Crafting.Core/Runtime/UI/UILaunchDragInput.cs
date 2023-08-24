using System;
using UnityEngine;

namespace SSJ23_Crafting
{
    [RequireComponent(typeof(UIDragInput))]
    public class UILaunchDragInput : MonoBehaviour
    {
        [SerializeField] PlayerId playerId;

        private GameManager gameManager;
        private UIDragInput dragInput;
        private bool isDragging = false;

        private void Awake()
        {
            gameManager = GameManager.FindOrCreateInstance();
            dragInput = GetComponent<UIDragInput>();
        }

        private void OnEnable()
        {
            dragInput.DragStarted += OnDragStarted;
            dragInput.DragCanceled += OnDragCanceled;
        }

        private void OnDisable()
        {
            dragInput.DragStarted -= OnDragStarted;
            dragInput.DragCanceled -= OnDragCanceled;
        }

        private void OnDragStarted()
        {
            isDragging = true;
        }

        private void OnDragCanceled()
        {
            isDragging = false;
            if (dragInput.Zone == UIDragInput.DropZone.ActionOne)
            {
                gameManager.LaunchRobot(playerId);
                dragInput.RestoreToOrigin();
            }

            var launcher = gameManager.GetLauncher(playerId);
            if (launcher != null && launcher.State == Launcher.LauncherState.Controlled)
            {
                launcher.Restore();
            }
        }

        private void Update()
        {
            if (isDragging)
            {
                var launcher = gameManager.GetLauncher(playerId);
                if (launcher != null)
                {
                    launcher.SetLaunchPercent(dragInput.GetDragPercent());
                }
            }
        }
    }
}