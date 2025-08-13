using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnpointSetter : MonoBehaviour
{
    public int _clientNumber;
    public string _initialAnimationState;

    private void Start()
    {
        if (ClientManager.instance != null)
        {
            ClientManager.instance.SetSpawnpoint(this);
        }
    }
}
