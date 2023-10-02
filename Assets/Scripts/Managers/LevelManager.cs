using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LevelManager : MonoBehaviour
{
    public List<Levels> levels = new List<Levels>();
    int nextLevelCount = 0;
    public ItemGrid ItemGrid;
    public int redC, greenC, blueC, orangeC;

    #region Instance
    public static LevelManager instance;
    private void Awake()
    {
        if (instance == null) { instance = this; }

        LevelOptions();

    }
    #endregion

    //private void Start()
    //{

    //    DOVirtual.DelayedCall(1, () =>
    //    {
    //    });

    //}

    public void NextLevel()
    {
        if (nextLevelCount < levels.Count - 1)
        {
            nextLevelCount++;

        }
        else
        {
            nextLevelCount = 0;
        }


        PlayerPrefs.SetInt("LevelCount", nextLevelCount);
    }

    void LevelOptions()
    {
        nextLevelCount = PlayerPrefs.GetInt("LevelCount");
        ItemGrid.gridSizeWidth = levels[nextLevelCount].gridX;
        ItemGrid.gridSizeHeight = levels[nextLevelCount].gridY;
    }
    public void CorrectMatchCompleted()
    {
        if (redC >= levels[nextLevelCount].redCount &&
            greenC >= levels[nextLevelCount].greenCount &&
            blueC >= levels[nextLevelCount].blueCount &&
            orangeC >= levels[nextLevelCount].orangeCount)
        {
            GameManager.Instance.Win();
        }
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