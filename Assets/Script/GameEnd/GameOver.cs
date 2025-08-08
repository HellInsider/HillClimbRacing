using TMPro;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] public GameObject GameOverPanel;
    [SerializeField] public Canvas GameOverPanelCanvas;
    [SerializeField] private TextMeshProUGUI TotalRoad;
    [SerializeField] private LevelMenager levelMenager;

    public void Start()
    {
        GameOverPanelCanvas.enabled = false;
        GameOverPanel.SetActive(false);
    }
    public void GameOverPlayer()
    {
        new WaitForSeconds(1f);
        Time.timeScale = 0f;
        GameOverPanelCanvas.enabled = true;
        GameOverPanel.SetActive(true);
        TotalRoad.text = "Total: " + ((int)levelMenager.recordTrack).ToString();
    }
}
