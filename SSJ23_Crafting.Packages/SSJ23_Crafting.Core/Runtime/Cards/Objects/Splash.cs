using UnityEngine;

namespace SSJ23_Crafting
{
    public class Splash : MonoBehaviour, HasOwner
    {
        [SerializeField] ParticleSystem particles;
        [SerializeField] AudioSource audioSource;
        [SerializeField] float destroyAfter = 4f;
        [SerializeField] float damageRadius = 5f;
        [SerializeField] int damageAmount = 1;
        [SerializeField] LayerMask damageLayerMask;

        public Robot Owner { get; set; }

        private void Start()
        {
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

                robot.Damage(Owner, damageAmount);
            }


            Destroy(this.gameObject, destroyAfter);
        }
    }
}