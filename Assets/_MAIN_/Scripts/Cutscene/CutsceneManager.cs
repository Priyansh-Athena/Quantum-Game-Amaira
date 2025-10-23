using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;

public class CutsceneManager : MonoBehaviour
{
    [Header("References")]
    public CinemachineSplineDolly dolly;

    [Header("Settings")]
    public float moveSpeed = 0.2f;

    [Header("Events")]
    public UnityEvent OnStartSuperposition;
    public UnityEvent OnStartEntanglement;
    public UnityEvent OnStartTunneling;
    public UnityEvent OnReachedSuperposition;
    public UnityEvent OnReachedEntanglement;
    public UnityEvent OnReachedTunneling;

    private bool isPlaying = false;
    private int idx = 1;

    void Start()
    {
        if (dolly == null)
            dolly = GetComponent<CinemachineSplineDolly>();

        StartDolly(dolly.Spline);
    }

    void Update()
    {
        if (!isPlaying || dolly == null)
            return;

        dolly.CameraPosition += moveSpeed * Time.deltaTime;

        if (dolly.CameraPosition >= 1f)
        {
            dolly.CameraPosition = 1f;
            isPlaying = false;
            OnCutsceneEnd();
        }
    }

    public void StartDolly(SplineContainer splineContainer)
    {
        StartCoroutine(StartDollyCoroutine(splineContainer));
    }

    private IEnumerator StartDollyCoroutine(SplineContainer splineContainer)
    {
        yield return new WaitForSeconds(2f);
        switch (idx)
        {
            case 1:
                OnStartSuperposition.Invoke();
                break;
            case 2:
                OnStartEntanglement.Invoke();
                break;
            case 3:
                OnStartTunneling.Invoke();
                break;
        }
        dolly.Spline = splineContainer;

        if (dolly == null) yield return null;

        dolly.CameraPosition = 0f;
        isPlaying = true;
    }

    private void OnCutsceneEnd()
    {
        StartCoroutine(OnCutsceneEndCutscene());
    }

    private IEnumerator OnCutsceneEndCutscene()
    {
        yield return new WaitForSeconds(2f);

        switch (idx)
        {
            case 1:
                OnReachedSuperposition.Invoke();
                break;
            case 2:
                OnReachedEntanglement.Invoke();
                break;
            case 3:
                OnReachedTunneling.Invoke();
                break;
        }

        idx++;
    }
}
