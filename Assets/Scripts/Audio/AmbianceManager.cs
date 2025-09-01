using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbianceManager : MonoBehaviour
{
    [SerializeField] public AudioClip currentBackground;

    void Start()
    {
        AudioManager.instance.SetAmbiance(currentBackground);
    }
}
