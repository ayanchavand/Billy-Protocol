using UnityEngine;

[ExecuteAlways]
public class Door : MonoBehaviour
{
    [Header("General")]
    [SerializeField] bool isActive = false;

    [Header("Position Settings (Local Space)")]
    [Tooltip("Local position when the door is closed.")]
    [SerializeField] Vector3 closedPosition = Vector3.zero;

    [Tooltip("Local position when the door is open.")]
    [SerializeField] Vector3 openPosition = Vector3.up;

    [Header("Motion Settings")]
    [Tooltip("How fast the door moves, in units per second.")]
    [SerializeField] float speed = 2f;

    // t = 0 => closed; t = 1 => open
    private float t = 0f;

    void Reset()
    {
        // On first add, initialize closedPosition to wherever the door currently sits
        closedPosition = transform.localPosition;
    }

    void Update()
    {
        float target = isActive ? 1f : 0f;
        // Smoothly move t towards target at the given speed
        t = Mathf.MoveTowards(t, target, speed * Time.deltaTime);
        transform.localPosition = Vector3.Lerp(closedPosition, openPosition, t);
    }

    public void SetActive(bool on)
    {
        isActive = on;
    }

    //Visulize the open/closed positions and path in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = isActive ? Color.green : Color.gray;

        var parent = transform.parent;
        Vector3 worldClosed = parent ? parent.TransformPoint(closedPosition) : closedPosition;
        Vector3 worldOpen = parent ? parent.TransformPoint(openPosition) : openPosition;

        Gizmos.DrawLine(worldClosed, worldOpen);
        Gizmos.DrawSphere(worldClosed, 0.05f);
        Gizmos.DrawSphere(worldOpen, 0.05f);
    }
}
