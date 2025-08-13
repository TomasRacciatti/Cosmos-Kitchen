using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class FishingMinigame : MonoBehaviour, IMinigame
{
    [SerializeField] GameObject _minigamePanel;
    [SerializeField] TextMeshProUGUI _minigameText;
    [SerializeField] TextMeshProUGUI _inventoryNotificationText;
    [SerializeField] Image _backgroundImage;
    [SerializeField] RectTransform fishTransform;
    [SerializeField] GameObject fishVisual;
    [SerializeField] float fishMoveSpeed = 50f;
    [SerializeField] float playerForce = 100f;
    [SerializeField] float catchDuration = 5f;
    [SerializeField] float allowedRadius = 100f;
    [SerializeField] float fishDirectionChangeInterval = 2f;

    public Collider fishingZone;
    public float biteTimeMin = 10f;
    public float biteTimeMax = 15f;
    public Color normalBackgroundColor = Color.blue;
    public Color bitingBackgroundColor = Color.green;
    public float notificationDisplayTime = 2f;

    [Header("Bubble Effect")]
    [SerializeField] GameObject bubblePrefab;
    [SerializeField] Transform[] bubbleSpawnPoints;
    [SerializeField] float bubbleDuration = 0.5f;
    [SerializeField] float bubbleScaleMultiplier = 1.3f;

    [Header("SFX")]
    [SerializeField] AudioClip[] bubbleSFX;
    [SerializeField] AudioClip fishReeling;
    [SerializeField] AudioClip fishCaught;
    [SerializeField] AudioClip fishingStart;

    [Header("Progress Bar")]
    [SerializeField] Image catchProgressBar;

    [Header("ANIMATIONS (DEBERIAMOS HACER UN EVENTO :/)")]
    [SerializeField] PlayerViewer playerViewer;

    private System.Action onCompleteSuccess;
    private System.Action onCompleteFailure;


    private bool isFishing = false;
    private bool isSuccess;
    private float biteTime;
    private bool fishBit = false;
    private Vector2 fishVelocity;
    private Vector2 fishDirection;
    private float fishDirectionTimer = 0f;
    private float catchTimer = 0f;
    private Vector2 centerPoint;
    private List<GameObject> bubblesSpawned;

    void Start()
    {
        bubblesSpawned = new List<GameObject>();
        if (_backgroundImage != null) _backgroundImage.color = normalBackgroundColor;
        if (_inventoryNotificationText != null) _inventoryNotificationText.gameObject.SetActive(false);
        if (_minigamePanel != null) _minigamePanel.SetActive(false);
        if (fishVisual != null) fishVisual.SetActive(false);
        if (catchProgressBar != null) catchProgressBar.fillAmount = 0f;
    }

    void Update()
    {
        if (isFishing)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) ExitMinigame();
        }

        if (isFishing && fishBit)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                fishVelocity += Vector2.up * playerForce;
                TriggerBubbleEffect(0);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                fishVelocity += Vector2.down * playerForce;
                TriggerBubbleEffect(1);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                fishVelocity += Vector2.left * playerForce;
                TriggerBubbleEffect(2);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                fishVelocity += Vector2.right * playerForce;
                TriggerBubbleEffect(3);
            }

            Vector2 currentDirection = fishDirection * fishMoveSpeed;
            fishVelocity += currentDirection * Time.deltaTime;

            fishTransform.anchoredPosition += fishVelocity * Time.deltaTime;

            if (fishVelocity.sqrMagnitude > 0.01f)
            {
                float angle = Mathf.Atan2(fishVelocity.y, fishVelocity.x) * Mathf.Rad2Deg;
                fishTransform.rotation = Quaternion.Euler(0, 0, angle - 90f);
            }

            if (fishTransform.anchoredPosition.magnitude > allowedRadius)
            {
                _minigameText.text = "The Fish escaped!";
                StartCoroutine(ShowMessageTemporarily("The Fish escaped!", 2f));
                AudioManager.instance.StopAllSFX();
                isSuccess = false;
                StopFishing();
            }
            else
            {
                catchTimer += Time.deltaTime;
                if (catchProgressBar != null)
                    catchProgressBar.fillAmount = catchTimer / catchDuration;

                if (catchTimer >= catchDuration)
                {
                    _minigameText.text = "";
                    playerViewer.SetTrigger("FishCaught");
                    AudioManager.instance.StopAllSFX();
                    AudioManager.instance.PlaySFX(fishCaught);
                    isSuccess = true;
                    StopFishing();
                }
            }

            fishDirectionTimer += Time.deltaTime;
            if (fishDirectionTimer >= fishDirectionChangeInterval)
            {
                fishDirection = Random.insideUnitCircle.normalized;
                fishDirectionTimer = 0f;
            }
        }
    }

    void FixedUpdate()
    {
        if (isFishing && fishBit)
        {
            Vector2 randomWiggle = Random.insideUnitCircle * 5f;
            fishVelocity += randomWiggle * Time.fixedDeltaTime;
        }
    }

    void TriggerBubbleEffect(int index)
    {
        if (bubbleSpawnPoints != null && index >= 0 && index < bubbleSpawnPoints.Length)
        {
            StartCoroutine(SpawnBubbleAt(bubbleSpawnPoints[index]));
        }
        if (bubbleSFX != null && bubbleSFX.Length > 0)
        {
            int randomIndex = Random.Range(0, bubbleSFX.Length);
            AudioManager.instance.PlaySFX(bubbleSFX[index]);
        }
    }

    IEnumerator SpawnBubbleAt(Transform spawnPoint)
    {
        GameObject bubble = Instantiate(bubblePrefab, spawnPoint.position, Quaternion.identity, spawnPoint.parent);
        bubblesSpawned.Add(bubble);
        Image bubbleImage = bubble.GetComponent<Image>();
        RectTransform rect = bubble.GetComponent<RectTransform>();
        float timer = 0f;
        Vector3 originalScale = rect.localScale;
        Vector3 targetScale = originalScale * bubbleScaleMultiplier;
        Color startColor = bubbleImage.color;

        while (timer < bubbleDuration)
        {
            float t = timer / bubbleDuration;
            rect.localScale = Vector3.Lerp(originalScale, targetScale, t);
            bubbleImage.color = new Color(startColor.r, startColor.g, startColor.b, 1 - t);
            timer += Time.deltaTime;
            yield return null;
        }

        bubblesSpawned.Remove(bubble);
        Destroy(bubble);
    }

    IEnumerator StartFishing()
    {
        isFishing = true;
        AudioManager.instance.PlaySFX(fishingStart);
        playerViewer.SetTrigger("FishingStart");

        _minigameText.text = "Fishing...";
        biteTime = Random.Range(biteTimeMin, biteTimeMax);
        yield return new WaitForSeconds(biteTime);

        AudioManager.instance.PlaySFX(fishReeling);
        playerViewer.Fishing(isFishing);
        fishBit = true;
        catchTimer = 0f;
        fishDirectionTimer = 0f;
        _minigameText.text = "Use WASD to keep the fish inside!";
        fishTransform.anchoredPosition = Vector2.zero;
        fishTransform.rotation = Quaternion.identity;
        centerPoint = Vector2.zero;
        fishVelocity = Vector2.zero;
        fishDirection = Random.insideUnitCircle.normalized;

        if (_backgroundImage != null) _backgroundImage.color = bitingBackgroundColor;
        if (fishVisual != null) fishVisual.SetActive(true);
        if (catchProgressBar != null) catchProgressBar.fillAmount = 0f;
    }

    void StopFishing()
    {
        isFishing = false;        
        fishBit = false;

        fishVelocity = Vector2.zero;
        fishTransform.anchoredPosition = Vector2.zero;
        fishTransform.rotation = Quaternion.identity;

        if (_backgroundImage != null) _backgroundImage.color = normalBackgroundColor;
        if (fishVisual != null) fishVisual.SetActive(false);
        if (catchProgressBar != null) catchProgressBar.fillAmount = 0f;
        StartCoroutine(FishingCooldown());
    }

    IEnumerator FishingCooldown()
    {
        playerViewer.Fishing(isFishing);
        yield return new WaitForSeconds(1);
        _minigamePanel.SetActive(false);
        if (isSuccess)
        {
            onCompleteSuccess();
        }else
            onCompleteFailure();
    }

    IEnumerator ShowMessageTemporarily(string message, float duration)
    {
        _minigameText.text = message;
        yield return new WaitForSeconds(duration);
        if (isFishing && fishBit && _minigameText.text == message)
        {
            _minigameText.text = "Use WASD to keep the fish inside!";
        }
    }

    IEnumerator ShowInventoryNotification(string message)
    {
        if (_inventoryNotificationText != null)
        {
            _inventoryNotificationText.text = message;
            _inventoryNotificationText.gameObject.SetActive(true);
            yield return new WaitForSeconds(notificationDisplayTime);
            _inventoryNotificationText.gameObject.SetActive(false);
        }
    }

    void OnDrawGizmos()
    {
        if (fishTransform != null)
        {
            Gizmos.color = Color.cyan;
            Vector3 position = fishTransform.transform.position;
            Gizmos.DrawWireSphere(position, allowedRadius);
        }
    }

    public void StartMinigame(System.Action onSuccess, System.Action onFailure)
    {
        StartCoroutine(StartFishing());
        _minigamePanel.SetActive(true);

        onCompleteSuccess = onSuccess;
        onCompleteFailure = onFailure;
    }

    private void ExitMinigame()
    {
        StopAllCoroutines();
        foreach (GameObject bubble in bubblesSpawned)
        {
            Destroy(bubble);
        }
        playerViewer.SetTrigger("Shutdown");
        isFishing = false;
        fishBit = false;

        fishVelocity = Vector2.zero;
        fishTransform.anchoredPosition = Vector2.zero;
        fishTransform.rotation = Quaternion.identity;

        if (_backgroundImage != null) _backgroundImage.color = normalBackgroundColor;
        if (fishVisual != null) fishVisual.SetActive(false);
        if (catchProgressBar != null) catchProgressBar.fillAmount = 0f;

        _minigamePanel.SetActive(false);
        onCompleteFailure();

        InputManager._instance.StopAll();
        InputManager._instance.StopInput();
    }
}