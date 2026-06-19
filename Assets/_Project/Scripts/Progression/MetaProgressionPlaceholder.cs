using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class MetaProgressionPlaceholder : MonoBehaviour
    {
        public int UnspentRunes { get; private set; }

        public void AddRunes(int amount)
        {
            UnspentRunes += Mathf.Max(0, amount);
        }
    }
}
