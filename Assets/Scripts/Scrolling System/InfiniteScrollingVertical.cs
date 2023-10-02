using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfiniteScrollingVertical : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform viewPortTransform;
    public RectTransform contentPanelTransform;
    public VerticalLayoutGroup VLG;
    public RectTransform[] ItemList;
    Vector2 OldVelocity;
    bool isUpdated;




    private void Start()
    {
        isUpdated = false;
        OldVelocity = Vector2.zero;
        int ItemsToAdd = Mathf.CeilToInt(viewPortTransform.rect.height / (ItemList[0].rect.height + VLG.spacing));
        //int ItemsToAdd = (int)(viewPortTransform.rect.width / (ItemList[0].rect.width + HLG.spacing));

        for (int i = 0; i < ItemsToAdd; i++)
        {
            RectTransform RT = Instantiate(ItemList[i % ItemList.Length], contentPanelTransform);
            RT.SetAsLastSibling();
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
        }
        contentPanelTransform.localPosition = new Vector3(contentPanelTransform.localPosition.x,
            (1 - (ItemList[0].rect.height + VLG.spacing) * ItemsToAdd),
            contentPanelTransform.localPosition.z);
    }

    private void Update()
    {
        if (isUpdated)
        {
            isUpdated = false;
            scrollRect.velocity = OldVelocity;
        }
        if (contentPanelTransform.localPosition.y > 0 + (ItemList.Length * (ItemList[0].rect.height + VLG.spacing)))
        {
            Canvas.ForceUpdateCanvases();
            OldVelocity = scrollRect.velocity;
            contentPanelTransform.localPosition -= new Vector3(0, ItemList.Length * (ItemList[0].rect.height + VLG.spacing), 0);
            isUpdated = true;
        }
        if (contentPanelTransform.localPosition.y < 0 )
        {
            Canvas.ForceUpdateCanvases();
            OldVelocity = scrollRect.velocity;
            contentPanelTransform.localPosition += new Vector3(0, ItemList.Length * (ItemList[0].rect.height + VLG.spacing), 0);
            isUpdated = true;
        }
    }

}
