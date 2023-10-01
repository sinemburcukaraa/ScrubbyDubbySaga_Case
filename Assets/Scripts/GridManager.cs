using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    #region SerializeField
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private float offset;
    [SerializeField] private RectTransform background;
    #endregion
    #region Singleton
    public static GridManager Instance;
    #endregion

    private GameObject tile;
    public ItemGrid grid;
    
    private void Awake()
    {
        if (Instance == null) { Instance = this; }
    }
}
