using System;
using UnityEngine;

namespace SSJ23_Crafting
{
    [RequireComponent(typeof(Rigidbody))]
    public class RobotMotor : MonoBehaviour
    {
        public Robot Robot { get; set; }

        public bool IsGrounded => GroundCount > 0;
        public int GroundCount { get; private set; }
        public float MoveVelocity { get; set; }
        public float JumpVelocity { get; set; }
        public float KnockBackVelocity { get; set; }

        public Rigidbody Rigidbody {
            get {
                if (rigidbody == null)
                {
                    rigidbody = GetComponent<Rigidbody>();
                }
                return rigidbody;
            }
        }

        public event Action OnKnockBack;
        public event Action OnGrounded;
        public event Action OnLeftGround;
        public event Action OnJump;
        /// <summary> Normal of wall hit </summary>
        public event Action<Vector3> OnHitWall;
        public event Action<Robot> OnLandOnRobot;
        public event Action<Robot> OnHitRobot;

        private new Rigidbody rigidbody;

        private void Awake()
        {
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

        public void Move(Vector3 direction)
        {
            if (IsGrounded)
            {
                Rigidbody.velocity = direction * MoveVelocity;
            }
        }

        public void Jump()
        {
            GroundCount = 0;
            Rigidbody.velocity = new Vector3(
                Rigidbody.velocity.x,
                JumpVelocity,
                Rigidbody.velocity.z
            );
            OnJump?.Invoke();
        }

        public void Turn(float speed)
        {
            var rotation = Quaternion.AngleAxis(speed * Time.deltaTime, Vector3.up);
            Rigidbody.MoveRotation(rotation * Rigidbody.rotation);
        }

        public void Face(Vector3 forward)
        {
            var faceRotation = Quaternion.LookRotation(forward, Vector3.up);
            Rigidbody.MoveRotation(faceRotation);
        }

        public void KnockBack(Vector3 direction)
        {
            GroundCount = 0;
            Rigidbody.velocity = new Vector3(
                direction.x * KnockBackVelocity,
                JumpVelocity,
                direction.z * KnockBackVelocity
            );
            OnKnockBack?.Invoke();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                GroundCount += 1;

                if (GroundCount == 1)
                {
                    OnGrounded?.Invoke();
                }
            }

            if (collision.gameObject.CompareTag("Robot"))
            {
                var robot = collision.gameObject.GetComponent<RobotMotor>();
                var robotDot = Vector3.Dot(collision.GetContact(0).normal, Vector3.up);

                if (robotDot > 0.66f)
                {
                    OnLandOnRobot?.Invoke(robot.Robot);
                    Jump();
                }
                else if (robotDot < -0.66f)
                {
                    // Debug.Log("Another robot landed on our head");
                }
                else
                {
                    OnHitRobot?.Invoke(robot.Robot);
                }
            }

            if (collision.gameObject.CompareTag("Wall"))
            {
                OnHitWall?.Invoke(collision.GetContact(0).normal);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ground") && GroundCount > 0)
            {
                GroundCount -= 1;
                if (GroundCount <= 0)
                {
                    GroundCount = 0;
                    OnLeftGround?.Invoke();
                }
            }
        }
    }
}