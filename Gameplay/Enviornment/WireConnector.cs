using UnityEngine;

[ExecuteAlways]
public class WireConnector : MonoBehaviour
{
    public Transform fromPoint; // Switch
    public Transform toPoint;   // Target (e.g. door)
    public LineRenderer lineRenderer;

    [Header("Colors")]
    public Color onColor = Color.green;
    public Color offColor = Color.red;

    public bool isOn = false; // You can hook this from your Switch class

    void Reset()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
            lineRenderer = gameObject.AddComponent<LineRenderer>();
    }

    void Update()
    {
        if (lineRenderer == null || fromPoint == null || toPoint == null)
            return;

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, fromPoint.position);
        lineRenderer.SetPosition(1, toPoint.position);

        UpdateColor();
    }

    public void SetState(bool state)
    {
        isOn = state;
        UpdateColor();
    }

    private void UpdateColor()
    {
        if (lineRenderer != null)
        {
            Color current = isOn ? onColor : offColor;
            lineRenderer.material.SetColor("_BaseColor", current);
            if (lineRenderer.material.HasProperty("_EmissionColor"))
                lineRenderer.material.SetColor("_EmissionColor", current);
        }
    }
}
