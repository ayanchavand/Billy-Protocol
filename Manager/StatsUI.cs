using TMPro;
using UnityEngine;

public class StatsUI : MonoBehaviour
{
    [SerializeField] TMP_Text ghost;
    [SerializeField] TMP_Text deaths;

    private void Start()
    {
        ghost.text = GameStatsManager.Instance.GetGhostCount().ToString();
        deaths.text = GameStatsManager.Instance.GetDeathCount().ToString();
    }
}
