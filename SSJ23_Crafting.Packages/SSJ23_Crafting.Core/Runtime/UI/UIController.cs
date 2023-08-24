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
        [SerializeField] string screenPressActionPath = "UI/ScreenPress";
        [SerializeField] string screenPositionActionPath = "UI/ScreenPosition";

        private InputAction screenPressAction;
        private InputAction screenPositionAction;

        public UIDragInput ActiveInput { get; private set; }
        public Vector3 ActiveOffset { get; private set; }

        private void OnEnable()
        {
            screenPressAction = inputs.FindAction(screenPressActionPath);
            screenPressAction.started += OnScreenPressStarted;
            screenPressAction.canceled += OnScreenPressCanceled;

            screenPositionAction = inputs.FindAction(screenPositionActionPath);

            inputs.Enable();
        }

        private void OnDisable()
        {
            inputs.Disable();

            screenPositionAction = null;

            screenPressAction.started -= OnScreenPressStarted;
            screenPressAction.canceled -= OnScreenPressCanceled;
            screenPressAction = null;
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