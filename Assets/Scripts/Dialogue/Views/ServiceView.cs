using System;
using UnityEngine;

namespace Dialogue.Views
{
    public abstract class ServiceView : MonoBehaviour
    {
        protected IDialogueService dialogueService;

        protected virtual void OnEnable()
        {
            dialogueService = DialogueManager.Instance;
            if (dialogueService != null)
                Subscribe(true);
        }

        protected virtual void OnDisable()
        {
            if (dialogueService != null)
                Subscribe(false);
            dialogueService = null;
        }

        protected abstract void Subscribe(bool on);
    }
}
