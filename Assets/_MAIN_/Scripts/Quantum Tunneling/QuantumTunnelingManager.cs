using DG.Tweening;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class QuantumTunnelingManager : MonoBehaviour
{
    [Header("Instructions")]
    public InstructionStep[] instructions;


    [Header("References")]
    public TMP_Text instructionTitle, instructionDescription, feedbackTxt;
    public CanvasGroup instructionsCG, instructionsNextBtnCG, informationCG, toggleTunnelingBtnCG, moveCarBtnCG, completedCG, tryagainCG, endSimulationCG, feedbackCG;
    public GameObject finishLine;
    public Rigidbody carRb;
    public Collider wallCollider;

    [Header("Settings")]
    [Range(10f, 50f)] public float carForce = 100f;

    [Header("Events")]
    public UnityEvent OnLevelComplete;


    int instructionsIdx = 0;
    Vector3 carInitialPosition;
    Quaternion carInitialRotation;


    private void Start()
    {
        carInitialPosition = carRb.transform.position;
        carInitialRotation = carRb.transform.rotation;
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
            toggleTunnelingBtnCG.blocksRaycasts = true;
            Fade(toggleTunnelingBtnCG, 1f, 0.5f);
            Fade(informationCG, 1f, 0.5f);
            finishLine.SetActive(true);
        }
    }

    public void TunnelingBtnAction()
    {
        StartCoroutine(TunnelingBtnActionCoroutine());
    }

    private IEnumerator TunnelingBtnActionCoroutine()
    {
        toggleTunnelingBtnCG.blocksRaycasts = false;
        Fade(toggleTunnelingBtnCG, 0f, 0.5f);
        yield return new WaitForSeconds(0.5f);

        moveCarBtnCG.blocksRaycasts = true;
        Fade(moveCarBtnCG, 1f, 0.5f);
    }

    public void MoveCarBtnAction()
    {
        StartCoroutine(MoveCarBtnActionCoroutine());
    }

    private IEnumerator MoveCarBtnActionCoroutine()
    {
        moveCarBtnCG.blocksRaycasts = false;
        Fade(moveCarBtnCG, 0f, 0.5f);

        float probablity = Random.Range(0f, 1f);

        if (probablity <= 0.3f)
        {
            feedbackTxt.text = "Quantum Tunneling happened";
            wallCollider.enabled = false;
        }
        else
        {
            feedbackTxt.text = "Quantum Tunneling didn't happen.";
            wallCollider.enabled = true;
        }

        yield return new WaitForSeconds(1f);

        carRb.AddForce(Vector3.left * carForce, ForceMode.Impulse);

        yield return new WaitForSeconds(1f);


        tryagainCG.blocksRaycasts = true;
        endSimulationCG.blocksRaycasts = true;
        Fade(tryagainCG, 1f, 0.5f);
        Fade(endSimulationCG, 1f, 0.5f);

        Fade(feedbackCG, 1f, 0.5f);
    }


    public void TryagainBtnAction()
    {
        StartCoroutine(TryagainBtnActionCoroutine());
    }

    private IEnumerator TryagainBtnActionCoroutine()
    {
        carRb.transform.position = carInitialPosition;
        carRb.transform.rotation = carInitialRotation;
        carRb.linearVelocity = Vector3.zero;
        carRb.angularVelocity = Vector3.zero;

        tryagainCG.blocksRaycasts = false;
        endSimulationCG.blocksRaycasts = false;
        Fade(tryagainCG, 0f, 0.5f);
        Fade(endSimulationCG, 0f, 0.5f);
        Fade(feedbackCG, 0f, 0.5f);

        yield return new WaitForSeconds(0.5f);

        toggleTunnelingBtnCG.blocksRaycasts = true;
        Fade(toggleTunnelingBtnCG, 1f, 0.5f);
    }

    public void EndSimulationBtnAction()
    {
        StartCoroutine(EndSimulationBtnActionCoroutine());
    }

    private IEnumerator EndSimulationBtnActionCoroutine()
    {
        tryagainCG.blocksRaycasts = false;
        endSimulationCG.blocksRaycasts = false;
        Fade(tryagainCG, 0f, 0.5f);
        Fade(endSimulationCG, 0f, 0.5f);
        Fade(feedbackCG, 0f, 0.5f);
        Fade(completedCG, 1f, 0.5f);

        yield return new WaitForSeconds(2f);

        OnLevelComplete.Invoke();
    }

    private void Fade(CanvasGroup cg, float alpha, float duration)
    {
        cg.DOFade(alpha, duration);
    }

}
