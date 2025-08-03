using UnityEngine;
using System.Collections;

[RequireComponent(typeof(WallController))]
public class WallMovementController : MonoBehaviour
{
    [Header("移动设置")]
    [SerializeField] private float _moveStep = 1f;    // 每次移动的步长
    [SerializeField] private float _rotateSpeed = 90f; // 旋转速度（度/秒）

    private WallController _wallController;
    private bool _isRotating;

    void Start()
    {
        _wallController = GetComponent<WallController>();
    }

    void Update()
    {
        HandleMovementInput();
        HandleRotationInput();
    }

    void HandleMovementInput()
    {
        Vector3 moveDir = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.W)) moveDir = Vector3.up;
        if (Input.GetKeyDown(KeyCode.S)) moveDir = Vector3.down;
        if (Input.GetKeyDown(KeyCode.A)) moveDir = Vector3.left;
        if (Input.GetKeyDown(KeyCode.D)) moveDir = Vector3.right;

        if (moveDir != Vector3.zero)
        {
            transform.position += moveDir * _moveStep;
        }
    }

    void HandleRotationInput()
    {
        if (_isRotating) return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(RotateWall(-90f));
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(RotateWall(90f));
        }
    }

    IEnumerator RotateWall(float degrees)
    {
        _isRotating = true;
        
        float currentRotation = 0f;
        float targetRotation = Mathf.Abs(degrees);
        int direction = (int)Mathf.Sign(degrees);

        while (currentRotation < targetRotation)
        {
            float rotateAmount = Mathf.Min(_rotateSpeed * Time.deltaTime, targetRotation - currentRotation);
            transform.Rotate(0f, 0f, rotateAmount * direction);
            currentRotation += rotateAmount;
            yield return null;
        }

        _isRotating = false;
    }
}