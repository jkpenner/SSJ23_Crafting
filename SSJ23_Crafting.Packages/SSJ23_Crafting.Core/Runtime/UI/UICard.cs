using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SSJ23_Crafting
{
    public class UICard : MonoBehaviour
    {
        public enum DragState
        {
            Idle,
            Drag,
            Restore,
            Release,
        }

        public enum DropZone
        {
            Use,
            Discard,
            Reset,
        }

        [SerializeField] InputActionAsset inputs;
        [SerializeField] float releaseRange = 50f;

        private new Camera camera;
        private GameEvents events;
        private DragState state;
        private Vector2 offset;
        private InputAction screenPressAction;
        private InputAction screenPositionAction;
        private Vector3 origin;
        private float restoreSpeed;
        private float restoreRate = 0.2f;
        
        private DropZone zone = DropZone.Reset;

        public Action<UICard> Used;
        public Action<UICard> Discarded;

        public CardData Card { get; private set; }

        private void OnEnable()
        {
            events = GameEvents.FindOrCreateInstance();

            screenPressAction = inputs.FindAction("UI/ScreenPress", true);
            screenPositionAction = inputs.FindAction("UI/ScreenPosition", true);

            screenPressAction.started += OnScreenPressStart;
            screenPressAction.canceled += OnScreenPressStop;

            inputs.Enable();

            // origin = transform.position;

            var parent = GetComponentInParent<Canvas>();
            if (parent is null)
            {
                Debug.LogError($"[{GetType().Name}]: Must be a child of a Canvas.");
                return;
            }

            camera = parent.worldCamera;
        }

        private void OnDisable()
        {
            events = null;

            inputs.Disable();
            screenPressAction.started -= OnScreenPressStart;
            screenPressAction.canceled -= OnScreenPressStop;
        }

        private void OnDrawGizmos()
        {
            var releaseOffset = Vector3.down * releaseRange;

            if (Application.isPlaying)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(transform.position, 20f);
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(origin, 20f);

                Gizmos.DrawLine(
                    origin + releaseOffset + Vector3.left * 200f,
                    origin + releaseOffset + Vector3.right * 200f
                );
                Gizmos.DrawLine(
                    origin - releaseOffset + Vector3.left * 200f,
                    origin - releaseOffset + Vector3.right * 200f
                );
            }
        }

        private void OnScreenPressStart(InputAction.CallbackContext context)
        {
            // Check if this card is under the cursor
            if (!TrySelfRaycast(out var result))
            {
                return;
            }

            var screenPosition = RectTransformUtility.WorldToScreenPoint(camera, transform.position);
            offset = new Vector2(transform.position.x, transform.position.y) - result.screenPosition;

            state = DragState.Drag;
        }

        private void OnScreenPressStop(InputAction.CallbackContext context)
        {
            // var distance = Vector3.Distance(origin, transform.position);
            if (Mathf.Abs(origin.y - transform.position.y) > releaseRange)
            {
                state = DragState.Release;
                if (Vector3.Dot(Vector3.up, transform.position - origin) >= 0f)
                {
                    Used?.Invoke(this);
                }
                else
                {
                    Discarded?.Invoke(this);
                }
            }
            else
            {
                RestoreToOrigin();
            }

            SetDropZone(DropZone.Reset);
        }

        private void Update()
        {
            switch (state)
            {
                case DragState.Idle:
                    // SetOrigin(transform.parent.position);
                    break;
                case DragState.Drag:
                    UpdateDragState();

                    if (Mathf.Abs(origin.y - transform.position.y) > releaseRange)
                    {
                        if (Vector3.Dot(Vector3.up, transform.position - origin) >= 0f)
                        {
                            SetDropZone(DropZone.Use);
                        }
                        else
                        {
                            SetDropZone(DropZone.Discard);
                        }
                    }
                    else
                    {
                        SetDropZone(DropZone.Reset);
                    }
                    break;
                case DragState.Restore:
                    UpdateRestoreState();
                    break;
            }
        }

        private void SetDropZone(DropZone newZone)
        {
            if (this.zone != newZone)
            {
                if (this.zone == DropZone.Discard)
                {
                    events.HideDiscard.Emit();
                }

                if (newZone == DropZone.Discard)
                {
                    events.ShowDiscard.Emit();
                }

                this.zone = newZone;
            }
        }

        private void UpdateDragState()
        {
            var screenPosition = screenPositionAction.ReadValue<Vector2>();
            transform.position = screenPosition + offset;
        }

        private void UpdateRestoreState()
        {
            var toOrigin = origin - transform.position;
            var distance = toOrigin.magnitude;
            var moveDistance = restoreSpeed * Time.deltaTime;

            if (distance < moveDistance)
            {
                transform.position = origin;
            }
            else
            {
                transform.position += toOrigin.normalized * moveDistance;
            }


            if (Vector3.Distance(transform.position, origin) < Mathf.Epsilon)
            {
                state = DragState.Idle;
            }
        }

        public void SetCard(CardData card)
        {
            Card = card;
        }

        public void SetOrigin(Vector3 origin)
        {
            this.origin = origin;
            Debug.Log($"Setting Origin to {origin}");
        }

        public void SnapToOrigin()
        {
            transform.position = origin;
        }

        public void RestoreToOrigin()
        {
            var distance = Vector3.Distance(origin, transform.position);
            restoreSpeed = distance / restoreRate;
            state = DragState.Restore;
        }

        /// <summary>
        /// Raycast from the current screen position and check if
        /// this card is under the cursor. Returns true if valid.
        /// </summary>
        private bool TrySelfRaycast(out RaycastResult result)
        {
            var pointerData = new PointerEventData(EventSystem.current)
            {
                position = screenPositionAction.ReadValue<Vector2>()
            };

            var raycastResults = new List<RaycastResult>();

            EventSystem.current.RaycastAll(pointerData, raycastResults);

            if (raycastResults.Count == 0 || raycastResults[0].gameObject != gameObject)
            {
                result = default;
                return false;
            }

            result = raycastResults[0];
            return true;
        }
    }
}