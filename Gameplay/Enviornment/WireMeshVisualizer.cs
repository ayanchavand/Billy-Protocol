using UnityEngine;

/// <summary>
/// Visualizes a mesh's state using emission colors. 
/// Can switch between <see cref="onColor"/> and <see cref="offColor"/> to indicate
/// active or inactive states. Intended for wiring or interactive meshes.
/// </summary>
[RequireComponent(typeof(MeshRenderer))]
public class WireMeshVisualizer : MonoBehaviour
{
    [Header("Renderer")]
    public MeshRenderer meshRenderer;

    [Header("Colors")]
    [ColorUsage(true, true)]
    public Color onColor = Color.green;

    [ColorUsage(true, true)]
    public Color offColor = Color.red;

    private void Awake()
    {
        // Auto-assign the MeshRenderer if not set in the Inspector
        if (meshRenderer == null)
            meshRenderer = GetComponent<MeshRenderer>();
    }

    /// <summary>
    /// Sets the mesh color based on the current state.
    /// </summary>
    /// <param name="isOn">True for active/on state, false for inactive/off state.</param>
    public void SetState(bool isOn)
    {
        if (meshRenderer == null || meshRenderer.material == null)
            return;

        if (meshRenderer.material.HasProperty("_EmissionColor"))
        {
            meshRenderer.material.SetColor("_EmissionColor", isOn ? onColor : offColor);
        }
    }
}
