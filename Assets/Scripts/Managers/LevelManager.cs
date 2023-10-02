using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LevelManager : MonoBehaviour
{
    #region Instance
    public static LevelManager instance;
    private void Awake()
    {
        if (instance == null) { instance = this; }
    }
    #endregion
    public List<Levels> levels = new List<Levels>();
    int nextLevelCount = 0;
    ItemGrid ItemGrid;
    public int redC, greenC, blueC, orangeC;
    public bool correctMatchCompleted()
    {
        if (redC >= levels[nextLevelCount].redCount &&
            greenC >= levels[nextLevelCount].greenCount &&
            blueC >= levels[nextLevelCount].blueCount &&
            orangeC >= levels[nextLevelCount].orangeCount)
        {
            GameManager.Instance.win();
            return true;
        }
        return false;
    }

    public void NextLevel()
    {
        nextLevelCount++;
        PlayerPrefs.SetInt("LevelCount", nextLevelCount);
        LevelOptions();
    }

    void LevelOptions()
    {
        nextLevelCount = PlayerPrefs.GetInt("LevelCount");
        ItemGrid.gridSizeWidth = levels[nextLevelCount].gridX;
        ItemGrid.gridSizeWidth = levels[nextLevelCount].gridY;
    }
}


[System.Serializable]
public class Levels
{
    public int gridX;
    public int gridY;
    public int redCount;
    public int greenCount;
    public int blueCount;
    public int orangeCount;
}