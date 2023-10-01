using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfiniteScroll : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform viewPortTransform;
    private RectTransform contentPanelTransform;
    public RectTransform contentPanelTransformHorizontal;
    public RectTransform contentPanelTransformVertical;
    public HorizontalLayoutGroup HLG;
    public RectTransform[] ItemList;
    public List<RectTransform> allItemList = new List<RectTransform>();
    Vector2 OldVelocity;
    bool isUpdated;
    private RectMask2D viewportMask;

    public bool doScrolling = false;

    Vector2Int direction;
    Vector3 startPosOfViewPortTransform;
    public Image[] raycastObjects;

    private IEnumerator Start()
    {
        viewportMask = viewPortTransform.GetComponent<RectMask2D>();
        yield return null;
        yield return null;
        startPosOfViewPortTransform = viewPortTransform.localPosition;
    }

    public void StartScrolling(RectTransform[] ItemList, Vector2Int dir)
    {
        this.ItemList = ItemList;
        allItemList.Clear();
        direction = dir;
        scrollRect.horizontal = false;
        scrollRect.vertical = false;

        if (direction.x > 0) // If direction is Horizontal
        {
            contentPanelTransform = contentPanelTransformHorizontal;
            scrollRect.horizontal = true;

            viewportMask.softness = new Vector2Int(50, 0);

            Vector2 newPos = viewPortTransform.position;
            newPos.y = ItemList[0].transform.position.y + ItemList[0].sizeDelta.y / 2;
            viewPortTransform.position = newPos;
        }
        else if (direction.y > 0) // If direction is Vertical
        {
            Array.Reverse(this.ItemList); // To Get Correct Order

            scrollRect.vertical = true;
            contentPanelTransform = contentPanelTransformVertical;

            viewportMask.softness = new Vector2Int(0, 50);


            Vector2 newPos = viewPortTransform.position;
            newPos.x = ItemList[0].transform.position.x - ItemList[0].sizeDelta.x / 2;
            viewPortTransform.position = newPos;
        }
        scrollRect.content = contentPanelTransform;
        contentPanelTransform.localPosition = Vector2.zero;

        Array.ForEach(ItemList, x =>
        {
            x.transform.SetParent(contentPanelTransform);
            allItemList.Add(x);
        });
        Init();
        doScrolling = true;
    }

    public void StopScrolling()
    {
        doScrolling = false;

    }
    private void Init()
    {
        isUpdated = false;
        OldVelocity = Vector2.zero;
        //int ItemsToAdd = Mathf.CeilToInt(viewPortTransform.rect.width / (ItemList[0].rect.width + HLG.spacing));
        int ItemsToAdd;
        if(direction.x > 0)
            ItemsToAdd = (int)(viewPortTransform.rect.width / (ItemList[0].rect.width + HLG.spacing));
        else
            ItemsToAdd = (int)(viewPortTransform.rect.height / (ItemList[0].rect.height + HLG.spacing));

        for (int i = 0; i < ItemsToAdd; i++)
{
            RectTransform RT = Instantiate(ItemList[i % ItemList.Length], contentPanelTransform);
            RT.SetAsLastSibling();
            allItemList.Add(RT);
        }
        for (int i = 0; i < ItemsToAdd; i++)
{
            int num = ItemList.Length - i - 1;
            while (num < 0)
            {
                num += ItemList.Length;
            }
            RectTransform RT = Instantiate(ItemList[num], contentPanelTransform);
            RT.SetAsFirstSibling();
            allItemList.Add(RT);
        }
        //contentPanelTransform.localPosition = new Vector3((1 - (ItemList[0].rect.width + HLG.spacing) * ItemsToAdd),
        //    contentPanelTransform.localPosition.y,
        //    contentPanelTransform.localPosition.z);
        //contentPanelTransform.localPosition += new Vector3((0 - GridManager.Instance.grid.GetTileGridPosition(Input.mousePosition).x * (ItemList[0].rect.width + HLG.spacing)),
        //    contentPanelTransform.localPosition.y,
        //    contentPanelTransform.localPosition.z);
    }

    private void Update()
    {
        if (doScrolling == false)
            return;
        if (isUpdated)
        {
            isUpdated = false;
            scrollRect.velocity = OldVelocity;
        }
        if(direction.x > 0)
        {
            if (contentPanelTransform.localPosition.x > 0)
            {
                Canvas.ForceUpdateCanvases();
                OldVelocity = scrollRect.velocity;
                contentPanelTransform.localPosition -= new Vector3(ItemList.Length * (ItemList[0].rect.width + HLG.spacing), 0, 0);
                isUpdated = true;
            }
            if (contentPanelTransform.localPosition.x < 0 - (ItemList.Length * (ItemList[0].rect.width + HLG.spacing)))
            {
                Canvas.ForceUpdateCanvases();
                OldVelocity = scrollRect.velocity;
                contentPanelTransform.localPosition += new Vector3(ItemList.Length * (ItemList[0].rect.width + HLG.spacing), 0, 0);
                isUpdated = true;
            }
        }
        else if (direction.y > 0)
        {
            if (contentPanelTransform.localPosition.y > 0 + (ItemList.Length * (ItemList[0].rect.height + HLG.spacing)))
            {
                Canvas.ForceUpdateCanvases();
                OldVelocity = scrollRect.velocity;
                contentPanelTransform.localPosition -= new Vector3(0, ItemList.Length * (ItemList[0].rect.height + HLG.spacing), 0);
                isUpdated = true;
            }
            if (contentPanelTransform.localPosition.y < 0)
            {
                Canvas.ForceUpdateCanvases();
                OldVelocity = scrollRect.velocity;
                contentPanelTransform.localPosition += new Vector3(0, ItemList.Length * (ItemList[0].rect.height + HLG.spacing), 0);
                isUpdated = true;
            }
        }
        if(scrollRect.velocity.sqrMagnitude <= 1000f && MovementManager.Instance.moveStarted == false)
        {
            StopScrolling();
        }
    }

    public void ClearRemainings()
    {
        foreach (Transform item in contentPanelTransform)
        {
            Destroy(item.gameObject);
        }
        viewPortTransform.localPosition = startPosOfViewPortTransform;

    }
    public void SetInteraction(bool sts) => Array.ForEach(raycastObjects, x => x.raycastTarget = sts);
}
