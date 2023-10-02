using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Sequence = DG.Tweening.Sequence;

public class ItemGrid : MonoBehaviour
{
    public const float tileSizeWidth = 85.3334f;
    public const float tileSizeHeight = 85.3334f;

    public int gridSizeWidth = 6;
    public int gridSizeHeight = 6;
    [HideInInspector] public int fixedWidth;
    [HideInInspector] public int fixedHeight;

    public FruitItem[,] fruitItemSlot;

    RectTransform gridTransform;
    public RectTransform maskTransform;
    Vector2 positionOnTheGrid = new Vector2();
    Vector2Int tileGridPosition = new Vector2Int();

    private void Start()
    {
        gridTransform = GetComponent<RectTransform>();
        Init(gridSizeWidth, gridSizeHeight);

        SpawnFruitsForVisibleGrids();
    }
    private void SpawnFruitsForVisibleGrids()
    {
        for (int i = 1; i < fixedWidth - 1; i++)
        {
            for (int j = 1; j < fixedHeight - 1; j++)
            {
                FruitItem randomFruit;

                do
                {
                    randomFruit = GetRandomItem(FruitManager.Instance.fruitItemPrefabs);
                } while (HasSameTypeNeighbor(i, j, randomFruit.myId));

                FruitItem fruitItem = Instantiate(randomFruit.gameObject).GetComponent<FruitItem>();
                PlaceItem(fruitItem, i, j);
            }
        }
    }

    private bool HasSameTypeNeighbor(int x, int y, int typeId)
    {
        if (x > 1 && fruitItemSlot[x - 1, y] != null && fruitItemSlot[x - 1, y].myId == typeId)//left
            return true;

        if (y > 1 && fruitItemSlot[x, y - 1] != null && fruitItemSlot[x, y - 1].myId == typeId)//up
            return true;

        return false;
    }


    private void Init(int width, int height)
    {
        fixedWidth = width + 2;
        fixedHeight = height + 2;
        fruitItemSlot = new FruitItem[fixedWidth, fixedHeight];
        Vector2 size = new Vector2(fixedWidth * tileSizeWidth, fixedHeight * tileSizeHeight);
        gridTransform.sizeDelta = size;

        Vector2 sizeMask = new Vector2(width * tileSizeWidth, height * tileSizeHeight);
        maskTransform.sizeDelta = sizeMask;
    }

    public Vector2Int GetTileGridPosition(Vector2 mousePosition)
    {
        positionOnTheGrid.x = mousePosition.x - gridTransform.position.x;
        positionOnTheGrid.y = mousePosition.y - gridTransform.position.y;

        tileGridPosition.x = (int)(positionOnTheGrid.x / tileSizeWidth);
        tileGridPosition.y = (int)(positionOnTheGrid.y / tileSizeHeight);

        return tileGridPosition;

    }

    public FruitItem PickupItem(int x, int y)
    {
        FruitItem toReturn = fruitItemSlot[x, y];
        fruitItemSlot[x, y] = null;
        return toReturn;
    }
    public FruitItem GetItem(int x, int y)
    {
        FruitItem toReturn = fruitItemSlot[x, y];
        return toReturn;
    }

    public void PlaceItem(FruitItem fruitItem, int posX, int posY)
    {
        RectTransform rectTransform = fruitItem.GetComponent<RectTransform>();
        rectTransform.SetParent(gridTransform);
        fruitItemSlot[posX, posY] = fruitItem;
        fruitItem.myPos = new Vector2Int(posX, posY);

        Vector2 position = new Vector2();
        position.x = posX * tileSizeWidth + tileSizeWidth / 2;
        position.y = posY * tileSizeHeight + tileSizeHeight / 2;

        rectTransform.localPosition = position;
    }
    public void PlaceItemWithoutMove(FruitItem fruitItem, int posX, int posY)
    {
        RectTransform rectTransform = fruitItem.GetComponent<RectTransform>();
        rectTransform.SetParent(gridTransform);
        fruitItemSlot[posX, posY] = fruitItem;
        fruitItem.myPos = new Vector2Int(posX, posY);

    }
    public T GetRandomItem<T>(IList<T> items) 
    {
        int index = UnityEngine.Random.Range(0, items.Count);

        return items[index];
    }

    public Vector3 GetLocalPosByGridPos(int posX, int posY)
    {
        Vector3 pos = new Vector2();
        pos.x = posX * tileSizeWidth + tileSizeWidth / 2;
        pos.y = posY * tileSizeHeight + tileSizeHeight / 2;

        return pos;
    }
    public Vector3 GetWorldPosByGridPos(int posX, int posY) => GetLocalPosByGridPos(posX, posY) + gridTransform.position;

    public int WidthDifference() => fixedWidth - gridSizeWidth;
    public int HeightDifference() => fixedHeight - gridSizeHeight;
    public bool AmIInGrid(int posX, int posY)
    {
        if (posX < WidthDifference() - 1 || posY < HeightDifference() - 1)
            return false;
        if (posX >= fixedWidth - 1 || posY >= fixedHeight - 1)
            return false;
        return true;
    }
    public bool AmIInGrid(FruitItem fruit)
    {
        Vector2Int gridPosOfItem = GridManager.Instance.grid.GetTileGridPosition(fruit.transform.position);
        return AmIInGrid(gridPosOfItem.x, gridPosOfItem.y);
    }

    public void ChangeItem(FruitItem itemToDuplicate, FruitItem itemToChange)
    {
        Vector2Int newPos = itemToChange.myPos;
        PickupAndDestroyItem(itemToChange);
        FruitItem fruitItem = Instantiate(itemToDuplicate);
        PlaceItem(fruitItem, newPos.x, newPos.y);
    }
    public void PickupAndDestroyItem(FruitItem item)
    {
        PickupItem(item.myPos.x, item.myPos.y);
        Destroy(item);
    }

    public void InstantiataAndPlaceItem(FruitItem fruit, int posX, int posY)
    {
        FruitItem fruitItem = Instantiate(fruit.gameObject).GetComponent<FruitItem>();
        PlaceItem(fruitItem, posX, posY);
    }

}


