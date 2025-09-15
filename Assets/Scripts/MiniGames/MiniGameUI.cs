using UnityEngine;
using UnityEngine.Serialization;

namespace MiniGames
{
    public class MiniGameUI : MonoBehaviour
    {
        [SerializeField] private MiniGameType type;
        
        public MiniGameType Type => type;
    }
}