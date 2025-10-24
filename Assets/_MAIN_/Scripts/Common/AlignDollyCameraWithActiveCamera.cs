using UnityEngine;

public class AlignDollyCameraWithActiveCamera : MonoBehaviour
{
    public Camera dollyCamera;

    void Update()
    {
        dollyCamera.transform.position = Camera.main.transform.position;
        dollyCamera.transform.rotation = Camera.main.transform.rotation;
    }
}
