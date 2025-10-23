using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace QuantumEntanglement
{
    public enum Axis
    {
        X = 0, Y = 1, Z = 2
    }

    public class QuantumEngtanglementManager : MonoBehaviour
    {
        [Header("Instructions")]
        public InstructionStep[] instructions;

        [Header("References")]
        public Renderer sourceRenderer;
        public Renderer targetRenderer;
        public Transform target, source;
        public Slider positionSlider, rotationSlider, scaleSlider;
        public TMP_Text positionValueTxt, rotationValueTxt, scaleValueTxt, instructionTitle, instructionDescription;
        public CanvasGroup instructionsCG, instructionsNextBtnCG, controllsCG, endSimulationCG, completedCG, informationCG;
        public Image[] positionAxisBtns, rotationAxisBtns, scaleAxisBtns;

        [Header("Events")]
        public UnityEvent OnLevelComplete;


        int instructionsIdx = 0;
        Vector3 initialTargetPosition, initialSourcePosition, initialRotation, initialScale;
        Axis positionAxis = Axis.X, rotationAxis = Axis.X, scaleAxis = Axis.X;


        private void Start()
        {
            initialTargetPosition = target.position;
            initialSourcePosition = source.position;
            initialRotation = target.rotation.eulerAngles;
            initialScale = target.localScale;

            Setup();
        }

        private void Setup()
        {
            StartCoroutine(SetupCoroutine());
        }

        private IEnumerator SetupCoroutine()
        {
            Fade(instructionsCG, 1, 0.5f);
            yield return new WaitForSeconds(0.5f);
            ShowInstructions();
        }

        public void OnPositionSliderValueChange(float value)
        {
            positionValueTxt.text = value.ToString("F1");

            Vector3 targetFinalPosition = initialTargetPosition;
            Vector3 sourceFinalPosition = initialSourcePosition; ;
            switch (positionAxis)
            {
                case Axis.X:
                    targetFinalPosition.x = initialTargetPosition.x + value;
                    sourceFinalPosition.x = initialSourcePosition.x + value;
                    break;
                case Axis.Y:
                    targetFinalPosition.y = initialTargetPosition.y + value;
                    sourceFinalPosition.y = initialSourcePosition.y + value;
                    break;
                case Axis.Z:
                    targetFinalPosition.z = initialTargetPosition.z + value;
                    sourceFinalPosition.z = initialSourcePosition.z + value;
                    break;
            }

            target.position = targetFinalPosition;
            source.position = sourceFinalPosition;
        }

        public void SetPositionAxis(int axis)
        {
            initialSourcePosition = source.position;
            initialTargetPosition = target.position;
            positionAxis = (Axis)axis;
            MarkAxisButtonSelected(positionAxisBtns, axis);

            switch (axis)
            {
                case 0:
                    positionSlider.SetValueWithoutNotify(initialSourcePosition.x + 1 - transform.position.x);
                    break;
                case 1:
                    positionSlider.SetValueWithoutNotify(initialSourcePosition.y - transform.position.y);
                    break;
                case 2:
                    positionSlider.SetValueWithoutNotify(initialSourcePosition.z - transform.position.z);
                    break;
            }
            positionValueTxt.text = positionSlider.value.ToString("F1");
            Debug.Log($"Position: {initialSourcePosition}, Slider Value: {positionSlider.value}");
        }

        public void OnRotationSliderValueChange(float value)
        {
            rotationValueTxt.text = value.ToString("F0");

            Vector3 finalRotation = initialRotation;
            switch (rotationAxis)
            {
                case Axis.X:
                    finalRotation.x = initialRotation.x + value;
                    break;
                case Axis.Y:
                    finalRotation.y = initialRotation.y + value;
                    break;
                case Axis.Z:
                    finalRotation.z = initialRotation.z + value;
                    break;
            }

            target.rotation = Quaternion.Euler(finalRotation);
            source.rotation = Quaternion.Euler(finalRotation);
        }

        public void SetRotationAxis(int axis)
        {
            initialRotation = source.rotation.eulerAngles;
            rotationAxis = (Axis)axis;
            MarkAxisButtonSelected(rotationAxisBtns, axis);

            switch (axis)
            {
                case 0:
                    rotationSlider.SetValueWithoutNotify(initialRotation.x);
                    break;
                case 1:
                    rotationSlider.SetValueWithoutNotify(initialRotation.y);
                    break;
                case 2:
                    rotationSlider.SetValueWithoutNotify(initialRotation.z);
                    break;
            }
            rotationValueTxt.text = rotationSlider.value.ToString("F0");
            Debug.Log($"Rotation: {initialRotation}, Slider Value: {rotationSlider.value}");
        }

        public void OnScaleSliderValueChange(float value)
        {
            scaleValueTxt.text = value.ToString("F1");

            Vector3 finalScale = initialScale;
            switch (scaleAxis)
            {
                case Axis.X:
                    finalScale.x = value;
                    break;
                case Axis.Y:
                    finalScale.y = value;
                    break;
                case Axis.Z:
                    finalScale.z = value;
                    break;
            }

            target.localScale = finalScale;
            source.localScale = finalScale;
        }

        public void SetScaleAxis(int axis)
        {
            initialScale = source.localScale;
            scaleAxis = (Axis)axis;
            MarkAxisButtonSelected(scaleAxisBtns, axis);

            switch (axis)
            {
                case 0:
                    scaleSlider.SetValueWithoutNotify(initialScale.x);
                    break;
                case 1:
                    scaleSlider.SetValueWithoutNotify(initialScale.y);
                    break;
                case 2:
                    scaleSlider.SetValueWithoutNotify(initialScale.z);
                    break;
            }
            scaleValueTxt.text = scaleSlider.value.ToString("F1");
            Debug.Log($"Scale: {initialScale}, Slider Value: {scaleSlider.value}");
        }

        public void SetColor(string hexCode)
        {
            Color color;
            if (ColorUtility.TryParseHtmlString(hexCode, out color))
            {
                sourceRenderer.material.color = color;

                sourceRenderer.material.EnableKeyword("_EMISSION");
                sourceRenderer.material.SetColor("_EmissionColor", color);

                targetRenderer.material.color = color;

                targetRenderer.material.EnableKeyword("_EMISSION");
                targetRenderer.material.SetColor("_EmissionColor", color);
            }
            else
            {
                Debug.LogError("Invalid hex code: " + hexCode);
            }
        }

        public void ShowInstructions()
        {
            StartCoroutine(ShowInstructionsCoroutine());
        }

        private IEnumerator ShowInstructionsCoroutine()
        {
            yield return StartCoroutine(PreInstructionsTasks());

            InstructionStep step = instructions[instructionsIdx];
            instructionTitle.text = step.title;
            TypewriterEffect.AnimateText(instructionDescription, step.description, step.duration);
            yield return new WaitForSeconds(step.duration);

            yield return StartCoroutine(PostInstructionsTasks());
            instructionsIdx++;
        }

        private IEnumerator PreInstructionsTasks()
        {
            instructionsNextBtnCG.blocksRaycasts = false;
            Fade(instructionsNextBtnCG, 0, 0.5f);
            yield return new WaitForSeconds(0.5f);
        }

        private IEnumerator PostInstructionsTasks()
        {
            if (instructionsIdx < instructions.Length - 1)
            {
                instructionsNextBtnCG.blocksRaycasts = true;
                Fade(instructionsNextBtnCG, 1, 0.5f);
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                controllsCG.blocksRaycasts = true;
                Fade(controllsCG, 1f, 0.5f);
                yield return new WaitForSeconds(5f);
                endSimulationCG.blocksRaycasts = true;
                Fade(endSimulationCG, 1f, 0.5f);
            }
        }

        public void MarkLevelCompleteBtnAction()
        {
            StartCoroutine(MarkLevelCompleteBtnActionCoroutine());
        }

        private IEnumerator MarkLevelCompleteBtnActionCoroutine()
        {
            endSimulationCG.blocksRaycasts = false;
            Fade(endSimulationCG, 0f, 0.5f);
            Fade(completedCG, 1f, 0.5f);

            yield return new WaitForSeconds(2f);

            OnLevelComplete.Invoke();
        }

        private void MarkAxisButtonSelected(Image[] images, int axis)
        {
            foreach (Image image in images)
                image.color = Color.white;

            images[axis].color = Color.green;
        }

        private void Fade(CanvasGroup cg, float alpha, float duration)
        {
            cg.DOFade(alpha, duration);
        }
    }
}