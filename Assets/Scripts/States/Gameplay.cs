using UnityEngine;

namespace States
{
    public class Gameplay : MonoBehaviour
    {
        void Start()
        {
            gameObject.AddComponent<StateTransition>();

            var map = gameObject.AddComponent<Map>();
            map.levelId = 0;
        }
    }
}
