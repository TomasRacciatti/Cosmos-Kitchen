using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioSettingsBootstrap : MonoBehaviour
{
    [SerializeField] private PlayerPrefAudioStore_SO store;
    [SerializeField] private AudioMixerApplier_SO applier;

    [SerializeField] private bool applyOnSceneLoaded = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        ApplyOnce();
        if (applyOnSceneLoaded)
            SceneManager.activeSceneChanged += (_, __) => ApplyOnce();
    }

    private void ApplyOnce()
    {
        applier.ApplyAll(store.Master, store.Music, store.Sfx);
    }
}
