using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAudioSettingsStore
{
    float Master { get; set; }
    float Music  { get; set; }
    float Sfx    { get; set; }

    void Save();
}

public interface IAudioVolumeApplier
{
    void ApplyMaster(float linear01);
    void ApplyMusic(float linear01);
    void ApplySfx(float linear01);
    void ApplyAll(float master, float music, float sfx);
}
