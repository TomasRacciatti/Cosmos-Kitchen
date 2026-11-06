using Managers;
using UnityEngine;

namespace UI.Components
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasGroup))]
    public class ScreenSpaceFollower : MonoBehaviour
    {
        [Tooltip("World object to follow. If empty, the registered player is used.")]
        [SerializeField] private Transform target;
        [Tooltip("Point to follow, relative to the target in world space (m). E.g. the head.")]
        [SerializeField] private Vector3 worldOffset = new Vector3(0f, 1.4f, 0f);
        [Tooltip("Shift on screen from that point, in canvas units (x < 0 = left, y > 0 = up).")]
        [SerializeField] private Vector2 screenOffset = new Vector2(-120f, 60f);

        private RectTransform _rect;
        private RectTransform _parent;
        private Canvas _canvas;
        private CanvasGroup _group;
        private Camera _camera;

        private void Awake()
        {
            _rect = (RectTransform)transform;
            _parent = (RectTransform)_rect.parent;
            _canvas = GetComponentInParent<Canvas>().rootCanvas;
            _group = GetComponent<CanvasGroup>();
        }

        // Positioned right before the UI is drawn: by then the camera (Cinemachine, in LateUpdate) has
        // already moved this frame, so the element doesn't lag a frame behind the character.
        private void OnEnable()
        {
            Canvas.willRenderCanvases += UpdatePosition;
        }

        private void OnDisable()
        {
            Canvas.willRenderCanvases -= UpdatePosition;
        }

        private void UpdatePosition()
        {
            if (!TryResolveReferences())
            {
                _group.alpha = 0f;
                return;
            }

            Vector3 screenPoint = _camera.WorldToScreenPoint(target.position + worldOffset);
            bool inFrontOfCamera = screenPoint.z > 0f;
            _group.alpha = inFrontOfCamera ? 1f : 0f;
            if (!inFrontOfCamera) return;

            Camera uiCamera = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_parent, screenPoint, uiCamera, out Vector2 local))
            {
                _rect.localPosition = new Vector3(local.x + screenOffset.x, local.y + screenOffset.y, _rect.localPosition.z);
            }
        }

        private bool TryResolveReferences()
        {
            if (GameManager.Player != null)
            {
                if (target == null) target = GameManager.Player.transform;
                if (_camera == null) _camera = GameManager.Player.MainCamera;
            }
            return target != null && _camera != null;
        }
    }
}
