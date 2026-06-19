using System.Collections;
using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class BreakableObject : MonoBehaviour, IDamageable
    {
        public static readonly System.Collections.Generic.List<BreakableObject> ActiveBreakables = new System.Collections.Generic.List<BreakableObject>();

        private BreakableObjectData data;
        private Renderer cachedRenderer;
        private XPDropper xpDropper;
        private Color baseColor;
        private float currentHealth;
        private bool broken;

        public bool IsAlive { get { return !broken && currentHealth > 0f; } }
        public Transform Transform { get { return transform; } }

        private void Awake()
        {
            cachedRenderer = GetComponentInChildren<Renderer>();
            xpDropper = GetComponent<XPDropper>();
            if (xpDropper == null)
            {
                xpDropper = gameObject.AddComponent<XPDropper>();
            }
        }

        private void OnEnable()
        {
            if (!ActiveBreakables.Contains(this))
            {
                ActiveBreakables.Add(this);
            }
        }

        private void OnDisable()
        {
            ActiveBreakables.Remove(this);
        }

        public void Initialize(BreakableObjectData objectData, Color tint)
        {
            data = objectData;
            currentHealth = data != null ? data.MaxHealth : 20f;
            broken = false;
            gameObject.SetActive(true);

            baseColor = tint;
            if (cachedRenderer != null)
            {
                cachedRenderer.material.color = baseColor;
            }
        }

        public void BreakInstantly()
        {
            if (IsAlive)
            {
                Break();
            }
        }

        public void TakeDamage(DamageInfo damageInfo)
        {
            if (!IsAlive || damageInfo == null)
            {
                return;
            }

            currentHealth = Mathf.Max(0f, currentHealth - damageInfo.FinalDamage);
            if (gameObject.activeInHierarchy && cachedRenderer != null)
            {
                StopAllCoroutines();
                StartCoroutine(FlashRoutine());
            }

            if (currentHealth <= 0f)
            {
                Break();
            }
        }

        private void Break()
        {
            if (broken)
            {
                return;
            }

            broken = true;
            float xp = data != null ? data.XpDrop : 1f;
            xpDropper.Drop(xp, transform.position + Vector3.up * 0.4f);
            SkillEffect.SpawnVfx(transform.position + Vector3.up * 0.5f, baseColor, Vector3.one * 1.2f, 0.28f);
            gameObject.SetActive(false);
        }

        private IEnumerator FlashRoutine()
        {
            cachedRenderer.material.color = Color.white;
            yield return new WaitForSeconds(0.06f);
            cachedRenderer.material.color = baseColor;
        }
    }
}
