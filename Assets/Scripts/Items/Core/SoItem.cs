using UnityEngine;

namespace Items.Core
{
    [CreateAssetMenu(menuName = "ScriptableObject/Items/Item")]
    public class SoItem : ScriptableObject
    {
        [SerializeField] private string itemName;
        [SerializeField] private Sprite image;
        [SerializeField, TextArea] private string description = "Item Description";
        [SerializeField, Min(1)] private int stack = 10;
        [SerializeField] private Mesh mesh;
        [SerializeField] private Material[] materials;
        
        public string ItemName => itemName;
        public Sprite Image => image;
        public string Description => description;
        public int Stack => stack;
        public Mesh Mesh => mesh;
        public Material[] Materials => materials;
    }
}
