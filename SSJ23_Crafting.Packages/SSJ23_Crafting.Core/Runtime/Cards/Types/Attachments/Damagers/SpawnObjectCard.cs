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
    }
}