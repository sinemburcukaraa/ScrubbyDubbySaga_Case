using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FillEmptySpaces : MonoBehaviour
{
    ItemGrid ItemGrid;

    public IEnumerator FillSpaces(ItemGrid itemGrid, int height, int widht)
    {
        List<FruitItem> FruitItems = new List<FruitItem>();
        List<Vector3> newPosL = new List<Vector3>();
        List<Vector2Int> newPosGrid = new List<Vector2Int>();
        int lastY = 0;
        int twinCount = 0;
        ItemGrid = itemGrid;
        lastY = 0;
        for (int y = itemGrid.HeightDifference() - 1; y <= height; y++)
        {
            for (int x = itemGrid.WidthDifference() - 1; x <= widht; x++)
            {
                if (!CheckIfSlotFull(x, y))
                {
                    for (int i = y + 1; i <= height; i++)
                    {
                        if (!CheckIfSlotFull(x, i))
                        {
                            continue;
                        }

                        FruitItem fruit = itemGrid.PickupItem(x, i);
                        DoAnimationMove(fruit, x, y);
                        break;

                    }

                }
            }
        }
        for (int y = itemGrid.HeightDifference() - 1; y <= height; y++)
        {
            for (int x = itemGrid.WidthDifference() - 1; x <= widht; x++)

            {
                if (!CheckIfSlotFull(x, y))
                {
                    FruitItem randomFruit = itemGrid.GetRandomItem(FruitManager.Instance.fruitItemPrefabs);
                    itemGrid.InstantiataAndPlaceItem(randomFruit, x, itemGrid.fixedHeight - 1);
                    FruitItem fruitItem = itemGrid.PickupItem(x, itemGrid.fixedHeight - 1);
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


            ItemGrid.PlaceItemWithoutMove(fruit, x, y);
            Vector3 newPos = ItemGrid.GetWorldPosByGridPos(x, y);
            FruitItems.Add(fruit);
            newPosL.Add(newPos);
            newPosGrid.Add(new Vector2Int(x, y));

        }
        IEnumerator DoCheckSystemLerping()
        {

            bool[] canBreak = new bool[FruitItems.Count];
            while (!isAllTrue(canBreak)) // Lerp And 
            {

                for (int i = 0; i < FruitItems.Count; i++)
                {
                    FruitItem item = null;

                    if (canBreak[i] == true)
                        continue;

                    if (FruitItems.Count > i)
                        item = FruitItems[i];

                    if (item != null)
                        item.transform.position = Vector3.Lerp(item.transform.position, newPosL[i], Time.deltaTime * (10 - newPosGrid[i].y));

                    if (Vector3.Distance(newPosL[i], item.transform.position) <= 1f)
                        canBreak[i] = true;

                }
                yield return null;
            }



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
    public bool CheckIfSlotFull(int posX, int posY) => ItemGrid.fruitItemSlot[posX, posY] != null;

}
