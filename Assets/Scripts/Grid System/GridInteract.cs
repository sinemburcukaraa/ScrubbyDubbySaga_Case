using UnityEngine;
using UnityEngine.EventSystems;

public class GridInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{


    public void OnPointerEnter(PointerEventData eventData)
    {
        MovementManager.Instance.isHoveringGrid = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MovementManager.Instance.isHoveringGrid = false;

    }
}