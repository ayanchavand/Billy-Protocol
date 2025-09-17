using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    [Tooltip("Where the player starts if no other checkpoints have been activated.")]
    [SerializeField] Transform startPoint;

    [Header("Checkpoint Highlighting")]
    [SerializeField] Material highlightMaterial;
    [SerializeField] Material defaultMaterial;

    private readonly List<Transform> checkpoints = new List<Transform>();
    private int currentIndex = 0;
    private Transform lastHighlighted = null;

    // Singleton pattern implementation and initialization 
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize checkpoints list with startPoint
        checkpoints.Clear();
        if (startPoint != null)
            checkpoints.Add(startPoint);
        else
            Debug.LogError("CheckpointManager: startPoint is not assigned!");
        currentIndex = 0;
        // Highlight the first checkpoint initially
        HighlightCheckpoint(currentIndex);
    }

    public void RegisterCheckpoint(Transform cp)
    {
        // Avoid duplicates and nulls
        if (cp == null || checkpoints.Contains(cp)) return;

        checkpoints.Insert(0, cp);
        currentIndex = 0;

        HighlightCheckpoint(currentIndex);
    }

    public Transform GetNextRespawnPoint()
    {
        if (checkpoints.Count == 0)
        {
            Debug.LogError("CheckpointManager: no checkpoints available!");
            return null;
        }

        Transform respawn = checkpoints[currentIndex];
        // Move to next index or start over
        currentIndex = (currentIndex + 1) % checkpoints.Count;
        HighlightCheckpoint(currentIndex);

        return respawn;
    }

    private void HighlightCheckpoint(int index)
    {
        // Reset previous highlight
        if (lastHighlighted != null)
        {
            var renderer = lastHighlighted.GetComponent<MeshRenderer>();
            if (renderer != null && defaultMaterial != null)
                renderer.material = defaultMaterial;
        }

        // Highlight new one
        if (index < checkpoints.Count)
        {
            Transform current = checkpoints[index];
            var renderer = current.GetComponent<MeshRenderer>();
            if (renderer != null && highlightMaterial != null)
                renderer.material = highlightMaterial;

            lastHighlighted = current;
        }
    }
}
