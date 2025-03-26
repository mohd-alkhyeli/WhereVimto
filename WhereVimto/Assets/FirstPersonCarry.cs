using UnityEngine;

public class FirstPersonCarry : MonoBehaviour
{
    [Header("Pickup Settings")]
    public float pickupRange = 3f;
    public string carriableTag = "Carriable";
    public KeyCode pickupKey = KeyCode.E;
    public KeyCode dropKey = KeyCode.Q;

    [Header("Held Object Settings")]
    public Transform holdPoint;
    public float holdDistance = 1.5f;
    public float moveSmoothSpeed = 10f;
    public float rotateSmoothSpeed = 10f;

    private GameObject heldObject;
    private Rigidbody heldRigidbody;

    void Update()
    {
        if (Input.GetKeyDown(pickupKey))
        {
            if (heldObject == null)
            {
                TryPickup();
            }
        }

        if (Input.GetKeyDown(dropKey))
        {
            if (heldObject != null)
            {
                DropObject();
            }
        }
    }

    void FixedUpdate()
    {
        if (heldObject != null)
        {
            MoveHeldObject();
        }
    }

    void TryPickup()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange))
        {
            if (hit.collider.CompareTag(carriableTag))
            {
                heldObject = hit.collider.gameObject;
                heldRigidbody = heldObject.GetComponent<Rigidbody>();

                if (heldRigidbody != null)
                {
                    heldRigidbody.useGravity = false;
                    heldRigidbody.freezeRotation = true;
                }
            }
        }
    }

    void MoveHeldObject()
    {
        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * holdDistance;
        Vector3 direction = targetPosition - heldObject.transform.position;

        if (heldRigidbody != null)
        {
            heldRigidbody.linearVelocity = direction * moveSmoothSpeed;
            Quaternion targetRotation = Quaternion.LookRotation(Camera.main.transform.forward);
            heldRigidbody.MoveRotation(Quaternion.Slerp(heldObject.transform.rotation, targetRotation, Time.deltaTime * rotateSmoothSpeed));
        }
    }

    void DropObject()
    {
        if (heldRigidbody != null)
        {
            heldRigidbody.useGravity = true;
            heldRigidbody.freezeRotation = false;
        }

        heldObject = null;
        heldRigidbody = null;
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // Raycast debug (Pickup Ray)
        Gizmos.color = Color.green;
        Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * pickupRange);

        // Hold position debug
        if (holdPoint != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 targetHoldPosition = Camera.main.transform.position + Camera.main.transform.forward * holdDistance;
            Gizmos.DrawWireSphere(targetHoldPosition, 0.2f);
        }

        // Pickup range sphere (optional visual aid)
        Gizmos.color = new Color(0, 1, 0, 0.1f);
        Gizmos.DrawWireSphere(Camera.main.transform.position, pickupRange);
    }
}