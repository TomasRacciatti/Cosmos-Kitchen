using System;
using System.Collections.Generic;
using Items.Core;
using Items.Inventory;
using Managers;
using UnityEngine;

namespace Stations
{
    public class StationModel : MonoBehaviour
    {
        [SerializeField] private List<GameObject> meshes;
        [SerializeField] private InvSystem invSystem;
        
        private List<MeshFilter> meshFilters;
        private List<MeshRenderer> meshRenderers;

        private void Awake()
        {
            meshFilters = new List<MeshFilter>();
            meshRenderers = new List<MeshRenderer>();

            foreach (var go in meshes)
            {
                if (go == null) continue;
                
                var filter = go.GetComponent<MeshFilter>();
                var render = go.GetComponent<MeshRenderer>();

                if (filter != null) meshFilters.Add(filter);
                if (render != null) meshRenderers.Add(render);
            }
            
            invSystem.Subscribe(SetMesh);
        }
        
        private void SetMesh(int index, ItemAmount itemAmount)
        {
            if (index < 0 || index >= meshFilters.Count) return;

            var filter = meshFilters[index];
            var render = meshRenderers[index];

            if (itemAmount.IsEmpty)
            {
                filter.mesh = null;
                return;
            }
            
            if (itemAmount.SoItem.Mesh)
            {
                filter.mesh = itemAmount.SoItem.Mesh;
                if (itemAmount.SoItem.Materials is { Length: > 0 })
                {
                    render.materials = itemAmount.SoItem.Materials;
                }
            }
            else
            {
                filter.mesh = PrefabsManager.ItemMesh;
                render.materials = PrefabsManager.ItemMaterials;
            }
        }
    }
}