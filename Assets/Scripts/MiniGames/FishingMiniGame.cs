using Managers;
using UnityEngine;

namespace MiniGames
{
    public class FishingMiniGame : MiniGame
    {
        private FishingUI _fishingUI;
        
        protected override void EnterMiniGame()
        {
            base.EnterMiniGame();
            _fishingUI = GameManager.Canvas.MiniGamesUI.ActiveMiniGame(miniGameType) as FishingUI;
            
            if (_fishingUI == null)
            {
                Debug.LogError($"No se encontró una UI para el minijuego {miniGameType.name}");
                return;
            }
            
            _fishingUI.gameObject.SetActive(true);
            _fishingUI.OnFishEscape.AddListener(LoseMiniGame);
            _fishingUI.OnFishEscape.AddListener(LeaveMiniGame);
        }

        protected override void LeaveMiniGame()
        {
            base.LeaveMiniGame();
            _fishingUI.gameObject.SetActive(false);
        }
    }
}