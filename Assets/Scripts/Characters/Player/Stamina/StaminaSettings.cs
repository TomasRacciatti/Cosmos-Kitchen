using UnityEngine;

namespace Characters.Player.Stamina
{
    [CreateAssetMenu(fileName = "StaminaSettings", menuName = "ScriptableObject/Player/Stamina Settings")]
    public class StaminaSettings : ScriptableObject
    {
        [Tooltip("Maximum stamina.")]
        [Min(1f)] [SerializeField] private float maxStamina = 100f;

        [Header("Regeneration")]
        [Tooltip("Stamina regained per second while it's not being used.")]
        [Min(0f)] [SerializeField] private float regenRate = 25f;
        [Tooltip("Time after the last use before stamina starts regenerating (s).")]
        [Min(0f)] [SerializeField] private float regenDelay = 0.5f;

        [Header("Exhaustion")]
        [Tooltip("After running out, stamina can't be used again until it refills to this fraction of the max " +
                 "(1 = full, like BOTW).")]
        [Range(0f, 1f)] [SerializeField] private float recoverFraction = 1f;

        public float MaxStamina => maxStamina;
        public float RegenRate => regenRate;
        public float RegenDelay => regenDelay;
        public float RecoverFraction => recoverFraction;
    }
}
