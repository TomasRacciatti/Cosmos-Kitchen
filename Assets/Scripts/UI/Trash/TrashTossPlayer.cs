using System.Collections.Generic;
using UnityEngine;

namespace UI.Trash
{
    public class TrashTossPlayer : MonoBehaviour
    {
        [SerializeField] private TrashDropZone dropZone;
        [SerializeField, Tooltip("The can's opening. Tossed items fly toward its center and are drawn inside it.")]
        private RectTransform effectsRoot;
        [SerializeField] private TrashTossEffect effectPrefab;
        [SerializeField, Tooltip("Random landing offset from the opening's center (UI units).")]
        private float landingJitter = 20f;
        [SerializeField, Min(0)] private int prewarmCount = 4;

        private readonly List<TrashTossEffect> _pool = new();
        private Canvas _rootCanvas;

        private void Awake()
        {
            _rootCanvas = effectsRoot.GetComponentInParent<Canvas>().rootCanvas;
            for (int i = 0; i < prewarmCount; i++)
                CreateEffect().gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            dropZone.ItemTrashed += Play;
        }

        private void OnDisable()
        {
            dropZone.ItemTrashed -= Play;
        }

        private void Play(TrashedItem trashed)
        {
            if (trashed.Sprite == null) return;

            Camera cam = _rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _rootCanvas.worldCamera;
            Vector2 half = trashed.ScreenSize * 0.5f;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(effectsRoot, trashed.ScreenPosition, cam, out Vector2 from);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(effectsRoot, trashed.ScreenPosition - half, cam, out Vector2 min);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(effectsRoot, trashed.ScreenPosition + half, cam, out Vector2 max);

            Vector2 to = effectsRoot.rect.center + Random.insideUnitCircle * landingJitter;

            TrashTossEffect effect = GetEffect();
            effect.transform.SetAsLastSibling();
            effect.Play(trashed.Sprite, from, to, max - min);
        }

        private TrashTossEffect GetEffect()
        {
            for (int i = 0; i < _pool.Count; i++)
            {
                if (!_pool[i].IsPlaying) return _pool[i];
            }
            return CreateEffect();
        }

        private TrashTossEffect CreateEffect()
        {
            TrashTossEffect effect = Instantiate(effectPrefab, effectsRoot, false);
            _pool.Add(effect);
            return effect;
        }
    }
}
