using UnityEngine;

[CreateAssetMenu(menuName = "TimeLoop/Ghost Settings")]
/// <summary>
/// Stores configuration data for ghosts in the TimeLoop system. 
/// Includes prefab references, materials, layer assignments, 
/// and spawn limits. This ScriptableObject allows designers 
/// to tweak ghost behavior and appearance without touching code.
/// </summary>
public class GhostSettings : ScriptableObject
{
    [Header("Prefab + Material")]
    public GameObject ghostPrefab;
    public Material ghostMaterial;

    [Header("Layer & Limits")]
    public LayerMask ghostLayer = 1 << 8;
    public bool infiniteGhosts = false;
    public int maxGhosts = 10;
}
