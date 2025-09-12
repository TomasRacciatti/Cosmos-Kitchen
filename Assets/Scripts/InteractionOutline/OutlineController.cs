using System.Collections.Generic;
using Interfaces;
using UnityEngine;

namespace InteractionOutline
{
    public class OutlineController : MonoBehaviour, IInteractable
    {
        [Header("Renderer Collection")] 
        [SerializeField] private bool _autoCollectRenderers = true; // Si esto es true, va a agarrar todos los mesh

        // renderers del objeto y los hijos, si es false
        // tenemos que settear el array manualmente
        [SerializeField] private Renderer[] _renderers; // Si _autoCollectRenderers es true, se llena solo con todos

        [Header("Shader property names")] 
        [SerializeField] private string _enabledProp = "_Enabled";

        private bool _isOn = false;
        private int _enabledPropId = -1; // Esto lo usamos para evitar string lookups del property name

        private MaterialPropertyBlock _mpb; // Esto nos evita usar renderer.material que puede ser pesado. Cambia cosas
        // en drawtime pero no modifica el material original.
        // Material = shared recipe.
        // MaterialPropertyBlock = temporary per-object seasoning sprinkled at render
        // time. No asset changes, no new materials.
        [SerializeField] private Transform interactionPoint;
        public Transform InteractionPoint => interactionPoint ? interactionPoint : transform;

        private void Awake()
        {
            _mpb = new MaterialPropertyBlock();
            if (!string.IsNullOrEmpty(_enabledProp))
                _enabledPropId = Shader.PropertyToID(_enabledProp); // Ahora podemos dejar de usar el string y solo usar el Id

            if (_autoCollectRenderers)
                CollectRenderers();
        }

        private void OnDisable()
        {
            if (_isOn)
            {
                _isOn = false;
                ApplyToAll();
            }
        }

        private void CollectRenderers()
        {
            var mesh = GetComponentsInChildren<MeshRenderer>(true);
            var skinned = GetComponentsInChildren<SkinnedMeshRenderer>(true); // no creo que tengamos skinned pero lo pongo por las dudas
        
            var rendList = new List<Renderer>(mesh.Length + skinned.Length);
            for (int i = 0; i < mesh.Length; i++)
                rendList.Add(mesh[i]);
            for (int i = 0; i < skinned.Length; i++)
                rendList.Add(skinned[i]);
        
            _renderers = rendList.ToArray();
        }

        private void ApplyToAll()
        {
            if (_renderers == null || _renderers.Length == 0) return;
            if (_enabledPropId < 0) return;

            float value = _isOn ? 1f : 0f;

            for (int i = 0; i < _renderers.Length; i++)
            {
                var rend = _renderers[i];
                if (!rend) continue;

                var mats = rend.sharedMaterials;
                int slots = (mats != null && mats.Length > 0) ? mats.Length : 1;

                for (int slot = 0; slot < slots; slot++)
                {
                    _mpb.Clear();
                    _mpb.SetFloat(_enabledPropId, value);
                    rend.SetPropertyBlock(_mpb, slot);
                }
            }
        }

        public void RefreshRenderers()
        {
            CollectRenderers();
            if (_isOn)
                ApplyToAll();
        }

        public void Interact(GameObject interactableObject)
        {
            
        }

        public void EnableInteract()
        {
            if (_isOn) return;
            _isOn = true;
            ApplyToAll();
        }

        public void DisableInteract()
        {
            if (!_isOn) return;
            _isOn = false;
            ApplyToAll();
        }
    }
}