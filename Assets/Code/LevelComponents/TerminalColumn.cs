using UnityEngine;

namespace Code.LevelComponents
{
    public class TerminalColumn : MonoBehaviour
    {
        private Material _material;

        void Awake()
        {
            _material = GetComponent<Renderer>().material;
        }

        public void SetCompletionWeight(float weight)
        {
            _material.SetFloat("_CompletionWeight", weight);
        }
    }
}