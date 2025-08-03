using UnityEngine.Events;
using UnityEngine;
using System.Collections.Generic;

public class G4_CameraController : MonoBehaviour
{
    private Vector3 _startPosition;
    private Vector3 _endPosition;

    private void Start()
    {
        _startPosition = transform.position;
        _endPosition = _startPosition + new Vector3(12.77f, 0, 0);
    }

    /// 外部调用的输入接收方法
    public void ReceiveInput(int inputNumber)
    {
        if (inputNumber == 0)
        {
            TurnLeft();
        }
        else
        {
            TurnRight();
        }
    }

    public void TurnLeft()
    {
        transform.position = _startPosition;
    }
    
    public void TurnRight()
    {
        transform.position = _endPosition;
    }
}