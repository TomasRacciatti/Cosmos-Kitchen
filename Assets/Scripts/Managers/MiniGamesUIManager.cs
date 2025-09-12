using MiniGames;
using UnityEngine;

namespace Managers
{
    public class MiniGamesUIManager : MonoBehaviour
    {
        [SerializeField] private SkillCheckUI skillCheckUI;

        public SkillCheckUI SkillCheckUI => skillCheckUI;
    }
}
