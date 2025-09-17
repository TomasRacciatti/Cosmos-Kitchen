using System;
using Interfaces;
using Items.Core;
using Managers;
using Regulators;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace MiniGames
{
    public abstract class MiniGame : MonoBehaviour , IInteractable
    {
        [Header("RewardedItem")]
        [SerializeField] private SoItem rewardedItem;
        [SerializeField] private int rewardedAmount = 1;
        
        [SerializeField] private float cooldownTime = 30f;
        [SerializeField] private Transform dropTransform;
        [SerializeField] private Transform interactionPoint;
        [SerializeField] protected float difficulty = 1f;
        [SerializeField] protected float scaleDifficulty = 0.1f;
        [SerializeField] protected MiniGameType miniGameType;
        
        [Header("SFX")]
        [SerializeField] private AudioClip enterSound;
        [SerializeField] private AudioClip leaveSound;
        [SerializeField] private AudioClip winSound;
        [SerializeField] private AudioClip loseSound;
        [SerializeField] protected AudioClip correctSound;
        [SerializeField] protected AudioClip wrongSound;
        [SerializeField] private UnityEvent onCooldown;
        [SerializeField] private UnityEvent onFinishCooldown;
        public Transform InteractionPoint => interactionPoint ? interactionPoint : transform;
        private Transform DropTransform => dropTransform ? dropTransform : transform;
        
        //Privates
        protected AudioSource AudioSource;
        protected Cooldown Cooldown;
        protected bool IsActive = false;
        protected int Lives;

        protected virtual void Awake()
        {
            AudioSource = GetComponent<AudioSource>();
        }

        public virtual void Interact(GameObject interactableObject)
        {
            if (!Cooldown.IsReady) return;

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

        protected virtual void EnterMiniGame()
        {
            AudioSource?.PlayOneShot(enterSound);
            GameManager.Player.SetInputActive(false);
            GameManager.Canvas.InvManager.gameObject.SetActive(false);
            if(miniGameType != null) GameManager.Canvas.MiniGamesUI.ActiveMiniGame(miniGameType);
            Lives = 3;
            IsActive = true;
        }

        protected virtual void LeaveMiniGame()
        {
            GameManager.Player.SetInputActive(true);
            GameManager.Canvas.InvManager.gameObject.SetActive(true);
            IsActive = false;
            StartCooldown();
        }

        protected virtual void WinMiniGame()
        {
            AudioSource?.PlayOneShot(winSound);
            NotificationsManager.NewNotification("You Win Minigame", PrefabsManager.NotificationWinUI);
            RewardPlayer();
            LeaveMiniGame();
        }

        protected virtual void LoseMiniGame()
        {
            AudioSource?.PlayOneShot(loseSound);
            NotificationsManager.NewNotification("You Lose Minigame", PrefabsManager.NotificationLoseUI);
            LeaveMiniGame();
        }

        protected void StartCooldown()
        {
            Cooldown.StartCooldown(cooldownTime);
            onCooldown?.Invoke();
            Invoke(nameof(FinishCooldown), cooldownTime);
        }

        private void FinishCooldown()
        {
            onFinishCooldown?.Invoke();
        }

        protected void RewardPlayer()
        {
            if (rewardedItem == null)
            {
                Debug.LogWarning("No se asignó rewardedItem en el inspector!");
                return;
            }
            
            Debug.Log($"Dropping {rewardedItem.name} x{rewardedAmount}");
            GameObject item = ObjectPool.SpawnObject(PrefabsManager.ItemPrefabPickup, DropTransform.position, DropTransform.rotation, false);
            item.GetComponent<ItemPickup>().SetItemAmount(new ItemAmount(rewardedItem, rewardedAmount));
            item.SetActive(true);
        }
    }
}
