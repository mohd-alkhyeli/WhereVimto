using UnityEngine;

[ExecuteAlways] // So gizmos draw even when not playing
public class EnvironmentFacingTrigger : MonoBehaviour
{
    [Tooltip("Direction the player must be facing to trigger the environment change (e.g., Vector3.forward).")]
    public Vector3 requiredFacingDirection = Vector3.forward;

    [Tooltip("Angle threshold in degrees within which the facing direction is accepted.")]
    public float facingThresholdAngle = 45f;

    [Tooltip("GameObjects to enable when triggered.")]
    public GameObject[] objectsToEnable;

    [Tooltip("GameObjects to disable when triggered.")]
    public GameObject[] objectsToDisable;

    private bool playerInside = false;
    private Transform playerTransform;
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            playerTransform = other.transform;
            hasTriggered = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            playerTransform = null;
            hasTriggered = false;
        }
    }

    private void Update()
    {
        if (playerInside && playerTransform != null && !hasTriggered)
        {
            Vector3 playerForward = playerTransform.forward;
            float angle = Vector3.Angle(requiredFacingDirection.normalized, playerForward.normalized);

            Debug.Log($"[Facing Check] Player Facing: {playerForward}, Required Direction: {requiredFacingDirection.normalized}, Angle: {angle:F2}, Threshold: {facingThresholdAngle}");

            if (angle <= facingThresholdAngle)
            {
                hasTriggered = true;

                foreach (GameObject go in objectsToEnable)
                    if (go != null) go.SetActive(true);

                foreach (GameObject go in objectsToDisable)
                    if (go != null) go.SetActive(false);

                Debug.Log("Environment triggered based on player facing direction.");
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Draw the required facing direction from this object
        Gizmos.color = Color.cyan;
        Vector3 origin = transform.position;
        Vector3 dir = transform.TransformDirection(requiredFacingDirection.normalized) * 2f;
        Gizmos.DrawLine(origin, origin + dir);
        Gizmos.DrawSphere(origin + dir, 0.1f);

        // Draw the angle cone
        int segments = 30;
        float step = facingThresholdAngle * 2f / segments;
        Quaternion baseRot = Quaternion.LookRotation(requiredFacingDirection.normalized);

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.4f); // Orange-ish
        Vector3 prevPoint = Vector3.zero;

        for (int i = 0; i <= segments; i++)
        {
            float angle = -facingThresholdAngle + step * i;
            Quaternion rot = baseRot * Quaternion.Euler(0, angle, 0);
            Vector3 point = origin + rot * Vector3.forward * 2f;

            if (i > 0)
                Gizmos.DrawLine(prevPoint, point);

            prevPoint = point;
        }

        // Draw base line
        Gizmos.DrawLine(origin, origin + baseRot * Quaternion.Euler(0, -facingThresholdAngle, 0) * Vector3.forward * 2f);
        Gizmos.DrawLine(origin, origin + baseRot * Quaternion.Euler(0, facingThresholdAngle, 0) * Vector3.forward * 2f);
    }
}
