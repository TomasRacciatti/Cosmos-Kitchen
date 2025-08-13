using UnityEngine;
using UnityEngine.UI;

public class CuttingMinigame : MonoBehaviour, IMinigame
{
    public RectTransform needlePivot;
    public RectTransform needle;
    public RectTransform hitZone;
    public Slider progressSlider;
    public GameObject canvasRoot;

    [Header("Gameplay")]
    public float rotationSpeed = 150f;
    public float successThreshold = 20f;
    public float progressPerHit = 0.4f;

    private float currentAngle = 0f;
    private bool isPlaying = false;
    private System.Action onCompleteSuccess;
    private System.Action onCompleteFailure;
    
    [Header("SFX")]
    [SerializeField] AudioClip EnterSound;
    [SerializeField] AudioClip WinSound;
    [SerializeField] AudioClip HitSound;

    public void StartMinigame(System.Action onSuccess, System.Action onFailure)
    {
        AudioManager.instance.PlaySFX(EnterSound); 

        canvasRoot.SetActive(true);
        enabled = true;
        isPlaying = true;
        progressSlider.value = 0f;
        RandomizeHitZone();

        onCompleteSuccess = onSuccess;
        onCompleteFailure = onFailure;
    }

    void Update()
    {
        if (!isPlaying) return;

        RotateNeedle();

        if (Input.GetKeyDown(KeyCode.Space)) TryCutting();
        if (Input.GetKeyDown(KeyCode.Escape)) EscapeMinigame();
    }

    void RotateNeedle()
    {
        currentAngle += rotationSpeed * Time.deltaTime;
        currentAngle %= 360f;
        needlePivot.localRotation = Quaternion.Euler(0f, 0f, -currentAngle);
    }

    void TryCutting()
    {
        float hitAngle = Mathf.Atan2(hitZone.anchoredPosition.y, hitZone.anchoredPosition.x) * Mathf.Rad2Deg;
        float needleVisualAngle = -currentAngle % 360f;
        if (needleVisualAngle < 0f) needleVisualAngle += 360f;

        float targetAngle = Mathf.Atan2(hitZone.anchoredPosition.y, hitZone.anchoredPosition.x) * Mathf.Rad2Deg;
        targetAngle = (targetAngle + 360f) % 360f;

        targetAngle -= 90f;
        if (targetAngle < 0f) targetAngle += 360f;


        float diff = Mathf.Abs(Mathf.DeltaAngle(needleVisualAngle, targetAngle));
        if (diff <= successThreshold)
        {
            progressSlider.value += progressPerHit;
            AudioManager.instance.PlaySFX(HitSound);
            RandomizeHitZone();
            if (progressSlider.value >= 1f) ExitMinigame(true);
        }
        else
        {
            progressSlider.value = Mathf.Max(0f, progressSlider.value - progressPerHit * 0.5f);
        }
    }

    void ExitMinigame(bool success)
    {
        isPlaying = false;
        canvasRoot.SetActive(false);
        enabled = false;

        if (success)
        {
            onCompleteSuccess?.Invoke();
            AudioManager.instance.PlaySFX(WinSound); 
        }
        else onCompleteFailure?.Invoke();
    }

    private void EscapeMinigame()
    {
        ExitMinigame(false);
        InputManager._instance.StopAll();
        InputManager._instance.StopInput();
    }

    void RandomizeHitZone()
    {
        float radius = 265f;
        float angle = Random.Range(0f, 360f);
        float rad = angle * Mathf.Deg2Rad;
        Vector2 pos = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;
        hitZone.anchoredPosition = pos;
        hitZone.localEulerAngles = Vector3.zero;
    }
}
