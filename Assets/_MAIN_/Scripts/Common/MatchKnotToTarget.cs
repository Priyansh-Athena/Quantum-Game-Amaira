using UnityEngine;
using UnityEngine.Splines;

[ExecuteAlways]
public class MatchKnotToTarget : MonoBehaviour
{
    public SplineContainer splineContainer;
    public GameObject target;
    public bool matchLastKnotNotFirst = true;
    public float tangentLength = 0.5f; // how long the tangent handle should be

    void Update()
    {
        if (splineContainer == null || target == null)
            return;

        var spline = splineContainer.Spline;
        if (spline.Count == 0)
            return;

        int idx = matchLastKnotNotFirst ? spline.Count - 1 : 0;

        // Convert target position to spline local space
        Vector3 localPos = splineContainer.transform.InverseTransformPoint(target.transform.position);

        // Get knot and update position
        BezierKnot knot = spline[idx];
        knot.Position = localPos;

        // Compute local rotation relative to the spline container
        Quaternion localRot = Quaternion.Inverse(splineContainer.transform.rotation) * target.transform.rotation;
        knot.Rotation = localRot;

        // Match tangent direction with target's forward (in local space)
        Vector3 localForward = splineContainer.transform.InverseTransformDirection(target.transform.forward);
        Vector3 tangent = localForward.normalized * tangentLength;

        if (matchLastKnotNotFirst)
            knot.TangentIn = -tangent;
        else
            knot.TangentOut = tangent;

        // Apply modified knot back to the spline
        spline.SetKnot(idx, knot);
    }
}
