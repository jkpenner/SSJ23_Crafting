using UnityEngine;

namespace SSJ23_Crafting
{
    public abstract class MoverCard : AttachmentCard
    {
        public override CardType CardType => CardType.Mover;
    }
}