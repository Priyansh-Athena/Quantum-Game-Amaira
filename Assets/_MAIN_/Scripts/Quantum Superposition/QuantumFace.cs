using UnityEngine;

public enum FacePosition
{
    UP, DOWN, LEFT, RIGHT, FRONT, BACk
}

namespace QuantumSuperposition
{
    public class QuantumFace : MonoBehaviour
    {
        // Quantum properties
        public FacePosition position;
        private bool isObserved = false;
        private Color[] possibleColors = new Color[]
        {
        Color.red,
        Color.blue,
        Color.green
        };

        private float[] probabilities = new float[] { 0.4f, 0.35f, 0.25f };
        private Color collapseColor;

        private Material faceMaterial;
        private MeshRenderer meshRenderer;
        private float superpositionFlashSpeed = 2f;

        void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            faceMaterial = new Material(meshRenderer.material);
            meshRenderer.material = faceMaterial;

            // Start in superposition state
            ShowSuperposition();
        }

        void Update()
        {
            if (!isObserved)
            {
                // Animate superposition with color flickering
                float cycle = Mathf.Sin(Time.time * superpositionFlashSpeed) * 0.5f + 0.5f;
                Color mixedColor = Color.Lerp(possibleColors[0], possibleColors[1], cycle);
                Color color = Color.Lerp(mixedColor, possibleColors[2], Mathf.Sin(Time.time * superpositionFlashSpeed * 0.7f) * 0.5f + 0.5f);
                faceMaterial.color = color;
                faceMaterial.SetColor("_EmissionColor", color);
            }
        }

        void ShowSuperposition()
        {
            faceMaterial.color = new Color(0.6f, 0.6f, 0.6f); // Gray superposition color
        }

        public void Observe()
        {
            if (isObserved) return;

            isObserved = true;

            // Wave function collapse - randomly select color based on probabilities
            float randomValue = Random.value;
            float cumulativeProbability = 0f;

            foreach (int i in System.Linq.Enumerable.Range(0, possibleColors.Length))
            {
                cumulativeProbability += probabilities[i];
                if (randomValue <= cumulativeProbability)
                {
                    collapseColor = possibleColors[i];
                    break;
                }
            }

            // Smooth color transition
            StartCoroutine(AnimateColorCollapse());
        }

        private System.Collections.IEnumerator AnimateColorCollapse()
        {
            Color startColor = faceMaterial.color;
            float duration = 0.5f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                Color color = Color.Lerp(startColor, collapseColor, elapsed / duration);
                faceMaterial.color = color;
                faceMaterial.SetColor("_EmissionColor", color);
                yield return null;
            }

            faceMaterial.color = collapseColor;
        }

        public bool IsObserved() => isObserved;
        public Color GetColor() => isObserved ? collapseColor : faceMaterial.color;
    }
}