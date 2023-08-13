using UnityEngine;

namespace SSJ23_Crafting
{
    [CreateAssetMenu(menuName = "Cards/Movers/Turn Card")]
    public class TurnCard : MoverCard
    {
        public override AttachmentType AttachmentType => AttachmentType.MoverTurn;
    }
}