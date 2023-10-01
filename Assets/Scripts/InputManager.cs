using System;
using Unity.VisualScripting;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private float coolDown = 0.1f;
    private float curCoolDown = 0;
    private bool movingStarted;
    private bool directionCalculated;
    private GridSingle curGrid;
    private Vector3 lastPos;

    public Vector3 delta;
    public int direction;

    public event Action onMoveEnded;
    #region Singleton
    public static InputManager Instance;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
    }
    #endregion

    public event Action onMouseButtonDown;
    public event Action onMouseButton;
    public event Action onMouseButtonUp;


    private void Update()
    {
        //if (Input.GetMouseButtonUp(0))
        //    OnMoveEnded();

        //CheckMovingDirection();
        
        
        if (Input.GetMouseButtonDown(0))
            onMouseButtonDown?.Invoke();

        if(Input.GetMouseButton(0))
            onMouseButton?.Invoke();

        if (Input.GetMouseButtonUp(0))
            onMouseButtonUp?.Invoke();


    }
    //public void OnMouseStartMoving(GridSingle gridPoint)
    //{
    //    if (movingStarted)
    //        return;

    //    movingStarted = true;
    //    curGrid = gridPoint;
    //    lastPos = Input.mousePosition;
    //}

}
