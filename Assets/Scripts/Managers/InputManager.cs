using System;
using Unity.VisualScripting;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    private float _coolDown = 0.1f;
    private float _curCoolDown = 0;
    private bool _movingStarted;
    private bool _directionCalculated;
    private Vector3 _lastPos;

    public Vector3 delta;
    public int direction;

    public event Action onMoveEnded;
    public event Action onMouseButtonDown;
    public event Action onMouseButton;
    public event Action onMouseButtonUp;

    #region Singleton
    public static InputManager Instance;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
    }
    #endregion

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            onMouseButtonDown?.Invoke();

        if (Input.GetMouseButton(0))
            onMouseButton?.Invoke();

        if (Input.GetMouseButtonUp(0))
            onMouseButtonUp?.Invoke();

    }
}
