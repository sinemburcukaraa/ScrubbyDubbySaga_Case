using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    public GameObject winPanel, gamePanel;
    private void Awake()
    {
        if (Instance == null) { Instance = this; }
    }

    private void Start()
    {
        GamePanel(false);
    }
    public void Win()
    {
        GamePanel(true);
    }

    private void GamePanel(bool control)
    {
        gamePanel.SetActive(!control);
        winPanel.SetActive(control);
    }

    public void restartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
