using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform target;
    [Tooltip("If true, the object only rotates on the Y-axis. If false, it fully faces the target in all axes.")]
    public bool yAxisOnly = true;

    private void Update()
    {
        if (target == null) return;

        if (yAxisOnly)
        {
            // --- Only rotate around Y axis ---
            Vector3 direction = target.position - transform.position;
            direction.y = 0f; // Ignore vertical difference

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                float yRotation = targetRotation.eulerAngles.y + 180f; // add 180° if needed
                transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
            }
        }
        else
        {
            // --- Full 3D LookAt ---
            transform.LookAt(target);

            // Add 180° flip if you need the back to face the target instead of the front
            transform.Rotate(0f, 180f, 0f);
        }
    }
}
