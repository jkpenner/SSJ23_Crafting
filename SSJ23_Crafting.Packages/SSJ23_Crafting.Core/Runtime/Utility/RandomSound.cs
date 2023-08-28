using UnityEngine;

namespace SSJ23_Crafting
{
    public class RandomSound : MonoBehaviour
    {
        [SerializeField] AudioSource source;
        [SerializeField] AudioClip[] clips;

        public void PlayRandom()
        {
            source.PlayOneShot(clips[Random.Range(0, clips.Length)]);
        }
    }
}