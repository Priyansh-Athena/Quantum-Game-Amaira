using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace QuantumSuperposition
{


    public class QuantumCubeManager : MonoBehaviour
    {
        [SerializeField] QuantumFace[] allFaces;
        [SerializeField] CanvasGroup instructionsCG, instructionsNextBtnCG, observationCG, completedCG;
        [SerializeField] TMP_Text instructionTitle, instructionDescription, observationTxt;
        [SerializeField] Button instructionsNextBtn;
        [SerializeField] LookAtOrbitalRotation cameraLook;

        QuantumFace selectedFace;

        private InstructionStep[] instructions = new InstructionStep[]
        {
        new InstructionStep()
        {
            title = "Quantum Superposition",
            description = "Welcome! This cube demonstrates quantum mechanics. A quantum system can exist in MULTIPLE STATES at once until it is observed. The hidden faces of this cube exist in a state of superposition.",
            duration = 5f
        },
        new InstructionStep()
        {
            title = "Before Observation",
            description = "Before you measure (click) a face, it exists in all possible states simultaneously. The face color is uncertain - it could be RED, BLUE, or GREEN. Notice the faces shimmering with mixed colors.",
            duration = 5f
        },
        new InstructionStep()
        {
            title = "Wave Function Collapse",
            description = "When you CLICK on a face, you force it to 'collapse' into ONE definite state. The superposition breaks, and the face reveals its actual color based on quantum probabilities. Red: 40% | Blue: 35% | Green: 25%",
            duration = 5f
        }
        };

        private TweenCallback OnDescriptionAnimationCompleted;
        private int observedCount = 0, instructionsIdx = 0;
        private bool canObserve = false;

        private void Start()
        {
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

        void Update()
        {
            if (canObserve && Input.GetMouseButtonDown(0))
            {
                RaycastSelectFace();
            }
        }

        private void RaycastSelectFace()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                QuantumFace face = hit.collider.GetComponent<QuantumFace>();
                if (face != null && !face.IsObserved())
                {
                    canObserve = false;
                    selectedFace = face;
                    StartCoroutine(FaceSelectedForObservation());
                }
            }
        }

        public void ShowInstructions()
        {
            StartCoroutine(ShowInstructionsCoroutine());
        }

        public void ObservationButtonAction()
        {
            StartCoroutine(ObservationButtonActionCoroutine());
        }

        private IEnumerator ObservationButtonActionCoroutine()
        {
            Fade(observationCG, 0f, 0.5f);
            yield return new WaitForSeconds(0.5f);

            selectedFace.Observe();
            observedCount++;

            if (observedCount == allFaces.Length)
            {
                canObserve = false;
                Completed();
            }
            else
                canObserve = true;
        }

        private IEnumerator FaceSelectedForObservation()
        {
            observationTxt.text = $"<u>{selectedFace.position}</u> face selected, Click on the Button below to collapse the wave function";
            Fade(observationCG, 1f, 0.5f);
            yield return new WaitForSeconds(0.5f);
        }

        private void Completed()
        {
            StartCoroutine(CompletedCoroutine());
        }

        private IEnumerator CompletedCoroutine()
        {
            Fade(instructionsCG, 0f, 0.5f);
            yield return new WaitForSeconds(0.5f);

            Fade(completedCG, 1f, 0.5f);
        }

        private IEnumerator ShowInstructionsCoroutine()
        {
            yield return StartCoroutine(PreInstructionsTasks());

            InstructionStep step = instructions[instructionsIdx];
            instructionTitle.text = step.title;
            TypewriterEffect.AnimateText(instructionDescription, step.description, step.duration, OnDescriptionAnimationCompleted);
            yield return new WaitForSeconds(step.duration);

            yield return StartCoroutine(PostInstructionsTasks());
            instructionsIdx++;
        }

        private IEnumerator PreInstructionsTasks()
        {
            if (instructionsIdx == 1)
            {
                ToggleQuantumFaces(true);
                cameraLook.enabled = true;
            }

            if (instructionsIdx == 2)
            {
                canObserve = true;
            }

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
        }

        private void ToggleQuantumFaces(bool action)
        {
            foreach (var qf in allFaces)
            {
                qf.enabled = action;
            }
        }

        private void Fade(CanvasGroup cg, float alpha, float duration)
        {
            cg.DOFade(alpha, duration);
        }
    }
}