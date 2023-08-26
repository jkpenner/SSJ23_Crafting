using UnityEngine;

namespace SSJ23_Crafting
{
    public class Projectile : MonoBehaviour, HasOwner
    {
        [SerializeField] ParticleSystem particles;
        [SerializeField] AudioSource audioSource;
        [SerializeField] float moveSpeed = 10f;
        [SerializeField] int damageAmount = 1;
        [SerializeField] float lifetime = 5f;

        public Robot Owner { get; set; }

        public void Start()
        {
            Destroy(this.gameObject, lifetime);
        }

        public void Update()
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other)
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

            otherRobot.Damage(Owner, damageAmount);
        }

        private void Explode()
        {
            if (particles != null)
            {
                particles.Play();
                particles.transform.SetParent(null);
            }

            if (audioSource != null)
            {
                audioSource.Play();
            }
        }
    }
}