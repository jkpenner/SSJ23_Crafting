using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace SSJ23_Crafting
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] new Camera camera;
        [SerializeField] Canvas canvas;

        [SerializeField] InputActionAsset inputs;
        [SerializeField] string screenTapActionPath = "UI/ScreenTap";
        [SerializeField] string screenHoldActionPath = "UI/ScreenHold";
        [SerializeField] string screenPositionActionPath = "UI/ScreenPosition";

        private InputAction screenTapAction;
        private InputAction screenHoldAction;
        private InputAction screenPositionAction;

        public UIDragInput ActiveInput { get; private set; }
        public Vector3 ActiveOffset { get; private set; }

        private void OnEnable()
        {
            screenTapAction = inputs.FindAction(screenTapActionPath);
            screenTapAction.performed += OnScreenTap;

            screenHoldAction = inputs.FindAction(screenHoldActionPath);
            screenHoldAction.started += OnScreenPressStarted;
            screenHoldAction.canceled += OnScreenPressCanceled;

            screenPositionAction = inputs.FindAction(screenPositionActionPath);

            inputs.Enable();
        }

        private void OnDisable()
        {
            inputs.Disable();

            screenPositionAction = null;

            screenTapAction.performed -= OnScreenTap;
            screenHoldAction.started -= OnScreenPressStarted;
            screenHoldAction.canceled -= OnScreenPressCanceled;
            screenHoldAction = null;
        }

        private void OnScreenTap(InputAction.CallbackContext context)
        {
            if (ActiveInput != null)
            {
                if (ActiveInput.Zone != UIDragInput.DropZone.Cancel)
                {
                    ActiveInput.OnDragCanceled();
                }
                else
                {
                    ActiveInput.Activate();
                }
                ActiveInput = null;
                return;
            }

            if (!Raycast(out var input, out var result))
            {
                Debug.Log("Did not click on a drag input");
                return;
            }

            input.Activate();
        }

        private void OnScreenPressStarted(InputAction.CallbackContext context)
        {
            if (!Raycast(out var input, out var result))
            {
                Debug.Log("Did not click on a drag input");
                return;
            }

            ActiveInput = input;
            ActiveInput.OnDragStarted();

            var screenPosition = screenPositionAction.ReadValue<Vector2>();

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                ActiveInput.GetComponent<RectTransform>(),
                screenPosition,
                camera,
                out var worldPoint
            ))
            {
                ActiveOffset = ActiveInput.transform.position - worldPoint;
            }
        }

        private void OnScreenPressCanceled(InputAction.CallbackContext context)
        {
            if (ActiveInput == null)
            {
                return;
            }

            ActiveInput.OnDragCanceled();
            ActiveInput = null;
        }

        private void Update()
        {
            if (ActiveInput == null)
            {
                return;
            }

            var screenPosition = screenPositionAction.ReadValue<Vector2>();

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                ActiveInput.GetComponent<RectTransform>(),
                screenPosition,
                camera,
                out var worldPoint
            ))
            {
                ActiveInput.MovePosition(worldPoint + ActiveOffset);
            }
        }

        private bool Raycast(out UIDragInput outInput, out RaycastResult outResult)
        {
            var pointerData = new PointerEventData(EventSystem.current)
            {
                position = screenPositionAction.ReadValue<Vector2>()
            };

            var raycastResults = new List<RaycastResult>();

            EventSystem.current.RaycastAll(pointerData, raycastResults);

            foreach (var result in raycastResults)
            {
                if (!result.gameObject.TryGetComponent<UIDragInput>(out outInput))
                {
                    continue;
                }

                outResult = result;
                return true;
            }

            outInput = default;
            outResult = default;
            return false;
        }
    }
}