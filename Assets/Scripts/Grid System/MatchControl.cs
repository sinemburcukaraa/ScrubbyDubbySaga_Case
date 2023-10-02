using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchControl : MonoBehaviour
{
    public ItemGrid itemGrid;


    public void MatchCheck()
    {
        itemGrid = GridManager.Instance.grid;

        StartCoroutine(MatchCheckCoroutine());
    }
    private IEnumerator MatchCheckCoroutine()
    {
        while (IsThereAnyMatch())
        {
            CheckMatches();
            yield return StartCoroutine(GridManager.Instance.emptySpaces.FillSpaces(itemGrid, itemGrid.gridSizeHeight, itemGrid.gridSizeWidth));
            yield return null;
        }
        MovementManager.Instance.ActivateInput();

    }
    public void CheckMatches()
    {

        for (int i = 0; i < itemGrid.gridSizeWidth; i++)
        {
            for (int j = 0; j < itemGrid.gridSizeHeight; j++)
            {
                FruitItem currentItem = itemGrid.fruitItemSlot[i, j];
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
                        itemGrid.PickupItem(item.myPos.x, item.myPos.y);
                        CorrectMatchControl(item.myId);
                        Destroy(item.gameObject);
                    }
                }

                if (verticalMatches.Count >= 3)
                {
                    foreach (var item in verticalMatches)
                    {
                        itemGrid.PickupItem(item.myPos.x, item.myPos.y);
                        CorrectMatchControl(item.myId);
                        Destroy(item.gameObject);
                    }
                }
            }
        }
    }


    public bool IsThereAnyMatch()
    {
        for (int i = 0; i < itemGrid.gridSizeWidth; i++)
        {
            for (int j = 0; j < itemGrid.gridSizeHeight; j++)
            {
                FruitItem currentItem = itemGrid.fruitItemSlot[i, j];
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

    private List<FruitItem> GetMatchesInDirection(FruitItem startItem, Vector2Int direction)
    {
        List<FruitItem> matches = new List<FruitItem>();
        matches.Add(startItem);
        Vector2Int currentPos = startItem.myPos + direction;

        while (itemGrid.AmIInGrid(currentPos.x, currentPos.y))
        {
            FruitItem currentItem = itemGrid.GetItem(currentPos.x, currentPos.y);
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
    public void CorrectMatchControl(int item)
    {
        switch (item)
        {
            case 0:
                LevelManager.instance.blueC++;
                break;
            case 1:
                LevelManager.instance.greenC++;
                break;
            case 2:
                LevelManager.instance.orangeC++;
                break;
            case 3:
                LevelManager.instance.redC++;
                break;
        }

        LevelManager.instance.CorrectMatchCompleted();
    }
}
