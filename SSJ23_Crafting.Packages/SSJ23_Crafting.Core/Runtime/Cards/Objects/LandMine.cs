using System;
using System.Collections;
using UnityEngine;

namespace SSJ23_Crafting
{
    public class LandMine : MonoBehaviour, HasOwner
    {
        [SerializeField] GameObject visuals;
        [SerializeField] ParticleSystem particles;
        [SerializeField] AudioSource audioSource;
        [SerializeField] float destroyAfter = 4f;
        [SerializeField] float damageRadius = 5f;
        [SerializeField] int damageAmount = 1;
        [SerializeField] float lifetime = 4f;
        [SerializeField] LayerMask damageLayerMask;
        [SerializeField] float knockBackVelocity = 2f;

        public Robot Owner { get; set; }

        private float counter = 0f;
        private bool isExploded = false;

        private void OnEnable()
        {
            var events = GameEvents.FindOrCreateInstance();
            events.GameOver.Register(OnGameOver);
        }

        private void OnDisable()
        {
            if (GameEvents.TryGetInstance(out var events))
            {
                events.GameOver.Unregister(OnGameOver);
            }
        }

        private void Update()
        {
            counter += Time.deltaTime;
            if (counter > lifetime)
            {
                Explode();
            }
        }

        private void OnGameOver()
        {
            Explode();
        }

        public void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<Robot>(out var otherRobot))
            {
                return;
            }

            if (Owner != null && otherRobot.PlayerId == Owner.PlayerId)
            {
                return;
            }

            Explode();
        }

        private void Explode()
        {
            if (isExploded)
            {
                return;
            }

            isExploded = true;
            StartCoroutine(ExplodeCourtine());
        }

        private IEnumerator ExplodeCourtine()
        {
            if (visuals != null)
            {
                visuals.gameObject.SetActive(false);
            }

            if (particles != null)
            {
                particles.Play();
            }

            if (audioSource != null)
            {
                audioSource.Play();
            }

            var colliders = Physics.OverlapSphere(
                transform.position,
                damageRadius,
                damageLayerMask
            );

            foreach (var collider in colliders)
            {
                if (!collider.TryGetComponent<Robot>(out var robot))
                {
                    continue;
                }

                if (robot == Owner || robot.PlayerId == Owner.PlayerId)
                {
                    continue;
                }

                var toRobot = robot.transform.position - transform.position;
                toRobot.Normalize();

                robot.Damage(Owner, damageAmount);
                robot.Motor.JumpVelocity = knockBackVelocity;
                robot.Motor.KnockBackVelocity = knockBackVelocity;
                robot.Motor.KnockBack(toRobot);
            }

            // Wait for the particles and audio to be complete
            var wait = Mathf.Max(particles.main.duration, audioSource.clip.length);
            yield return new WaitForSeconds(wait);

            Destroy(this.gameObject);
        }
    }
}