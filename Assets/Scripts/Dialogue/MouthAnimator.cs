using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class MouthAnimator : MonoBehaviour
{
    [Header("Binding")]
    public NPCSpeaker npc;            // auto-found if empty
    public Transform target;          // scaled object (defaults to this.transform)
    public GameObject visual;         // shown/hidden; default = target.gameObject

    [Header("Y Scale Targets")]
    [Range(0f, 2f)] public float vowelY = 1.0f;  // on blip  -> (X,Y) ~ (1,1)
    [Range(0f, 2f)] public float gapY   = 0.3f;  // between blips
    [Range(0f, 2f)] public float restY  = 0.0f;  // after end -> disable (X,Y) -> (0.5,0)

    [Header("X Width Mapping")]
    [Tooltip("Minimum X width as a fraction of base X when the mouth is closed.")]
    [Range(0f, 1f)] public float minXFactor = 0.5f; // <= your requirement

    [Header("Timings (seconds)")]
    public float upTime   = 0.03f;   // to vowel
    public float downTime = 0.06f;   // back to gap
    public float endTime  = 0.08f;   // to rest then hide

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
        // Normalize Y against "fully open" (vowelY) to drive X
        float norm = (vowelY <= 0f) ? 0f : Mathf.Clamp01(y / vowelY);

        float xMin = baseScale.x * Mathf.Clamp01(minXFactor); // e.g., 0.5 * base X
        float x    = Mathf.Lerp(xMin, baseScale.x, norm);     // y=0 -> xMin, y=vowelY -> base X

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





