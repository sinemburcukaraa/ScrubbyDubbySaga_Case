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
        CanvasControl(false);
    }
    public void win()
    {
        CanvasControl(true);
    }

    private void CanvasControl(bool control)
    {
        gamePanel.SetActive(!control);
        winPanel.SetActive(control);
    }

    public void restartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
