using UnityEngine;

namespace SSJ23_Crafting
{
    [CreateAssetMenu(menuName = "Cards/Ignore Collisions")]
    public class IgnoreCollisionCard : DefenderCard
    {
        [SerializeField] GameObject prefab;
        [SerializeField] Vector3 offset;
        [SerializeField] bool ignoreGroundCollisions;
        [SerializeField] bool ignoreJumpCollisions;
    }
}