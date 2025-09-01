using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSliders : MonoBehaviour
{
    public static AudioSliders instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        AudioManager.instance.SetSliders();
    }

    public Slider musicSlider;
    public Slider SFXSlider;
}
