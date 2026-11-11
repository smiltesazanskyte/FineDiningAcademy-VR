
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class TablewareSnap : MonoBehaviour
{
    public Transform snapPoint;
    public float snapDistance = 0.25f;

    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;
    public bool IsSnapped { get; private set; } = false;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    void OnDisable()
    {
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }

    void OnReleased(SelectExitEventArgs args)
    {
        if (snapPoint == null)
            return;

        float distance = Vector3.Distance(
            transform.position,
            snapPoint.position
        );

        if (distance <= snapDistance)
        {
            // Stop all movement.
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Make the object immovable by physics.
            rb.isKinematic = true;

            // Move it to its exact saved position.
            transform.SetPositionAndRotation(
                snapPoint.position,
                snapPoint.rotation
            );

            // Prevent the player from grabbing it again.
            grabInteractable.enabled = false;
            IsSnapped = true;
            Debug.Log(
            "SNAPPED: " + gameObject.name +
            " | Position: " + transform.position +
            " | Is Selected: " + grabInteractable.isSelected
            );
        }
    }
}