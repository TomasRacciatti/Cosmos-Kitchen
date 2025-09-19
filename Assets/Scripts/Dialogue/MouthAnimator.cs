using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class MouthAnimator : MonoBehaviour
{
    [Header("Binding")]
    public NPCSpeaker npc;
    public Transform target;
    public GameObject visual;

    [Header("Y Scale Targets")]
    [Range(0f, 2f)] public float vowelY = 1.0f;  // Abierto
    [Range(0f, 2f)] public float gapY   = 0.3f;  // Descanzo
    [Range(0f, 2f)] public float restY  = 0.0f;  // Cerrado

    [Header("X Width Mapping")]
    [Range(0f, 1f)] public float minXFactor = 0.5f;

    [Header("Timings (seconds)")]
    public float upTime   = 0.03f;   // Abierto
    public float downTime = 0.06f;   // Descanzo
    public float endTime  = 0.08f;   // Fin

    Transform t;
    Vector3 baseScale;
    Coroutine animCor;

    void Awake()
    {
        if (!npc) npc = GetComponentInParent<NPCSpeaker>();
        t = target ? target : transform;
        if (!visual) visual = t.gameObject;

        baseScale = t.localScale;
        SetFromY(restY);
        SetVisual(false);

        if (npc)
        {
            npc.OnSpeakStart += HandleStart;
            npc.OnBlip       += HandleBlip;
            npc.OnSpeakEnd   += HandleEnd;
        }
    }

    void OnDestroy()
    {
        if (npc)
        {
            npc.OnSpeakStart -= HandleStart;
            npc.OnBlip       -= HandleBlip;
            npc.OnSpeakEnd   -= HandleEnd;
        }
    }

    // --- Event handlers ---
    void HandleStart(NPCSpeaker _)
    {
        SetVisual(true);
        StartAnimTo(gapY, 0.05f);
    }

    void HandleBlip(NPCSpeaker _)
    {
        StopAnim();
        animCor = StartCoroutine(Co_Bounce());
    }

    void HandleEnd(NPCSpeaker _)
    {
        StopAnim();
        animCor = StartCoroutine(Co_ScaleTo(restY, endTime, disableAtEnd: true));
    }

    // --- Animation coroutines ---
    IEnumerator Co_Bounce()
    {
        yield return Co_ScaleTo(vowelY, upTime, false);
        yield return Co_ScaleTo(gapY,   downTime, false);
        animCor = null;
    }

    IEnumerator Co_ScaleTo(float targetY, float duration, bool disableAtEnd)
    {
        float startY = t.localScale.y;
        float acc = 0f;
        duration = Mathf.Max(0.0001f, duration);

        while (acc < 1f)
        {
            acc += Time.unscaledDeltaTime / duration;
            float s = Mathf.SmoothStep(0f, 1f, acc);

            float y = Mathf.Lerp(startY, targetY, s);
            ApplyXY(y);

            yield return null;
        }

        ApplyXY(targetY);

        if (disableAtEnd) SetVisual(false);
    }

    // --- Helpers ---
    void StartAnimTo(float y, float dur)
    {
        StopAnim();
        animCor = StartCoroutine(Co_ScaleTo(y, dur, false));
    }

    void StopAnim()
    {
        if (animCor != null) StopCoroutine(animCor);
        animCor = null;
    }

    void SetFromY(float y)
    {
        ApplyXY(y);
    }

    void ApplyXY(float y)
    {
        float norm = (vowelY <= 0f) ? 0f : Mathf.Clamp01(y / vowelY);

        float xMin = baseScale.x * Mathf.Clamp01(minXFactor);
        float x    = Mathf.Lerp(xMin, baseScale.x, norm);

        t.localScale = new Vector3(x, y, baseScale.z);
    }

    void SetVisual(bool on)
    {
        if (visual) visual.SetActive(on);
        else
        {
            foreach (var r in GetComponentsInChildren<Renderer>()) r.enabled = on;
        }
    }
}





