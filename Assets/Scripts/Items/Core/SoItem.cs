using System;
using Items.Tools;
using UnityEngine;
using Cooking;

namespace Items.Core
{
    [CreateAssetMenu(menuName = "ScriptableObject/Items/Item")]
    public class SoItem : ScriptableObject
    {
        [Header("Item")]
        [SerializeField] private string itemName;
        [SerializeField] private SoItemVisualSet  visualSet;
        [SerializeField, TextArea] private string description = "Item Description";
        [SerializeField, Min(1)] private int stack = 10;
        [Header("Item 3D")]
        [SerializeField] private Mesh mesh;
        [SerializeField] private Material[] materials;
        [Header("Item Crafts")]
        [SerializeField] private ItemTools[] tools;
        
        [Header("Processing Prerequisites")]
        [SerializeField] private ProcessingPrerequisite[] prerequisites;
        
        public string ItemName => itemName;
        public SoItemVisualSet VisualSet => visualSet;
        //public Sprite Image => image;
        public string Description => description;
        public int Stack => stack;
        public Mesh Mesh => mesh;
        public Material[] Materials => materials;
        public ItemTools[] Tools => tools;
        public ProcessingPrerequisite[] Prerequisites => prerequisites;
    }

    [Serializable]
    public struct ItemTools
    {
        public SoTool tool;
        public SoItem item;
    }
}
