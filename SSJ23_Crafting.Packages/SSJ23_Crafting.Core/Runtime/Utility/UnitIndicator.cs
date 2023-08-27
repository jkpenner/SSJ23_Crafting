using UnityEngine;

namespace SSJ23_Crafting
{
    public class UnitIndicator : MonoBehaviour
    {

        [SerializeField] Color playerOneMainColor;
        [SerializeField] Color playerOneFillColor;
        [SerializeField] Color playerTwoMainColor;
        [SerializeField] Color playerTwoFillColor;

        private Material material;

        private Material Mat
        {
            get
            {
                if (material == null)
                {
                    material = GetComponent<MeshRenderer>().material;
                }
                return material;
            }
        }

        public void SetPercent(float percent)
        {
            Mat.SetFloat("_FillPercent", percent);
        }

        public void SetPlayer(PlayerId playerId)
        {
            if (playerId == PlayerId.One)
            {
                Mat.SetColor("_Color", playerOneMainColor);
                Mat.SetColor("_EmptyColor", playerOneFillColor);
            }
            else
            {
                Mat.SetColor("_Color", playerTwoMainColor);
                Mat.SetColor("_EmptyColor", playerTwoFillColor);
            }
        }
    }
}