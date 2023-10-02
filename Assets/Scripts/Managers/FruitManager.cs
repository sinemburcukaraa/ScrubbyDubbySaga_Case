using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class FruitManager : MonoBehaviour
{
    #region Singleton
    public static FruitManager Instance;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
    }
    #endregion

    public List<FruitItem> fruitItemPrefabs = new List<FruitItem>();

}
