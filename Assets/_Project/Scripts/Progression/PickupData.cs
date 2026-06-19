using UnityEngine;

namespace ArcaneSurvival
{
    [CreateAssetMenu(menuName = "Arcane Survival/Pickups/Pickup Data")]
    public sealed class PickupData : ScriptableObject
    {
        public string PickupName;
        public PickupType Type;
        public Color VisualColor = Color.white;
        public float Value = 1f;
    }
}
