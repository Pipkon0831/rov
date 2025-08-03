using UnityEngine.Events;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ButtonPuzzleController : MonoBehaviour
{
    [Tooltip("谜题解谜成功时触发的事件")]
    [SerializeField] private UnityEvent onPuzzleSolved;
    
    [Tooltip("谜题输入错误时触发的事件")]
    [SerializeField] private UnityEvent onPuzzleReset;

    [Tooltip("初始旋转角度（Z轴方向）")]
    [SerializeField] private float initialRotationAngle = -90f;

    private int _currentStep;                   // 当前验证步骤索引
    private float _currentRotationDirection;    // 当前旋转方向
    private int _endNumber = 0;
    private bool _ready = false;

    public Scene NextGame;

    private void Start()
    {
        _currentRotationDirection = initialRotationAngle;
    }

    /// 外部调用的输入接收方法
    public void ReceiveInput(int inputNumber)
    {
        if (inputNumber != _endNumber && _ready)
        {
            RotateObject();
        }

        if (inputNumber == 2)
        {
            _ready = true;
        }
        else
        {
            _endNumber = inputNumber;
            _ready = false;
        }
        
    }

    public void RotateObject()
    {
        
        transform.Rotate(0, 0, _currentRotationDirection, Space.World);
        _currentRotationDirection *= -1;
    }

}