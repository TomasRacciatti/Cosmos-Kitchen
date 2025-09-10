using System;
using Interfaces;
using Items.Core;
using Managers;
using Regulators;
using UnityEngine;
using UnityEngine.Serialization;

namespace MiniGames
{
    public abstract class MiniGame : MonoBehaviour , IInteractable
    {
        [Header("RewardedItem")]
        [SerializeField] private SoItem rewardedItem;
        [SerializeField] private int rewardedAmount = 1;
        
        [SerializeField] private float cooldownTime = 10f;
        [SerializeField] private Transform drop;
        [SerializeField] protected float difficulty = 1f;
        [SerializeField] protected float scaleDifficulty = 0.1f;
        
        [Header("SFX")]
        [SerializeField] private AudioClip enterSound;
        [SerializeField] private AudioClip leaveSound;
        [SerializeField] private AudioClip winSound;
        [SerializeField] private AudioClip loseSound;
        
        //Privates
        private AudioSource _audioSource;
        private Cooldown _cooldown;
        protected bool IsActive = false;
        protected int Lives;

        protected virtual void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void Interact(GameObject interactableObject)
        {
            if (!_cooldown.IsReady) return;

            if (interactableObject == GameManager.Player.gameObject)
            {
                EnterMiniGame();
            }
        }

        protected virtual void EnterMiniGame()
        {
            //_audioSource?.PlayOneShot(enterSound);
            //GameManager.Canvas.gameObject.SetActive(false);
            Lives = 3;
            _cooldown.StartCooldown(cooldownTime);
            IsActive = true;
        }

        protected virtual void LeaveMiniGame()
        {
            //_audioSource?.PlayOneShot(leaveSound);
            IsActive = false;
        }

        protected virtual void WinMiniGame()
        {
            //_audioSource?.PlayOneShot(winSound);
            RewardPlayer();
        }

        protected virtual void LoseMiniGame()
        {
            //_audioSource?.PlayOneShot(loseSound);
        }

        private void RewardPlayer()
        {
            if (rewardedItem == null) return;
            
            GameObject item = ObjectPool.SpawnObject(PrefabsManager.ItemPrefabPickup, drop.position, drop.rotation, false);
            item.GetComponent<ItemPickup>().SetItemAmount(new ItemAmount(rewardedItem, rewardedAmount));
            item.SetActive(true);
        }
    }
}
