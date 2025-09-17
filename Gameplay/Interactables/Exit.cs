using System.Collections;
using UnityEngine;

/// <summary>
/// Handles player exits. Can either:
/// 1. Load the next level (normal gameplay),
/// 2. Temporarily adjust the max ghosts for prototype levels (legacy/prototyping mode).
/// <b>Note:</b> The SetMaxGhosts mode exists for prototype levels and is intentionally left
/// in the script. Do NOT remove it, even if it’s unused in the final game
/// </summary>
public class Exit : MonoBehaviour
{
    public enum ExitMode
    {
        LoadNextLevel,    // your old behavior
        SetMaxGhosts      // tweak GhostManager.maxGhosts instead
    }

    [Header("General")]
    [SerializeField] ExitMode exitMode = ExitMode.LoadNextLevel;
    [Tooltip("Only used in LoadNextLevel mode")]
    [SerializeField] float loadDelay = 1f;

    [Header("Ghost-Limit Settings")]
    [Tooltip("Only used in SetMaxGhosts mode")]
    [SerializeField] int newMaxGhosts = 10;

    private void OnTriggerEnter(Collider other)
    {
        switch (exitMode)
        {
            case ExitMode.LoadNextLevel:
                StartCoroutine(LoadNextLevelInSeconds(loadDelay));
                break;
            
            case ExitMode.SetMaxGhosts:
                if (GhostManager.Instance != null)
                {
                    GhostManager.Instance.maxGhosts = newMaxGhosts;
                    //UIHandler.Instance.ChangeGhostNumber(newMaxGhosts);
                    Debug.Log($"[Exit] maxGhosts set to {newMaxGhosts}");
                }
                else
                {
                    Debug.LogWarning("[Exit] No GhostManager instance found!");
                }
                break;
        }
    }

     IEnumerator LoadNextLevelInSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        CrossfadeToNextScene.Instance.StartTransitionToNextScene();
    }
}
