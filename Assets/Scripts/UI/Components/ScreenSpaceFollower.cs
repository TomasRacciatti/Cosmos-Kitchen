using Interfaces;
using Managers;
using UnityEngine;

namespace UI.Components
{
    [RequireComponent(typeof(RectTransform))]
    public class ScreenSpaceFollower : MonoBehaviour
    {
        [Tooltip("World object to follow. If empty, the registered player's HUD anchor (its body bone) is used.")]
        [SerializeField] private Transform target;
        [Tooltip("Point to follow, relative to the target in world space (m). E.g. from the hips up to the head.")]
        [SerializeField] private Vector3 worldOffset = new Vector3(0f, 0.8f, 0f);
        [Tooltip("Shift on screen from that point, in canvas units (x < 0 = left, y > 0 = up).")]
        [SerializeField] private Vector2 screenOffset = new Vector2(-120f, 60f);

        private RectTransform _rect;
        private RectTransform _parent;
        private Canvas _canvas;
        private Camera _camera;

        private void Awake()
        {
            _rect = (RectTransform)transform;
            _parent = (RectTransform)_rect.parent;
            _canvas = GetComponentInParent<Canvas>().rootCanvas;
        }
        
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
            if (!TryResolveReferences()) return;
            
            Vector3 screenPoint = _camera.WorldToScreenPoint(target.position + worldOffset);
            if (screenPoint.z <= 0f) return;

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
                if (target == null)
                {
                    IHudAnchor anchor = GameManager.Player.GetComponent<IHudAnchor>();
                    target = anchor != null ? anchor.HudAnchor : GameManager.Player.transform;
                }
                if (_camera == null) _camera = GameManager.Player.MainCamera;
            }
            return target != null && _camera != null;
        }
    }
}
