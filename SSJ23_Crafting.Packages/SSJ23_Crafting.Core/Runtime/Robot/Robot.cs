using System;
using UnityEngine;

namespace SSJ23_Crafting
{
    public enum RobotState
    {
        Build,
        Launch,
        Battle,
        Dead,
    }

    public enum RotationMode
    {
        Turn,
        Face,
    }

    [RequireComponent(typeof(Rigidbody))]
    public class Robot : MonoBehaviour
    {
        [SerializeField] Stat maxHealth = new Stat(0f);
        [SerializeField] Stat moveSpeed = new Stat(0f);
        [SerializeField] Stat turnSpeed = new Stat(0f);
        [SerializeField] Stat jumpSpeed = new Stat(0f);

        [SerializeField] int health = 1;
        [SerializeField] AttachmentPoint[] attachmentPoints;
        [SerializeField] float launchDuration = 1f;
        [SerializeField] LayerMask groundMask;

        public PlayerId PlayerId { get; private set; }
        public RobotState State { get; private set; }
        public Rigidbody Rigidbody { get; private set; }

        public bool AllowMovement { get; set; } = false;
        public Vector3 MoveDirection { get; set; }

        public bool AllowRotation { get; set; } = false;
        public RotationMode RotationMode { get; set; }
        public Vector3 TurnDirection { get; set; }

        public Stat MoveSpeed => moveSpeed;
        public Stat TurnSpeed => turnSpeed;
        public Stat JumpSpeed => jumpSpeed;

        public int Health => health;
        public bool IsGrounded { get; private set; }

        private Vector3 launchStart;
        private Vector3 launchMiddle;
        private Vector3 launchTarget;
        private float launchPercent;



        public event Action LandedOnGround;
        public event Action<Robot> LandedOnRobot;
        public event Action<Robot> ImpactedRobot;

        public delegate void DamageEvent(Robot source, int damage);
        public event DamageEvent Damaged;
        public event DamageEvent Destroyed;

        public bool IsActionLocked { get; private set; }
        public CardData ActionLockOwner { get; private set; }


        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            Disable();
        }

        public void Enable()
        {
            Rigidbody.useGravity = true;
            Rigidbody.isKinematic = false;
            Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        public void Disable()
        {
            Rigidbody.useGravity = false;
            Rigidbody.isKinematic = true;
            Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }

        public bool SetActionLock(CardData obj)
        {
            if (IsActionLocked)
            {
                return false;
            }

            ActionLockOwner = obj;
            return true;
        }

        public void ReleaseActionLock(CardData card)
        {
            if (!IsActionLocked || card != ActionLockOwner)
            {
                return;
            }

            IsActionLocked = false;
            ActionLockOwner = null;
        }

        public void OnCollisionEnter(Collision collision)
        {
            Debug.Log($"{this.name} collided with {collision.collider.name}");

            if (collision.collider.CompareTag("Wall"))
            {
                var newForward = Vector3.Reflect(transform.forward, collision.GetContact(0).normal);
                transform.rotation = Quaternion.LookRotation(newForward, Vector3.up);
                return;
            }

            var robot = collision.collider.GetComponentInParent<Robot>();
            if (robot != null)
            {
                if (Vector3.Dot(Vector3.up, collision.GetContact(0).normal) > 0.65f)
                {
                    Debug.Log("Hit the top of another robot");
                    // Destroy(robot.gameObject);
                    LandedOnRobot?.Invoke(robot);
                }
                else
                {
                    Debug.Log("Hit side of another robot");
                    var newForward = Vector3.Reflect(transform.forward, collision.GetContact(0).normal);
                    transform.rotation = Quaternion.LookRotation(newForward, Vector3.up);
                    ImpactedRobot?.Invoke(robot);
                }
            }
        }

        public bool IsValidAttachment(AttachmentType attachmentType)
        {
            foreach (var attachmentPoint in attachmentPoints)
            {
                if (attachmentPoint.AttachmentType == attachmentType)
                {
                    return true;
                }
            }

            return false;
        }

        public void Attach(AttachmentCard attachment)
        {
            attachment.Owner = this;
            foreach (var attachmentPoint in attachmentPoints)
            {
                if (attachmentPoint.AttachmentType == attachment.AttachmentType)
                {
                    attachmentPoint.Attach(this, attachment);
                }
            }
        }

        public void Launch(Vector3 target)
        {
            launchStart = transform.position;
            launchTarget = target;

            launchMiddle = new Vector3(
                (launchTarget.x - launchStart.x) / 2f + launchStart.x,
                Mathf.Max(launchStart.y, launchTarget.y) + 4f,
                (launchTarget.z - launchStart.z) / 2f + launchStart.z
            );

            launchPercent = 0f;

            SetState(RobotState.Launch);
            transform.SetParent(null);
        }

        private void Update()
        {
            switch (State)
            {
                case RobotState.Launch:
                    UpdateLaunchState();
                    break;
                case RobotState.Battle:
                    UpdateBattleState();
                    break;
            }

            SetGrounded(
                Physics.Raycast(
                    transform.position + Vector3.up,
                    Vector3.down,
                    1.1f,
                    groundMask
                )
            );

            // Direction (from rotation)
            // MoveSpeed(+/-), TurnSpeed (+/-), Speed Modifier


            // Move (Move Direction, Move Speed)
            // Turn (Turn Direction, Turn Speed)
            // Jump (Jump Height, Jump Duration)
            // Dash (Dash Direction, Dash Speed, Dash Duration)
        }

        private void SetGrounded(bool isGrounded)
        {
            if (IsGrounded != isGrounded)
            {
                IsGrounded = isGrounded;
                if (IsGrounded)
                {
                    LandedOnGround?.Invoke();
                }
            }
        }

        public void Damage(Robot source, int damage)
        {
            if (health <= 0 || damage <= 0)
            {
                return;
            }

            var clampedDamage = Mathf.Clamp(damage, 0, health);
            health -= clampedDamage;

            Damaged?.Invoke(source, clampedDamage);

            if (health <= 0)
            {
                health = 0;
                Destroyed?.Invoke(source, clampedDamage);
                SetState(RobotState.Dead);
            }
        }

        private void UpdateLaunchState()
        {
            launchPercent += 1f / GameSettings.LaunchDuration * Time.deltaTime;
            if (launchPercent >= 1f)
            {
                launchPercent = 1f;
            }

            var a = Vector3.Lerp(launchStart, launchMiddle, launchPercent);
            var b = Vector3.Lerp(launchMiddle, launchTarget, launchPercent);
            Rigidbody.MovePosition(Vector3.Lerp(a, b, launchPercent));

            if (launchPercent >= 1f)
            {
                SetState(RobotState.Battle);
            }
        }

        private void UpdateBattleState()
        {
            if (AllowMovement)
            {
                var movement = MoveDirection * MoveSpeed.Value * Time.deltaTime;
                var nextPosition = transform.position + movement;
                Rigidbody.MovePosition(nextPosition);
            }

            if (AllowRotation)
            {
                switch (RotationMode)
                {
                    case RotationMode.Turn:
                        var rotation = Quaternion.AngleAxis(TurnSpeed.Value * Time.deltaTime, Vector3.up);
                        Rigidbody.MoveRotation(rotation * Rigidbody.rotation);
                        break;
                    case RotationMode.Face:
                        var faceRotation = Quaternion.LookRotation(TurnDirection, Vector3.up);
                        Rigidbody.MoveRotation(faceRotation);
                        break;
                }
            }

            foreach (var attachmentPoint in attachmentPoints)
            {
                attachmentPoint.UpdateAttachment(this);
            }
        }

        private void SetState(RobotState state)
        {
            if (State != state)
            {
                OnStateExit(State);
                State = state;
                OnStateEnter(State);
            }
        }

        private void OnStateExit(RobotState state)
        {
            switch (State)
            {
                case RobotState.Battle:
                    foreach (var attachmentPoint in attachmentPoints)
                    {
                        attachmentPoint.Disable(this);
                    }
                    break;
            }
        }

        private void OnStateEnter(RobotState state)
        {
            switch (State)
            {
                case RobotState.Launch:
                    Disable();
                    Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
                    break;
                case RobotState.Battle:
                    Enable();
                    foreach (var attachmentPoint in attachmentPoints)
                    {
                        attachmentPoint.Enable(this);
                    }
                    break;
                case RobotState.Dead:
                    Destroy(this.gameObject);
                    break;
            }
        }

        public void Jump()
        {
            IsGrounded = false;
            Rigidbody.velocity = new Vector3(
                Rigidbody.velocity.x,
                jumpSpeed.Value,
                Rigidbody.velocity.z
            );
        }
    }
}