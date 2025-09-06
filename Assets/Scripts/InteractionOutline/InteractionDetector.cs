using System;
using Interfaces;
using UnityEngine;

namespace InteractionOutline
{
    [DefaultExecutionOrder(-10)] // necesitamos que esto corra antes o no va a funcar
    public class InteractionDetector : MonoBehaviour
    {
        [Header("Detection")]
        [SerializeField] private float _sphereRadius = 0.75f;
        [SerializeField] private float _detectionRange = 3f;
        [SerializeField] private LayerMask _interactableMask;
    
        [Header("Gating")]
        [SerializeField, Range(0, 1)] private float _directLookThreshold = 0.85f; // cerca de 1 es mirar directo, cerca
        // de 0 te da un changui de 90 grados
        [SerializeField] private float _offDelaySeconds = 0.1f; // Nos ayuda a evitar que titile si mira al borde
    
        private Camera _mainCamera;
        private IHighlightable _currentHighlight;
        private Collider _currentCollider;
        private float _lastValidTime;
    
        public IHighlightable CurrentHighlight => _currentHighlight;
        public Collider CurrentCollider => _currentCollider;

        private void Awake()
        {
            _mainCamera = GetComponent<Camera>();
        }

        private void OnDisable()
        {
            if (_currentHighlight != null)
            {
                _currentHighlight.DisableHighlight();
                _currentHighlight = null;
                _currentCollider = null;
            }
        }

        private void Update()
        {
            DetectAndToggle();
        }

        private void DetectAndToggle()
        {
            var ray = new Ray(_mainCamera.transform.position, _mainCamera.transform.forward);
            RaycastHit hit;
            
            IHighlightable newHighlight = null;
            Collider newCol = null;

            if (Physics.SphereCast(ray, _sphereRadius, out hit, _detectionRange, _interactableMask, QueryTriggerInteraction.Ignore))
            {
                newHighlight = hit.collider.GetComponentInParent<IHighlightable>();
                newCol = hit.collider;
                
                // Manejamos el direct look threshold
                if (newHighlight != null)
                {
                    Vector3 dir = (hit.point - _mainCamera.transform.position).normalized;
                    float dot = Vector3.Dot(_mainCamera.transform.forward, dir);
                    
                    if (dot < _directLookThreshold)
                    {
                        newHighlight = null;
                        newCol = null;
                    }
                }
            }

            // Si el target cambió, apagamos el highlight viejo y prendemos el nuevo
            if (newHighlight != _currentHighlight)
            {
                if (_currentHighlight != null)
                    _currentHighlight.DisableHighlight();

                if (newHighlight != null)
                {
                    newHighlight.EnableHighlight();
                    _lastValidTime = Time.time;
                }
                
                _currentHighlight = newHighlight;
                _currentCollider = newCol;
                return;
            }

            // Si el target es el mismo, reseteamos el timer
            // (cuando lo deje de ver, el timer corre para apagarse con delay)
            if (_currentHighlight != null)
            {
                if (newHighlight != null)
                {
                    _lastValidTime = Time.time;
                }
                else if (Time.time - _lastValidTime > _offDelaySeconds)
                {
                    _currentHighlight.DisableHighlight();
                    _currentHighlight = null;
                    _currentCollider = null;
                }
            }
        }
    }
}
