using TMPro;
using UnityEngine;

/// <summary>
/// Manages core UI elements in the game, including updating ghost counters
/// and displaying the Game Over screen. Implements a singleton pattern
/// for easy access from other scripts.
/// </summary>
public class UIHandler : MonoBehaviour
{
    public static UIHandler Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TMP_Text ghostNum;
    [SerializeField] private GameObject gameOverUI;

    private void Awake()
    {
        // Assign singleton instance
        Instance = this;
    }

    private void Update()
    {
        // Quick restart during testing
        if (Input.GetKeyDown(KeyCode.R))
        {
            CrossfadeToNextScene.Instance.ReloadCurrentScene();
        }
    }
    public void ChangeGhostNumber(int number)
    {
        if (ghostNum != null)
            ghostNum.text = number.ToString();
    }

    public void GameOver()
    {
        if (gameOverUI != null)
            gameOverUI.SetActive(true);
    }
}
