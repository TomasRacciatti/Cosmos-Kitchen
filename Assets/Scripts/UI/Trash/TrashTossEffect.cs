using UnityEngine;
using UnityEngine.UI;

namespace UI.Trash
{
    [RequireComponent(typeof(Image))]
    public class TrashTossEffect : MonoBehaviour
    {
        [Header("Timing")]
        [SerializeField, Tooltip("Random duration (seconds) between x and y.")]
        private Vector2 durationRange = new(0.4f, 0.55f);

        [Header("Randomness")]
        [SerializeField, Tooltip("Final scale relative to the item's size, random between x and y.")]
        private Vector2 endScaleRange = new(0.08f, 0.2f);
        [SerializeField, Tooltip("Total spin in degrees, random between x and y; the direction is random too.")]
        private Vector2 spinRange = new(25f, 110f);
        [SerializeField, Tooltip("Max sideways bulge of the path (UI units), random sign.")]
        private float maxArc = 40f;

        [Header("Look")]
        [SerializeField, Tooltip("Color the item fades toward as it sinks into the can.")]
        private Color depthTint = new(0.3f, 0.3f, 0.3f, 1f);

        [Header("Curves (x = normalized time)")]
        [SerializeField, Tooltip("0 = drop point, 1 = landing point.")]
        private AnimationCurve moveCurve = new(new Keyframe(0f, 0f, 0f, 2f), new Keyframe(1f, 1f, 0f, 0f));
        [SerializeField, Tooltip("1 = item's size, 0 = end scale. Going above 1 early reads as tossing the item up toward the camera.")]
        private AnimationCurve scaleCurve = new(new Keyframe(0f, 1f), new Keyframe(0.15f, 1.12f), new Keyframe(1f, 0f, -2f, 0f));
        [SerializeField, Tooltip("Alpha over time.")]
        private AnimationCurve fadeCurve = new(new Keyframe(0f, 1f), new Keyframe(0.45f, 1f), new Keyframe(1f, 0f));

        private Image _image;
        private RectTransform _rect;
        private bool _playing;
        private float _elapsed;
        private float _duration;
        private float _endScale;
        private float _spin;
        private float _startAngle;
        private Vector2 _from;
        private Vector2 _to;
        private Vector2 _arcOffset;

        public bool IsPlaying => _playing;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _rect = (RectTransform)transform;
            _image.raycastTarget = false;
            _image.preserveAspect = true;
        }

        private void OnDisable()
        {
            // The trash canvas closed mid-toss: drop the animation instead of resuming it next time.
            _playing = false;
            _image.enabled = false;
        }

        public void Play(Sprite sprite, Vector2 from, Vector2 to, Vector2 size)
        {
            gameObject.SetActive(true);
            _image.sprite = sprite;
            _image.enabled = true;
            _rect.sizeDelta = size;

            _from = from;
            _to = to;
            _elapsed = 0f;
            _duration = Mathf.Max(0.01f, Random.Range(durationRange.x, durationRange.y));
            _endScale = Random.Range(endScaleRange.x, endScaleRange.y);
            _spin = Random.Range(spinRange.x, spinRange.y) * RandomSign();
            _startAngle = Random.Range(-8f, 8f);

            Vector2 path = to - from;
            Vector2 side = path.sqrMagnitude > 0.0001f ? new Vector2(-path.y, path.x).normalized : Vector2.up;
            _arcOffset = side * Random.Range(-maxArc, maxArc);

            _playing = true;
            Apply(0f);
        }

        private void Update()
        {
            if (!_playing)
            {
                gameObject.SetActive(false);
                return;
            }

            _elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(_elapsed / _duration);
            Apply(t);

            if (t >= 1f)
            {
                _playing = false;
                gameObject.SetActive(false);
            }
        }

        private void Apply(float t)
        {
            float move = moveCurve.Evaluate(t);
            Vector2 position = Vector2.LerpUnclamped(_from, _to, move) + _arcOffset * Mathf.Sin(move * Mathf.PI);
            _rect.localPosition = position;

            _rect.localScale = Vector3.one * Mathf.LerpUnclamped(_endScale, 1f, scaleCurve.Evaluate(t));
            _rect.localRotation = Quaternion.Euler(0f, 0f, _startAngle + _spin * move);

            Color color = Color.Lerp(Color.white, depthTint, t * t);
            color.a = fadeCurve.Evaluate(t);
            _image.color = color;
        }

        private static float RandomSign() => Random.value < 0.5f ? -1f : 1f;
    }
}
