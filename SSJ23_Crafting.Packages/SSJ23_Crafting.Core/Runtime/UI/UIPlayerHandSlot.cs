using UnityEngine;

namespace SSJ23_Crafting
{
    public class UIPlayerHandSlot : MonoBehaviour
    {
        public UICard Card { get; private set; }

        public void SetCard(UICard card)
        {
            var rt = GetComponent<RectTransform>();

            var position = RectTransformUtility.WorldToScreenPoint(
                GetComponentInParent<Canvas>().worldCamera,
                transform.position
            );

            Debug.Log(position);
            Card = card;
            Card.transform.SetParent(transform, false);
            Card.SetOrigin(position);
            Card.RestoreToOrigin();
            Card.gameObject.SetActive(true);
        }

        public UICard TakeCard()
        {
            var result = Card;
            Card = null;
            return result;
        }
    }
}