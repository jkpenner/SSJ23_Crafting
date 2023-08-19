using UnityEngine;

namespace SSJ23_Crafting
{
    public class Spinner : MonoBehaviour
    {
        public float speed = 1f;
        public Vector3 axis = Vector3.up;
        public Space space = Space.Self;


        public void Update()
        {
            transform.Rotate(axis * speed * Time.deltaTime, space);
        }
    }
}