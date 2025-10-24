using DG.Tweening;
using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    [Header("References")]
    public CinemachineSplineDolly dolly;
    public CanvasGroup startGameCG, endGameCG;
    public Image audioBtnImage;
    public Sprite muteSprite, unmuteSprite;
    public AudioSource audioSource;

    [Header("Settings")]
    public float moveSpeed = 0.2f;

    [Header("Events")]
    public UnityEvent OnStartSuperposition;
    public UnityEvent OnStartEntanglement;
    public UnityEvent OnStartTunneling;
    public UnityEvent OnReachedSuperposition;
    public UnityEvent OnReachedEntanglement;
    public UnityEvent OnReachedTunneling;

    private bool isPlaying = false, isMute = false;
    private int idx = 1;

    void Start()
    {
        if (dolly == null)
            dolly = GetComponent<CinemachineSplineDolly>();
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

    public void MusicToggle()
    {
        isMute = !isMute;

        if(isMute)
        {
            audioSource.Pause();
            audioBtnImage.sprite = muteSprite;
        }
        else
        {
            audioSource.Play();
            audioBtnImage.sprite = unmuteSprite;
        }
    }

    public void StartGame()
    {
        StartCoroutine(StartGameCoroutine());
    }

    private IEnumerator StartGameCoroutine()
    {
        startGameCG.blocksRaycasts = false;
        Fade(startGameCG, 0f, 0.5f);
        yield return new WaitForSeconds(0.5f);

        StartDolly(dolly.Spline);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void StartDolly(SplineContainer splineContainer)
    {
        StartCoroutine(StartDollyCoroutine(splineContainer));
    }

    private IEnumerator StartDollyCoroutine(SplineContainer splineContainer)
    {
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
        yield return new WaitForSeconds(1f);

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
            case 4:
                StartCoroutine(EndGameCoroutine());
                break;
        }

        idx++;
    }

    private IEnumerator EndGameCoroutine()
    {
        Fade(endGameCG, 1f, 0.5f);
        yield return new WaitForSeconds(0.5f);

        endGameCG.blocksRaycasts = true;
    }

    private void Fade(CanvasGroup cg, float alpha, float duration)
    {
        cg.DOFade(alpha, duration);
    }
}
