using UnityEngine;

namespace States
{
    public class Gameplay : MonoBehaviour
    {
        void Start()
        {
            var transition = gameObject.AddComponent<StateTransition>();
            
            var gameBoard = new GameObject();
            gameBoard.transform.parent = gameObject.transform;
            
            var map = gameBoard.AddComponent<Map>();
            map.levelId = 0;
        }
    }
}
