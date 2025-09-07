using System;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Serialization;

namespace Dialogue.Views
{
    public class NotificationView : ServiceView
    {
        [SerializeField] private GameObject notificationGO;
        [SerializeField] private TextMeshProUGUI notificationTMP;
        [SerializeField] private float duration = 4f;
        
        Coroutine _coroutine;


        private void Awake()
        {
            notificationGO.SetActive(false);
            notificationTMP.text = "";
        }

        protected override void Subscribe(bool on)
        {
            if (on)  
                dialogueService.Notified += OnNotified;
            else     
                dialogueService.Notified -= OnNotified;
        }
        
        void OnNotified(string msg)
        {
            if (_coroutine != null) 
                StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(Run(msg));
        }
        
        IEnumerator Run(string msg)
        {
            if (notificationTMP) 
                notificationTMP.text = msg ?? "Empty notification";
            
            notificationGO?.SetActive(true);
            
            yield return new WaitForSeconds(duration);
            
            notificationGO?.SetActive(false);
        }
    }
}
