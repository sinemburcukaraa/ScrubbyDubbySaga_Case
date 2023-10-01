using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridSingle : MonoBehaviour, IPointerDownHandler
{
    public Fruit point;
    public Vector2Int myLocalCoordinate;
    private void Start()
    {
        //Create();
    }
    //Generates random objects(fruits).
    //public int RandomIndex()
    //{
    //    int index = Random.Range(0, FruitManager.Instance.fruitPrefabs.Count);
    //    return index;
    //}

    //public void Create()
    //{
    //    point = Instantiate(FruitManager.Instance.fruitPrefabs[RandomIndex()].transform, this.transform).GetComponent<Fruit>();
    //}

    public void OnPointerDown(PointerEventData eventData)
    {
        //InputManager.Instance.OnMouseStartMoving(this);
    }

}
