using UnityEngine;

namespace SSJ23_Crafting
{
    [CreateAssetMenu(menuName = "Cards/Spawn On Land")]
    public class SpawnOnLandCard : DamagerCard
    {
        [SerializeField] GameObject prefab;
        [SerializeField] Vector3 offset;
    }
}