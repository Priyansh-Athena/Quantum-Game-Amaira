using Unity.Cinemachine;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    public CinemachineSplineDolly dolly;
    public float moveSpeed = 0.2f;
    public GameObject objectToEnable;

    private bool isPlaying = false;

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

        if (dolly.CameraPosition >= 0.99f)
        {
            dolly.CameraPosition = 0.99f;
            isPlaying = false;
            OnCutsceneEnd();
        }
    }

    public void StartDolly()
    {
        if (dolly == null) return;

        dolly.CameraPosition = 0f;
        isPlaying = true;
    }

    private void OnCutsceneEnd()
    {
        if (objectToEnable != null)
            objectToEnable.SetActive(true);

        Debug.Log("Cutscene Completed");
    }
}
