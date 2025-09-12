using System;
using Cinemachine;
using Managers;
using UnityEngine;

namespace Characters.Player
{
    public class CameraModifier : MonoBehaviour
    {
        public void SetCamera(CinemachineVirtualCamera newCamera)
        {
            GameManager.Player.SetCamera(newCamera);
        }

        public void FirstPersonCamera()
        {
            SetCamera(GameManager.Player.FirstPersonCamera);
        }
        
        public void ThirdPersonCamera()
        {
            SetCamera(GameManager.Player.ThirdPersonCamera);
        }
    }
}
