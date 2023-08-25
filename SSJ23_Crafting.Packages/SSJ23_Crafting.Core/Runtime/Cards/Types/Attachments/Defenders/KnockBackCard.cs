using UnityEngine;

namespace SSJ23_Crafting
{
    [CreateAssetMenu(menuName = "Cards/Knock Back")]
    public class KnockBackCard : DefenderCard
    {
        [SerializeField] GameObject prefab;
        [SerializeField] Vector3 offset;
    }
}