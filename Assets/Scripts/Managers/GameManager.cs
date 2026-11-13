using Characters.Player;
using Interfaces;
using Progress;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public static PlayerController Player { get; private set; }
        public static CanvasManager Canvas { get; private set; }
        public static NovaProgress Novas { get; private set; }
        public static ISceneLoader Scenes { get; private set; }
        public static InventoryCarryOver CarriedInventory { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            Novas = new NovaProgress();
            CarriedInventory = new InventoryCarryOver();
            Scenes ??= new SceneLoader();
            DontDestroyOnLoad(gameObject);
        }
        
        public static void RegisterPlayer(PlayerController player) => Player = player;
        public static void RegisterCanvas(CanvasManager canvas) => Canvas = canvas;

        public static void Pause()
        {
            MusicEvents.RequestMusicChange(MusicEvents.MusicType.Pause);
            Time.timeScale = 0f;
            
            if (AudioManager.instance != null && AudioManager.instance.MixerApplier != null)
            {
                AudioManager.instance.MixerApplier.MuteWorldSFX();
            }
        }
        
        public static void Resume()
        {
            Time.timeScale = 1f;
            MusicEvents.RequestMusicResume();
            
            if (AudioManager.instance != null && AudioManager.instance.MixerApplier != null)
            {
                AudioManager.instance.MixerApplier.UnmuteWorldSFX();
            }
        }
    }
}