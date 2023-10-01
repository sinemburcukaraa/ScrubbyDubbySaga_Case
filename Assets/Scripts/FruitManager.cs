using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class FruitManager : MonoBehaviour
{
    public List<Fruit> fruitPrefabs = new List<Fruit>();
    public GridManager gridSystem;
    public Dictionary<Fruit, GridSingle> fruitGridDic = new Dictionary<Fruit, GridSingle>();
    #region Singleton
    public static FruitManager Instance;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
    }
    #endregion
  
    public List<FruitItem> fruitItemPrefabs = new List<FruitItem>();
    
    //public void UpdateMyGrid(Fruit f, GridSingle g)
    //{
    //    fruitGridDic[f] = g;
    //}
}
