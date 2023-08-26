using System;
using System.Collections;
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
    [RequireComponent(typeof(RobotMotor))]
    public class Robot : MonoBehaviour
    {
        [SerializeField] Stat maxHealth = new Stat(0f);
        [SerializeField] Stat moveSpeed = new Stat(0f);
        [SerializeField] Stat turnSpeed = new Stat(0f);
        [SerializeField] Stat jumpSpeed = new Stat(0f);

        [SerializeField] int health = 1;

        [Header("Attachments")]
        [SerializeField] AttachmentSlot[] attachmentSlots;
        [SerializeField] AttachmentPoint[] attachmentPoints;

        [Header("Effects")]
        [SerializeField] ParticleSystem explodePrefab;
        [SerializeField] AudioSource audioSource;
        [SerializeField] AudioClip[] explosionClips;

        public PlayerId PlayerId { get; private set; }
        public RobotState State { get; private set; }
        public RobotMotor Motor { get; private set; }
        public Rigidbody Rigidbody { get; private set; }

        public Stat MoveSpeed => moveSpeed;
        public Stat TurnSpeed => turnSpeed;
        public Stat JumpSpeed => jumpSpeed;

        public int Health => health;

        private Vector3 launchStart;
        private Vector3 launchMiddle;
        private Vector3 launchTarget;
        private float launchPercent;

        public AttachmentSlot[] Slots => attachmentSlots;

        public event Action LandedOnGround;
        public event Action<Robot> LandedOnRobot;
        public event Action<Robot> ImpactedRobot;

        public delegate void DamageEvent(Robot source, int damage);
        public event DamageEvent Damaged;
        public event DamageEvent Destroyed;

        public bool IsActionLocked { get; private set; }
        public CardData ActionLockOwner { get; private set; }

        private GameManager gameManager;
        private GameEvents gameEvents;

        private void Awake()
        {
            gameManager = GameManager.FindOrCreateInstance();
            gameEvents = GameEvents.FindOrCreateInstance();

            Rigidbody = GetComponent<Rigidbody>();
            Motor = GetComponent<RobotMotor>();
            Motor.Robot = this;
            Disable();
        }

        public void Enable()
        {
            Motor.OnHitWall += OnHitWall;
            Motor.OnHitRobot += OnHitRobot;
            Motor.Enable();

            foreach (var slot in attachmentSlots)
            {
                if (slot.Card == null)
                {
                    continue;
                }

                slot.Card.OnCardEnable();
            }
        }



        public void Disable()
        {
            foreach (var slot in attachmentSlots)
            {
                if (slot.Card == null)
                {
                    continue;
                }

                slot.Card.OnCardDisable();
            }

            Motor.Disable();
            Motor.OnHitWall -= OnHitWall;
            Motor.OnHitRobot -= OnHitRobot;
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

        private void OnHitWall(Vector3 contactNormal)
        {
            var forward = Motor.transform.forward;
            var reflected = Vector3.Reflect(transform.forward, contactNormal);
            reflected.y = 0;
            reflected.Normalize();

            Debug.DrawRay(transform.position, forward, Color.blue, 1f, false);
            Debug.DrawRay(transform.position, reflected, Color.yellow, 1f, false);

            Motor.Face(reflected);
            Motor.JumpVelocity = 5f;
            Motor.KnockBackVelocity = 5f;
            Motor.KnockBack(contactNormal);
        }

        private void OnHitRobot(Robot robot)
        {
            if (robot.PlayerId != this.PlayerId)
            {
                robot.Damage(this, 1);
            }

            var normal = (Motor.transform.position - robot.transform.position).normalized;
            var forward = Motor.transform.forward;
            var reflected = Vector3.Reflect(transform.forward, normal);
            reflected.y = 0;
            reflected.Normalize();

            Motor.Face(reflected);
            Motor.JumpVelocity = 5f;
            Motor.KnockBackVelocity = 5f;
            Motor.KnockBack(normal);
        }

        public bool IsAttached(AttachmentCard attachment)
        {
            foreach (var slot in attachmentSlots)
            {
                if (slot.Card == attachment)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsValidAttachment(AttachmentCard card)
        {
            foreach (var slot in attachmentSlots)
            {
                if (slot.IsValidAttachment(card))
                {
                    return true;
                }
            }

            return false;
        }

        public bool Attach(AttachmentCard attachment)
        {
            if (!IsValidAttachment(attachment))
            {
                return false;
            }

            attachment.Owner = this;

            foreach (var slot in attachmentSlots)
            {
                if (!slot.IsValidAttachment(attachment))
                {
                    continue;
                }

                slot.Card = attachment;
            }

            attachment.OnCardAttach();
            gameEvents.CardAttached.Emit(new AttachmentEventArgs
            {
                playerId = PlayerId,
                attachment = attachment
            });
            return true;
        }

        public bool Detach(AttachmentCard attachment)
        {
            if (!IsAttached(attachment))
            {
                return false;
            }

            attachment.OnCardDetach();

            foreach (var slot in attachmentSlots)
            {
                if (slot.Card != attachment)
                {
                    continue;
                }

                slot.Card = null;
            }

            gameEvents.CardDetached.Emit(new AttachmentEventArgs
            {
                playerId = PlayerId,
                attachment = attachment
            });

            return true;
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

            if (health <= 0 && State != RobotState.Dead)
            {
                if (source.PlayerId != this.PlayerId)
                {
                    gameManager.GivePoint(source.PlayerId);
                }

                health = 0;
                Destroyed?.Invoke(source, clampedDamage);
                Explode();
            }
        }

        public void Explode(bool randomDelay = true)
        {
            if (State == RobotState.Dead)
            {
                return;
            }

            SetState(RobotState.Dead);
            gameManager.UnregisterRobot(this);
            StartCoroutine(ExplodeRoutine(randomDelay));
        }

        private IEnumerator ExplodeRoutine(bool randomDelay)
        {
            if (randomDelay)
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 0.5f));
            }

            if (explodePrefab != null)
            {
                var explosion = GameObject.Instantiate(
                    explodePrefab,
                    transform.position,
                    transform.rotation
                );
            }

            if (audioSource != null && explosionClips.Length > 0)
            {
                audioSource.PlayOneShot(explosionClips[UnityEngine.Random.Range(0, explosionClips.Length)]);
            }

            while (true)
            {
                transform.localScale -= Vector3.one * Time.deltaTime;
                if (transform.localScale.x < 0f)
                {
                    transform.localScale = Vector3.zero;
                    break;
                }

                yield return null;
            }

            Destroy(this.gameObject);
        }

        private void UpdateLaunchState()
        {
            launchPercent += 1f / gameManager.Settings.LaunchDuration * Time.deltaTime;
            if (launchPercent >= 1f)
            {
                launchPercent = 1f;
            }

            var a = Vector3.Lerp(launchStart, launchMiddle, launchPercent);
            var b = Vector3.Lerp(launchMiddle, launchTarget, launchPercent);
            Motor.Rigidbody.MovePosition(Vector3.Lerp(a, b, launchPercent));

            if (launchPercent >= 1f)
            {
                SetState(RobotState.Battle);
            }
        }

        private void UpdateBattleState()
        {
            foreach (var slot in attachmentSlots)
            {
                if (slot.Card == null)
                {
                    continue;
                }

                slot.Card.OnCardUpdate();
            }
        }

        private void SetState(RobotState state)
        {
            if (State != state)
            {
                State = state;
                OnStateEnter(State);
            }
        }

        private void OnStateEnter(RobotState state)
        {
            switch (State)
            {
                case RobotState.Battle:
                case RobotState.Launch:
                    Enable();
                    break;

                default:
                    Disable();
                    break;
            }
        }

        public void SetOwner(PlayerId id)
        {
            PlayerId = id;
        }
    }
}