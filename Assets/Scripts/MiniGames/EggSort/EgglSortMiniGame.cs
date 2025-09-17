using Managers;
using UnityEngine;

namespace MiniGames.EggSort
{
    public class EggSortMiniGame : MiniGame
    {
        private float _checkWinInterval = 0.1f; // Sirve como timeslice para que no este revisando si gano en cada frame
        private EggSortUI _ui;
        private float _nextCheck;
        
        protected override void EnterMiniGame()
        {
            base.EnterMiniGame();

            _ui = GameManager.Canvas.MiniGamesUI.ActiveMiniGame(miniGameType) as EggSortUI;
            
            if (_ui == null) // Pequeño failsafe para que salga del minigame sin crashear
            {
                Debug.LogError($"No UI found for {miniGameType?.name}. Did you assign EggSortUI to this MiniGameType in MiniGamesUI?");
                LeaveMiniGame();
                return;
            }

            // Provide an SFX delegate so UI/Balls don’t know AudioManager
            System.Action<AudioClip> playSfx = (clip) =>
            {
                if (clip != null) AudioSource?.PlayOneShot(clip);
            };

            _ui.gameObject.SetActive(true);
            _ui.Setup(playSfx);

            _nextCheck = Time.time + _checkWinInterval;
        }
    }
}