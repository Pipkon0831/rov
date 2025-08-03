using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICameraMovment : MonoBehaviour
{
    public GameObject MainCamera;

    private Vector3 _startPosition;
    
    void Start()
    {
        _startPosition = transform.position;
    }

    public void ToMainCamera()
    {
        transform.position = MainCamera.transform.position;
    }

    public void StartPosition()
    {
        transform.position = _startPosition;
    }
}
