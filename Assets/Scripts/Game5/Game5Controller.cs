using System;
using UnityEngine.Events;
using UnityEngine;
using System.Collections.Generic;

public class Game5Controller : MonoBehaviour
{
    public GameObject Door;
    public DiaLogmanager DM;
    
    [Tooltip("正确的按钮触发顺序")]
    [SerializeField] private List<int> correctSequence = new List<int>() { 1, 2, 3 , 4, 5, 6, 7, 8, 9, 10, 11};

    [Tooltip("谜题解谜成功时触发的事件")]
    [SerializeField] private UnityEvent onPuzzleSolved;
    
    [Tooltip("谜题输入错误时触发的事件")]
    [SerializeField] private UnityEvent onPuzzleReset;

    private int _currentStep;                   // 当前验证步骤索引

    private void Awake()
    {
        DM = FindObjectOfType<DiaLogmanager>();
    }

    /// 外部调用的输入接收方法
    public void ReceiveInput(int inputNumber)
    {
        if (_currentStep >= correctSequence.Count) return;

        if (inputNumber == correctSequence[_currentStep])
        {
            HandleCorrectInput();
        }
        else if (inputNumber == correctSequence[0])
        {
            _currentStep = 1;
            onPuzzleReset?.Invoke();
        }
        else
        {
            HandleWrongInput();
        }
    }

    private void HandleCorrectInput()
    {
        _currentStep++;
        
        if (_currentStep == correctSequence.Count)
        {
            RotateObject();
            Debug.Log("你过关");
            Door.transform.position = new Vector3(4.2f, -3.3f, 0);
            onPuzzleSolved?.Invoke();
            ResetPuzzle();
        }
    }

    private void HandleWrongInput()
    {
        Debug.Log($"输入错误，关卡步骤重置");
        DM.EnterGame();
        ResetPuzzle();
    }

    private void ResetPuzzle()
    {
        _currentStep = 0;
        onPuzzleReset?.Invoke();
    }

    public void RotateObject()
    {
        
    }

    // 编辑器调试用方法
    [ContextMenu("测试正确顺序")]
    private void TestCorrectSequence()
    {
        foreach (var num in correctSequence)
        {
            ReceiveInput(num);
        }
    }
}