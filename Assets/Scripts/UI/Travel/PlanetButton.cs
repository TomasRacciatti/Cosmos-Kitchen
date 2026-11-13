using System;
using Planets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Travel
{
    public class PlanetButton : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        private static readonly int IsLockedHash = Animator.StringToHash("IsLocked");
        private static readonly int IsCurrentHash = Animator.StringToHash("IsCurrent");

        [SerializeField] private SoPlanet planet;
        [SerializeField, Tooltip("Shown while this planet is selected.")]
        private GameObject popUp;

        [Header("Hover")]
        [SerializeField, Tooltip("Grows on hover. Defaults to this object. Don't let the state Animator animate its scale.")]
        private RectTransform scaleTarget;
        [SerializeField, Min(1f)] private float hoverScale = 1.12f;
        [SerializeField, Min(0.01f)] private float scaleDuration = 0.12f;

        [Header("Tint")]
        [SerializeField, Tooltip("Tinted while pressed or locked. Defaults to the Graphic on this object.")]
        private Graphic tintTarget;
        [SerializeField] private Color pressedTint = new(0.75f, 0.75f, 0.75f, 0.85f);
        [SerializeField, Tooltip("Placeholder until the blocked-planet animation exists. White = no tint.")]
        private Color lockedTint = new(0.45f, 0.45f, 0.45f, 1f);
        [SerializeField, Min(0f)] private float tintDuration = 0.08f;

        [Header("State animation (optional)")]
        [SerializeField, Tooltip("Gets the bool params IsLocked and IsCurrent, if the controller has them.")]
        private Animator stateAnimator;

        public SoPlanet Planet => planet;
        public bool IsLocked { get; private set; }
        public bool IsCurrent { get; private set; }

        public event Action<PlanetButton> Clicked;

        private bool _hovered;
        private bool _pressed;
        private bool _hasLockedParam;
        private bool _hasCurrentParam;
        private Vector3 _baseScale;
        private float _hoverBlend;

        private void Awake()
        {
            if (scaleTarget == null) scaleTarget = (RectTransform)transform;
            if (tintTarget == null) tintTarget = GetComponent<Graphic>();
            _baseScale = scaleTarget.localScale;
            if (planet == null) Debug.LogWarning($"[PlanetButton] {name}: no SoPlanet assigned.", this);
            if (popUp == null) Debug.LogWarning($"[PlanetButton] {name}: no PopUp assigned.", this);

            if (stateAnimator == null || stateAnimator.runtimeAnimatorController == null) return;
            foreach (var parameter in stateAnimator.parameters)
            {
                if (parameter.type != AnimatorControllerParameterType.Bool) continue;
                if (parameter.nameHash == IsLockedHash) _hasLockedParam = true;
                else if (parameter.nameHash == IsCurrentHash) _hasCurrentParam = true;
            }
        }

        private void OnDisable()
        {
            _hovered = false;
            _pressed = false;
            _hoverBlend = 0f;
            scaleTarget.localScale = _baseScale;
            ApplyTint(true);
        }

        private void Update()
        {
            var target = _hovered ? 1f : 0f;
            if (_hoverBlend == target) return;

            _hoverBlend = Mathf.MoveTowards(_hoverBlend, target, Time.unscaledDeltaTime / scaleDuration);
            var eased = Mathf.SmoothStep(0f, 1f, _hoverBlend);
            scaleTarget.localScale = _baseScale * Mathf.LerpUnclamped(1f, hoverScale, eased);
        }

        public void SetState(bool locked, bool current)
        {
            IsLocked = locked;
            IsCurrent = current;
            ApplyTint(true);

            if (_hasLockedParam) stateAnimator.SetBool(IsLockedHash, locked);
            if (_hasCurrentParam) stateAnimator.SetBool(IsCurrentHash, current);
        }

        public void SetSelected(bool selected)
        {
            if (popUp != null) popUp.SetActive(selected);
        }

        public void OnPointerEnter(PointerEventData eventData) => _hovered = true;

        public void OnPointerExit(PointerEventData eventData)
        {
            _hovered = false;
            SetPressed(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left) SetPressed(true);
        }

        public void OnPointerUp(PointerEventData eventData) => SetPressed(false);

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left) Clicked?.Invoke(this);
        }

        private void SetPressed(bool pressed)
        {
            if (_pressed == pressed) return;
            _pressed = pressed;
            ApplyTint(false);
        }

        private void ApplyTint(bool instant)
        {
            if (tintTarget == null) return;
            var tint = IsLocked ? lockedTint : Color.white;
            if (_pressed) tint *= pressedTint;
            tintTarget.CrossFadeColor(tint, instant ? 0f : tintDuration, true, true);
        }
    }
}
