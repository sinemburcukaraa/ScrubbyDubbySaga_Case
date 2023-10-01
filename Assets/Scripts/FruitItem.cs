using UnityEngine;


public class FruitItem : MonoBehaviour
{
    public int myId;

    public Vector2Int myPos;
    public RectTransform myRect;

    private void Awake()
    {
        myRect = GetComponent<RectTransform>();
    }
}