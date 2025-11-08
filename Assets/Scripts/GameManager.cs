using UnityEngine;
using UnityEngine.SceneManagement; 

public class GameManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject titleScreenPanel;
    public GameObject winScreenPanel;
    public GameObject deathScreenPanel;

    [Header("Game Actors")]
    public SimpleFPC playerScript;
    public MicrophoneInput micScript;
    public HunterAI hunterScript;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;

        if (titleScreenPanel != null)
            titleScreenPanel.SetActive(true);
            
        if (winScreenPanel != null)
            winScreenPanel.SetActive(false);

        if (deathScreenPanel != null)
            deathScreenPanel.SetActive(false);
            
        if (playerScript != null) playerScript.enabled = false;
        if (micScript != null) micScript.enabled = false;
        if (hunterScript != null) hunterScript.enabled = false;
    }

    public void StartGame()
    {
        Debug.Log("Starting game...");
        Cursor.lockState = CursorLockMode.Locked;

        if (titleScreenPanel != null)
            titleScreenPanel.SetActive(false);
            
        if (playerScript != null) playerScript.enabled = true;
        if (micScript != null) micScript.enabled = true;
        if (hunterScript != null) hunterScript.enabled = true;
    }

    public void RestartGame()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
    
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}