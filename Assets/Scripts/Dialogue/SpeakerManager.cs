using UnityEngine;
using System.Collections;

public class SpeakerManager : MonoBehaviour
{
    [Header("Shared Audio")]
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip[] vowelClips;

    private Coroutine speakCor;
    private NPCSpeaker activeNPC;

    public void Speak(string text, NPCSpeaker npc)
    {
        if (string.IsNullOrEmpty(text) || npc == null) return;
        if (!source || vowelClips == null || vowelClips.Length == 0) return;

        CancelSpeak();                     // cancel any previous speech
        activeNPC = npc;
        activeNPC.RaiseSpeakStart();

        speakCor = StartCoroutine(Co_Speak(text, activeNPC));
    }

    public void CancelSpeak()
    {
        if (speakCor != null)
        {
            StopCoroutine(speakCor);
            speakCor = null;
        }
        if (source) source.Stop();
        if (activeNPC != null)
        {
            activeNPC.RaiseSpeakEnd();
            activeNPC = null;
        }
    }

    private IEnumerator Co_Speak(string text, NPCSpeaker npc)
    {
        // Count letters only
        int letters = 0;
        foreach (char c in text) if (char.IsLetter(c)) letters++;

        // Ensure at least 1 blip if there's at least 1 letter
        int playCount = Mathf.CeilToInt(letters * Mathf.Clamp01(npc.ratio));
        if (letters > 0) playCount = Mathf.Max(1, playCount);

        float gap = Mathf.Max(0f, npc.gapSeconds);

        for (int i = 0; i < playCount; i++)
        {
            // audio
            var clip = vowelClips[Random.Range(0, vowelClips.Length)];
            float jitter = 1f + Random.Range(-npc.pitchJitter, npc.pitchJitter);
            source.pitch = Mathf.Clamp(npc.basePitch * jitter, 0.1f, 3f);
            source.PlayOneShot(clip);

            // event
            npc.RaiseBlip();

            yield return new WaitForSecondsRealtime(gap);
        }

        npc.RaiseSpeakEnd();
        if (activeNPC == npc) activeNPC = null;
        speakCor = null;
    }
}


