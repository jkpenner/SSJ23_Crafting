using System;
using UnityEngine;

namespace SSJ23_Crafting
{
    [CreateAssetMenu(menuName = "Cards/Spawn On Land")]
    public class SpawnOnLandCard : DamagerCard
    {
        [SerializeField] GameObject prefab;
        [SerializeField] Vector3 offset;

        public override void OnCardEnable()
        {
            Owner.Motor.OnGrounded += OnLandOnGround;
        }

        public override void OnCardDisable()
        {
            Owner.Motor.OnGrounded -= OnLandOnGround;
        }

        private void OnLandOnGround()
        {
            var gameObject = GameObject.Instantiate(
                prefab, 
                Owner.transform.position + offset, 
                Owner.transform.rotation
            );

            foreach(var hasOwner in gameObject.GetComponents<HasOwner>())
            {
                hasOwner.Owner = Owner;
            }
        }
    }
}