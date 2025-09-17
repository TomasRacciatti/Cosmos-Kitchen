using System;
using UnityEngine;

namespace MiniGames
{
    public class MiniGamesUIManager : MonoBehaviour
    {
        [SerializeField] private MiniGameUI[] miniGameUIs;

        public MiniGameUI ActiveMiniGame(MiniGameType type)
        {
            MiniGameUI activeUI = null;

            foreach (var miniGameUI in miniGameUIs)
            {
                bool isActive = miniGameUI.Type == type;
                miniGameUI.gameObject.SetActive(isActive);

                if (isActive)
                    activeUI = miniGameUI;
            }

            return activeUI;
        }
    }
}
