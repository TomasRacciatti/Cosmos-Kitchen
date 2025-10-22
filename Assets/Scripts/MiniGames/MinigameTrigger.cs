using Interfaces;
using Items.Core;
using Managers;
using Regulators;
using UnityEngine;
using UnityEngine.Events;

namespace MiniGames
{
    public class MinigameTrigger : MonoBehaviour, IInteractable
    {
        [Header("RewardedItem")]
        [SerializeField] private SoItem rewardedItem;
        [SerializeField] private int rewardedAmount = 1;
        
        [Header("Refs")]
        [SerializeField] private float cooldownTime = 30f;
        [SerializeField] private Transform dropTransform;
        [SerializeField] private Transform interactionPoint;
        [SerializeField] private GameObject minigame;
        
        [Header("Actions Cooldown")]
        [SerializeField] private UnityEvent onStartCooldown;
        [SerializeField] private UnityEvent onFinishCooldown;

        public Transform InteractionPoint => interactionPoint ? interactionPoint : transform;
        private Transform DropTransform => dropTransform ? dropTransform : transform;
        private Minigame _currentMinigame;
        protected Cooldown Cooldown;
        
        public virtual void Interact(GameObject interactableObject)
        {
            if (!Cooldown.IsReady)
            {
                NotificationsManager.NewNotification("Minigame in Cooldown", PrefabsManager.NotificationLoseUI);
                return;
            }

            if (interactableObject == GameManager.Player.gameObject)
            {
                EnterMiniGame();
            }
        }

        public void EnableInteract()
        {
            
        }

        public void DisableInteract()
        {
            
        }
        
        private void EnterMiniGame()
        {
            GameObject spawnObject = Instantiate(minigame, transform.position, Quaternion.identity);
            _currentMinigame = spawnObject.GetComponent<Minigame>();
            _currentMinigame.StartMinigame();
            _currentMinigame.OnWin += WinMiniGame;
            _currentMinigame.OnLose += LoseMiniGame;
        }

        private void LeaveMiniGame()
        {
            if (_currentMinigame)
            {
                _currentMinigame.OnWin -= WinMiniGame;
                _currentMinigame.OnLose -= LoseMiniGame;
                _currentMinigame.ExitMinigame();
                Destroy(_currentMinigame.gameObject);
                _currentMinigame = null;
            }
            StartCooldown();
        }

        private void WinMiniGame(int quality)
        {
            NotificationsManager.NewNotification("You Win Minigame", PrefabsManager.NotificationWinUI);
            RewardPlayer(quality);
            LeaveMiniGame();
        }

        private void LoseMiniGame()
        {
            NotificationsManager.NewNotification("You Lose Minigame", PrefabsManager.NotificationLoseUI);
            LeaveMiniGame();
        }

        protected void StartCooldown()
        {
            Cooldown.StartCooldown(cooldownTime);
            onStartCooldown?.Invoke();
            Invoke(nameof(FinishCooldown), cooldownTime);
        }

        private void FinishCooldown()
        {
            onFinishCooldown?.Invoke();
        }

        protected void RewardPlayer(int quality)
        {
            if (rewardedItem == null)
            {
                Debug.LogWarning("No se asignó rewardedItem en el inspector!");
                return;
            }

            GameObject item = ObjectPool.SpawnObject(PrefabsManager.ItemPrefabPickup, DropTransform.position, DropTransform.rotation, false);
            item.GetComponent<ItemPickup>().SetItemAmount(new ItemAmount(rewardedItem, rewardedAmount));
            item.SetActive(true);
        }
    }
}
