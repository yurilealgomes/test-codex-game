using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArcaneSurvival
{
    public sealed class PlayerVisualController : MonoBehaviour
    {
        private readonly List<Renderer> renderers = new List<Renderer>();
        private readonly List<Color> baseColors = new List<Color>();

        private void Awake()
        {
            Renderer rootRenderer = GetComponent<Renderer>();
            if (rootRenderer != null)
            {
                rootRenderer.enabled = false;
            }

            BuildWizardVisual();
        }

        private void OnEnable()
        {
            EventBus.PlayerLevelUp += HandleLevelUp;
        }

        private void OnDisable()
        {
            EventBus.PlayerLevelUp -= HandleLevelUp;
        }

        public void FlashDamage()
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(FlashRoutine(Color.red, 0.12f));
            }
        }

        private void HandleLevelUp(int level)
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(FlashRoutine(new Color(0.7f, 1f, 0.55f), 0.28f));
            }
        }

        private void BuildWizardVisual()
        {
            Transform visualRoot = new GameObject("Low Poly Wizard Visual").transform;
            visualRoot.SetParent(transform, false);
            visualRoot.localPosition = Vector3.zero;

            CreatePart("Robe", PrimitiveType.Capsule, visualRoot, new Vector3(0f, 0.72f, 0f), new Vector3(0.82f, 1.15f, 0.82f), new Color(0.16f, 0.12f, 0.36f));
            CreatePart("Head", PrimitiveType.Sphere, visualRoot, new Vector3(0f, 1.42f, 0f), new Vector3(0.46f, 0.42f, 0.46f), new Color(0.82f, 0.68f, 0.52f));
            CreatePart("Hat Brim", PrimitiveType.Cylinder, visualRoot, new Vector3(0f, 1.64f, 0f), new Vector3(0.76f, 0.08f, 0.76f), new Color(0.22f, 0.16f, 0.58f));
            CreatePart("Hat Crown", PrimitiveType.Cylinder, visualRoot, new Vector3(0f, 1.92f, 0f), new Vector3(0.42f, 0.58f, 0.42f), new Color(0.26f, 0.18f, 0.68f));

            Transform staffRoot = new GameObject("Staff Aim Root").transform;
            staffRoot.SetParent(visualRoot, false);
            staffRoot.localPosition = new Vector3(0.46f, 0.95f, 0.18f);
            GameObject staff = CreatePart("Staff", PrimitiveType.Cylinder, staffRoot, new Vector3(0f, 0.25f, 0.44f), new Vector3(0.08f, 0.7f, 0.08f), new Color(0.42f, 0.25f, 0.12f));
            staff.transform.localRotation = Quaternion.Euler(65f, 0f, 0f);
            CreatePart("Staff Tip", PrimitiveType.Sphere, staffRoot, new Vector3(0f, 0.73f, 0.88f), new Vector3(0.22f, 0.22f, 0.22f), new Color(0.35f, 0.8f, 1f));

            WizardStaffAim staffAim = GetComponent<WizardStaffAim>();
            if (staffAim == null)
            {
                staffAim = gameObject.AddComponent<WizardStaffAim>();
            }

            staffAim.SetStaff(staffRoot);
        }

        private GameObject CreatePart(string name, PrimitiveType primitive, Transform parent, Vector3 localPosition, Vector3 localScale, Color color)
        {
            GameObject part = GameObject.CreatePrimitive(primitive);
            part.name = name;
            part.transform.SetParent(parent, false);
            part.transform.localPosition = localPosition;
            part.transform.localScale = localScale;

            Collider collider = part.GetComponent<Collider>();
            if (collider != null)
            {
                Destroy(collider);
            }

            Renderer renderer = part.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = CreateMaterial(color);
                renderers.Add(renderer);
                baseColors.Add(color);
            }

            return part;
        }

        private static Material CreateMaterial(Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            Material material = new Material(shader);
            material.color = color;
            return material;
        }

        private IEnumerator FlashRoutine(Color flashColor, float duration)
        {
            for (int i = 0; i < renderers.Count; i++)
            {
                renderers[i].material.color = flashColor;
            }

            yield return new WaitForSecondsRealtime(duration);

            for (int i = 0; i < renderers.Count && i < baseColors.Count; i++)
            {
                renderers[i].material.color = baseColors[i];
            }
        }
    }
}
