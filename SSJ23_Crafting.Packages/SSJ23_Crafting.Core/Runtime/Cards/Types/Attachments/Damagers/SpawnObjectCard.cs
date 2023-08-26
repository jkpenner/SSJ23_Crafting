using UnityEngine;

namespace SSJ23_Crafting
{
    [CreateAssetMenu(menuName = "Cards/Spawn Object")]
    public class SpawnObjectCard : DamagerCard
    {
        [SerializeField] GameObject prefab;
        [SerializeField] Vector3 offset;
        [SerializeField] float spawnInterval;
        [SerializeField] bool spawnWhileGrounded;

        private float counter = 0f;

        public override void OnCardUpdate()
        {
            counter += Time.deltaTime;
            if (counter < spawnInterval)
            {
                return;
            }

            if (spawnWhileGrounded && !Owner.Motor.IsGrounded)
            {
                return;
            }

            var transformedOffset = Owner.transform.TransformDirection(offset);

            counter = 0f;
            var gameObject = GameObject.Instantiate(
                prefab, 
                Owner.transform.position + transformedOffset, 
                Owner.transform.rotation
            );

            foreach(var hasOwner in gameObject.GetComponents<HasOwner>())
            {
                hasOwner.Owner = Owner;
            }
        }
    }
}