using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dialogue.Views
{
    public class InteractionPromptView : ServiceView
    {
        [SerializeField] private GameObject interactionPromptGO;

        private void Awake()
        {
            interactionPromptGO.SetActive(false);
        }

        protected override void Subscribe(bool on)
        {
            if (on)  
                dialogueService.InteractionPromptToggled += OnToggled;
            else     
                dialogueService.InteractionPromptToggled -= OnToggled;
        }
        
        void OnToggled(bool show) => interactionPromptGO?.SetActive(show);
    }
}
