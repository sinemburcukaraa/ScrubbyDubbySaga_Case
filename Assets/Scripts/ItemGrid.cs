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

    public void CheckMatches()
    {
        for (int i = 0; i < fixedWidth; i++)
        {
            for (int j = 0; j < fixedHeight; j++)
            {
                FruitItem currentItem = fruitItemSlot[i, j];
                if (currentItem == null)
                {
                    continue;
                }

                List<FruitItem> horizontalMatches = GetMatchesInDirection(currentItem, Vector2Int.right);
                List<FruitItem> verticalMatches = GetMatchesInDirection(currentItem, Vector2Int.up);

                if (horizontalMatches.Count >= 3)
                {
                    foreach (var item in horizontalMatches)
                    {
                        PickupItem(item.myPos.x, item.myPos.y);
                        print(item.myId);

                        Destroy(item.gameObject);
                    }
                }

                if (verticalMatches.Count >= 3)
                {
                    foreach (var item in verticalMatches)
                    {
                        PickupItem(item.myPos.x, item.myPos.y);
                        print(item.myId);
                        Destroy(item.gameObject);
                    }
                }
            }
        }
    }

    public bool IsThereAnyMatch()
    {
        for (int i = 0; i < fixedWidth; i++)
        {
            for (int j = 0; j < fixedHeight; j++)
            {
                FruitItem currentItem = fruitItemSlot[i, j];
                if (currentItem == null)
                {
                    continue;
                }

                List<FruitItem> horizontalMatches = GetMatchesInDirection(currentItem, Vector2Int.right);
                List<FruitItem> verticalMatches = GetMatchesInDirection(currentItem, Vector2Int.up);

                if (horizontalMatches.Count >= 3)
                {
                    return true;
                }

                if (verticalMatches.Count >= 3)
                {
                    return true;
                }
            }
        }
        return false;


    }
    int lastY = 0;

    //Coroutine planB;
    public IEnumerator FillEmptySpaces()
    {
        //if (planB != null)
        //{
        //    StopCoroutine(planB);
        //}
        //Sequence sequence = DOTween.Sequence();
        int twinCount = 0;
        List<FruitItem> FruitItems = new List<FruitItem>();
        List<Vector3> newPosL = new List<Vector3>();
        List<Vector2Int> newPosGrid = new List<Vector2Int>();

        lastY = 0;
        for (int y = HeightDifference() - 1; y <= gridSizeHeight; y++)
        {
            for (int x = WidthDifference() - 1; x <= gridSizeWidth; x++)
            {
                if (!CheckIfSlotFull(x, y))
                {
                    for (int i = y + 1; i <= gridSizeHeight; i++)
                    {
                        if (!CheckIfSlotFull(x, i))
                        {
                            continue;
                        }

                        FruitItem fruit = PickupItem(x, i);
                        DoAnimationMove(fruit, x, y);
                        break;

                    }

                }
            }
        }
        for (int y = HeightDifference() - 1; y <= gridSizeHeight; y++)
        {
            for (int x = WidthDifference() - 1; x <= gridSizeWidth; x++)

            {
                if (!CheckIfSlotFull(x, y))
                {
                    FruitItem randomFruit = GetRandomItem(FruitManager.Instance.fruitItemPrefabs);
                    InstantiataAndPlaceItem(randomFruit, x, fixedHeight - 1);
                    FruitItem fruitItem = PickupItem(x, fixedHeight - 1);
                    DoAnimationMove(fruitItem, x, y);

                }
            }
        }

        yield return StartCoroutine(DoCheckSystemLerping());
        void DoAnimationMove(FruitItem fruit, int x, int y)
        {

            if (lastY != fruit.myPos.y)
            {
                twinCount++;
                lastY = fruit.myPos.y;
            }


            PlaceItemWithoutMove(fruit, x, y);
            Vector3 newPos = GetWorldPosByGridPos(x, y);
            FruitItems.Add(fruit);
            newPosL.Add(newPos);
            newPosGrid.Add(new Vector2Int(x, y));
            //sequence.Insert(twinCount * 0.05f, fruit.transform.DOMoveY(newPos.y, 0.5f).SetEase(Ease.OutBack));
            //sequence.AppendInterval(0.01f);
            //sequence.Join(fruit.transform.DOMoveY(newPos.y, 1).SetEase(Ease.OutBack).SetSpeedBased());
            //sequence.OnComplete(() =>
            //{
            //    Debug.Log("sgf");
            //    MovementManager.Instance.ActivateInput();
            //});
        }
        IEnumerator DoCheckSystemLerping()
        {
            //yield return new WaitForSeconds(3);
            bool[] canBreak = new bool[FruitItems.Count];
            while (!isAllTrue(canBreak)) // Lerp And 
            {
                for (int i = 0; i < FruitItems.Count; i++)
                {
                    if (canBreak[i] == true)
                        continue;
                    FruitItem item = FruitItems[i];
               
                    item.transform.position = Vector3.Lerp(item.transform.position, newPosL[i], Time.deltaTime * (10 - newPosGrid[i].y));
                    if (Vector3.Distance(newPosL[i], item.transform.position) <= 1f)
                        canBreak[i] = true;
                }
                yield return null;
            }


            bool isAllTrue(bool[] bools)
            {
                foreach (var item in bools)
                {
                    if (item == false)
                        return false;
                }
                return true;
            }
        }


    }

    public void MatchCheck()
    {
        StartCoroutine(MatchCheckCoroutine());
    }
    private IEnumerator MatchCheckCoroutine()
    {
        while (IsThereAnyMatch())
        {
            CheckMatches();
            yield return FillEmptySpaces();
            yield return null;
        }
        MovementManager.Instance.ActivateInput();

    }
    private List<FruitItem> GetMatchesInDirection(FruitItem startItem, Vector2Int direction)
    {
        List<FruitItem> matches = new List<FruitItem>();
        matches.Add(startItem);
        Vector2Int currentPos = startItem.myPos + direction;

        while (AmIInGrid(currentPos.x, currentPos.y))
        {
            FruitItem currentItem = GetItem(currentPos.x, currentPos.y);
            if (currentItem != null && currentItem.myId == startItem.myId)
            {
                matches.Add(currentItem);
                currentPos += direction;
            }
            else
            {
                break;
            }
        }

        return matches;
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

        // Sol komşuyu kontrol et
        if (x > 1 && fruitItemSlot[x - 1, y] != null && fruitItemSlot[x - 1, y].myId == typeId)
            return true;

        // Üst komşuyu kontrol et
        if (y > 1 && fruitItemSlot[x, y - 1] != null && fruitItemSlot[x, y - 1].myId == typeId)
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
    public T GetRandomItem<T>(IList<T> items) //generic, extentions
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
        //if (posX < 0 || posY < 0)
        //    return false;
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

    public bool CheckIfSlotFull(int posX, int posY) => fruitItemSlot[posX, posY] != null;
}


