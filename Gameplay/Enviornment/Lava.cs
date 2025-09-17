using UnityEngine;

/// <summary>
/// Kills the player on contact. 
/// Registers a death in the <see cref="GameStatsManager"/> 
/// and triggers the game over UI via <see cref="UIHandler"/>.
/// </summary>
public class Lava : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("DIE");
            GameStatsManager.Instance.RegisterDeath();
            UIHandler.Instance.GameOver();
        }
    }

}
