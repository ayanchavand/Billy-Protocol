using UnityEngine;

/// <summary>
/// Singleton manager that tracks overall game statistics such as ghost creations and player deaths. 
/// </summary>

public class GameStatsManager : MonoBehaviour
{
    //making sure that there's only one instance of this manager accessible everywhere
    public static GameStatsManager Instance { get; private set; }

    //Stats to track
    private int ghostCount = 0;
    private int deathCount = 0;

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // Persist across scenes
        DontDestroyOnLoad(gameObject);
    }

    // Called when a ghost is created
    public void RegisterGhost()
    {
        ghostCount++;
    }

    // Called when player resets or dies
    public void RegisterDeath()
    {
        deathCount++;
    }

    // Getters for final stats
    public int GetGhostCount()
    {
        return ghostCount;
    }

    public int GetDeathCount()
    {
        return deathCount;
    }

    //Reset stats manually (e.g., for new game)
    public void ResetStats()
    {
        ghostCount = 0;
        deathCount = 0;
    }
}
