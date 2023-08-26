using System;
using UnityEngine;

namespace SSJ23_Crafting
{
    public class UIDragInput : MonoBehaviour
    {
        [SerializeField] private float dragAngle = 0f;
        [SerializeField] private float activateDistance = 1f;
        [SerializeField] private float maxDragDistance = 0f;
        [SerializeField] private float restoreRate = 0.25f;

        [SerializeField] private bool allowNegativeDrag = true;

        public enum DragState
        {
            Idle,
            Drag,
            Restore,
            Release,
        }

        public enum DropZone
        {
            Cancel,
            ActionOne,
            ActionTwo
        }

        public DragState State { get; private set; }
        public DropZone Zone { get; private set; }
        public Vector3 Origin { get; private set; }

        public bool IsDraggable => State != DragState.Release;

        public event Action DragStarted;
        public event Action DragCanceled;
        public event Action DropZoneChanged;

        private float restoreSpeed;

        public void OnEnable()
        {
            Origin = transform.position;
        }

        public void OnDragStarted()
        {
            State = DragState.Drag;
            DragStarted?.Invoke();
        }

        public void OnDragCanceled()
        {
            DragCanceled?.Invoke();

            if (Zone == DropZone.Cancel)
            {
                RestoreToOrigin();
            }
        }

        public void Activate()
        {
            SetDropZone(DropZone.ActionOne);
            DragCanceled?.Invoke();
        }

        private void Update()
        {
            switch (State)
            {
                case DragState.Restore:
                    UpdateRestoreState();
                    break;
            }
        }

        private void UpdateRestoreState()
        {
            var toOrigin = Origin - transform.position;
            var distance = toOrigin.magnitude;
            var moveDistance = restoreSpeed * Time.deltaTime;

            if (distance < moveDistance)
            {
                State = DragState.Idle;
                SnapToOrigin();
            }
            else
            {
                transform.position += toOrigin.normalized * moveDistance;
            }
        }

        public void SetOrigin(Vector3 origin)
        {
            Origin = origin;
        }

        public void SnapToOrigin()
        {
            transform.position = Origin;
        }

        public void RestoreToOrigin()
        {
            var distance = Vector3.Distance(Origin, transform.position);
            restoreSpeed = distance / restoreRate;
            State = DragState.Restore;
        }

        private void SetDropZone(DropZone zone)
        {
            if (Zone == zone)
            {
                return;
            }

            Zone = zone;
            DropZoneChanged?.Invoke();
        }

        public void MovePosition(Vector3 worldPosition)
        {
            transform.position = worldPosition;


            if (maxDragDistance > 0f && Vector3.Distance(transform.position, Origin) > maxDragDistance)
            {
                transform.position = (transform.position - Origin).normalized * maxDragDistance + Origin;
            }

            if (!allowNegativeDrag && Vector3.Dot(GetDragDirection(), (transform.position - Origin).normalized) < 0f)
            {
                Debug.Log("Projecting Position");
                var projected = Vector3.Project(transform.position - Origin, Quaternion.AngleAxis(dragAngle + 90f, Vector3.forward) * Vector3.up);
                transform.position = Origin + projected;
            }

            if (Mathf.Abs(Origin.y - transform.position.y) > activateDistance)
            {
                if (Vector3.Dot(GetDragDirection(), (transform.position - Origin).normalized) >= 0f)
                {
                    SetDropZone(DropZone.ActionOne);
                }
                else
                {
                    SetDropZone(DropZone.ActionTwo);
                }
            }
            else
            {
                SetDropZone(DropZone.Cancel);
            }
        }

        public Vector3 GetDragDirection()
        {
            return Quaternion.AngleAxis(dragAngle, Vector3.forward) * Vector3.up;
        }

        public float GetDragPercent()
        {
            return Mathf.Clamp01(Vector3.Distance(transform.position, Origin) / activateDistance);
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(Origin, GetDragDirection());

            if (!Application.isPlaying)
            {
                return;
            }

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 1f);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(Origin, 1f);

            switch (Zone)
            {
                case DropZone.Cancel:
                    Gizmos.color = Color.red;
                    break;
                case DropZone.ActionOne:
                    Gizmos.color = Color.blue;
                    break;
                case DropZone.ActionTwo:
                    Gizmos.color = Color.yellow;
                    break;
            }

            Gizmos.DrawLine(transform.position, Origin);
        }
    }
}