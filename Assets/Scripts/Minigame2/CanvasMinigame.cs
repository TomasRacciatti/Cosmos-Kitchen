using System;
using Managers;
using UnityEngine;

namespace Minigame2
{
    public abstract class CanvasMinigame : Minigame3
    {
        public override void StartMinigame()
        {
            GameManager.Player.SetInputActive(false);
            GameManager.Canvas.InvManager.gameObject.SetActive(false);
            base.StartMinigame();
        }

        public override void ExitMinigame()
        {
            GameManager.Player.SetInputActive(true);
            GameManager.Canvas.InvManager.gameObject.SetActive(true);
            base.ExitMinigame();
        }

        protected virtual void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Lose();
                ExitMinigame();
            }
        }
    }
}