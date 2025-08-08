using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public Canvas pauseMenuPanelCanvas;
    private bool isPaused = false;
    void Start()
    {
        pauseMenuPanel.SetActive(false);
    }
    public void TogglePause()
    {
        pauseMenuPanelCanvas.enabled = true;
        isPaused = !isPaused;

        if (isPaused)
        {
            
            Time.timeScale = 0f; 
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(true); 
            }
        }
        else
        {
            
            Time.timeScale = 1f; 
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(false); 
            }
        }
    }
    public void GoToMainMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainMenu"); 
    }
    public void GoToMainGarage()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Garage");
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }
}
