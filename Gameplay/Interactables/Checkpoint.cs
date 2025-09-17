using UnityEngine;

[RequireComponent(typeof(Collider))]
/// <summary>
/// Detects when a player activates this checkpoint and notifies 
/// the <see cref="CheckpointManager"/> to update the current checkpoint.
/// </summary>
public class Checkpoint : MonoBehaviour
{
    // Tell the manager we've activated this checkpoint when player enters the checkpoint trigger
    private void OnTriggerEnter(Collider other)
    {
        var controller = other.GetComponent<PlatformerController>();
        if (controller != null)
        {
            CheckpointManager.Instance.RegisterCheckpoint(transform);
        }
    }
}
