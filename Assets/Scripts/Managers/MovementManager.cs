using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    #region Singleton
    public static MovementManager Instance;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
    }
    #endregion

    //public bool moveStarted;
    public float speed = 1;

    public bool moveEndedCOStarted;

    public bool moveStarted;
    public bool isHoveringGrid;
    public bool directionCalculated;
    FruitItem selectedItem;
    [SerializeField] private float coolDown = 0.1f;
    private float curCoolDown = 0;
    private Vector3 lastPos;
    private Vector2Int direction;
    float moveSpeed = 10;
    List<FruitItem> itemsToMove = new List<FruitItem>();
    List<Vector3> offsets = new List<Vector3>();
    private int startedCount;

    public InfiniteScroll scroll;

    private void Start()
    {
        InputManager.Instance.onMouseButtonDown += InitializeMovement;
        InputManager.Instance.onMouseButton += CheckMovingDirection;
        InputManager.Instance.onMouseButtonUp += OnMoveEnded;

    }

    private void InitializeMovement()
    {
        if (moveEndedCOStarted == true)
            return;
        if (this.itemsToMove.Count > 0)
            return;


        lastPos = Input.mousePosition;
        curCoolDown = 0;
    }

    private void CheckMovingDirection()
    {
        if (moveEndedCOStarted == true)
            return;

        if (this.itemsToMove.Count > 0)
            return;


        if (directionCalculated)
            return;

        curCoolDown += Time.deltaTime;
        if (curCoolDown > coolDown)
        {
            CalculateMovementDirection();
            curCoolDown = 0;
        }
    }

    private void CalculateMovementDirection()
    {
        directionCalculated = true;

        Vector3 delta = lastPos - Input.mousePosition;
        float a = Mathf.Max(Mathf.Abs(delta.x), Mathf.Abs(delta.y));

        //x == 1 means Horizontal
        //y == 1 means Vertical
        if (Mathf.Abs(delta.x) == a)
        {
            direction.x = 1;
            direction.y = 0;
        }
        else if (Mathf.Abs(delta.y) == a)
        {
            direction.x = 0;
            direction.y = 1;
        }

        StartMovement();

    }

    private void StartMovement()
    {

        if (moveEndedCOStarted == true)
            return;

        if (this.itemsToMove.Count > 0)
            return;

        moveStarted = true;


        Vector2Int selectedItemPos = GridManager.Instance.grid.GetTileGridPosition(Input.mousePosition); //Get selected grid
        List<FruitItem> itemsToMove = new List<FruitItem>(); // The list to select which objects to move

        if (direction.x > 0) // If direction is Horizontal
        {
            int width = GridManager.Instance.grid.fixedWidth;
            for (int i = 0; i < width; i++) // Get All grids where the grids' y pos is equals to selected grid's y pos
            {
                if (i >= GridManager.Instance.grid.fruitItemSlot.GetLength(0) ||
                    selectedItemPos.y >= GridManager.Instance.grid.fruitItemSlot.GetLength(1))
                    return;
                if (GridManager.Instance.grid.fruitItemSlot[i, selectedItemPos.y] == null)
                    continue;
                itemsToMove.Add(GridManager.Instance.grid.fruitItemSlot[i, selectedItemPos.y]);
            }
        }
        else if (direction.y > 0) // If direction is Vertical
        {
            int height = GridManager.Instance.grid.fixedHeight;
            for (int i = 0; i < height; i++) // Get All grids where the grids' x pos is equals to selected grid's x pos
            {
                if (i >= GridManager.Instance.grid.fruitItemSlot.GetLength(1) ||
                    selectedItemPos.x >= GridManager.Instance.grid.fruitItemSlot.GetLength(0))
                    return;
                if (GridManager.Instance.grid.fruitItemSlot[selectedItemPos.x, i] == null)
                    continue;
                itemsToMove.Add(GridManager.Instance.grid.fruitItemSlot[selectedItemPos.x, i]);
            }
        }
        if (itemsToMove.Count <= 0)
            return;
        this.itemsToMove = itemsToMove;

        InitializeScrolling();
    }

    private void InitializeScrolling()
    {
        RectTransform[] itemsToMoveRect = new RectTransform[itemsToMove.Count];
        for (int i = 0; i < itemsToMove.Count; i++)
        {
            itemsToMoveRect[i] = itemsToMove[i].myRect;
        }

        scroll.StartScrolling(itemsToMoveRect, direction);
    }

    private void GetOffsetAndPickUpFromGrid(FruitItem item)
    {
        Vector3 offset = item.transform.position - Input.mousePosition;
        offsets.Add(offset);

        GridManager.Instance.grid.PickupItem(item.myPos.x, item.myPos.y);
    }

    private void OnMoveEnded()//mouse
    {
        if (moveEndedCOStarted == true)
            return;
 
        moveStarted = false;
        directionCalculated = false;
        curCoolDown = 0;
        offsets.Clear();
        scroll.SetInteraction(false);
        if (itemsToMove.Count <= 0)
            return;
        StartCoroutine(OnMoveEndedCoroutine());
    }


    private IEnumerator OnMoveEndedCoroutine()
    {
        moveEndedCOStarted = true;
        itemsToMove.Clear();
        while (scroll.doScrolling)
            yield return null;
        List<FruitItem> items = new List<FruitItem>();
        List<FruitItem> itemsInGrid = new List<FruitItem>();
        List<Transform> itemsNOTInGridT = new List<Transform>();
        List<Vector2Int> tileGridPositions = new List<Vector2Int>();
        List<Vector3> snapPositions = new List<Vector3>();


        scroll.allItemList.ForEach(x => items.Add(x.GetComponent<FruitItem>())); //Initialize a list and get all fruits with fruit component
        for (int i = 0; i < items.Count; i++)
        {
            FruitItem item = items[i];
            if (GridManager.Instance.grid.AmIInGrid(item) == false)
            {
                itemsNOTInGridT.Add(item.transform);
                continue;
            }
            Vector2Int itemPosInGrid = GridManager.Instance.grid.GetTileGridPosition(item.transform.position);
            if (tileGridPositions.Contains(itemPosInGrid))
            {
                itemsNOTInGridT.Add(item.transform);
                continue;
            }
            itemsInGrid.Add(item);

            Vector2 screenPosOfItem = item.transform.position;
            tileGridPositions.Add(GridManager.Instance.grid.GetTileGridPosition(screenPosOfItem));
            snapPositions.Add(GridManager.Instance.grid.GetWorldPosByGridPos(tileGridPositions[tileGridPositions.Count - 1].x, tileGridPositions[tileGridPositions.Count - 1].y));

        }
        scroll.HideUnused(itemsNOTInGridT);

        bool canBreak = false;
        while (!canBreak) // Lerp And 
        {
            for (int i = 0; i < itemsInGrid.Count; i++)
            {
                FruitItem item = itemsInGrid[i];
                item.transform.position = Vector3.Lerp(item.transform.position, snapPositions[i], Time.deltaTime * moveSpeed);
                if (Vector3.Distance(snapPositions[i], item.transform.position) <= 5f)
                    canBreak = true;
            }
            yield return null;
        }
        for (int i = 0; i < itemsInGrid.Count; i++)
        {
            FruitItem item = itemsInGrid[i];
            GridManager.Instance.grid.PlaceItem(item, tileGridPositions[i].x, tileGridPositions[i].y);
        }
        scroll.ClearRemainings();
        GridManager.Instance.matchControl.MatchCheck();
    }

    public void ActivateInput()
    {
        moveEndedCOStarted = false;
        scroll.SetInteraction(true);
    }
}
