using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] public int onceStep = 5;

    [Header("Object References")]
    [SerializeField] private GameObject R1;
    [SerializeField] private GameObject R2;

    private Coroutine _moveCoroutine;
    private MouseController _mouseController;
    public MyCell _currentCell;
    private bool _canTeleport = true;
    public List<MyCell> _recentCells = new List<MyCell>();
    public bool IsMoving { get; private set; }
    
    // 新增字段：目标终点单元格
    private MyCell _targetEndCell;

    void Start()
    {
        _mouseController = FindObjectOfType<MouseController>();
        SnapToStartPoint();
    }

    public void SnapToStartPoint()
    {
        if (MapController.Instance?.Player != null)
        {
            transform.position = MapController.Instance.Player.position;
        }
    }

    // 新增方法：启动逐步移动
    public void MoveToCell(MyCell endCell)
    {
        if (endCell == null || endCell.IsBlocked || endCell == _currentCell)
            return;

        _targetEndCell = endCell;

        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);

        _moveCoroutine = StartCoroutine(MoveStepByStep());
    }

    // 修改后的移动协程：每次移动一格并重新寻路
    private IEnumerator MoveStepByStep()
    {
        IsMoving = true;
        int temp = onceStep;
        
        while (true)
        {
            if(temp == 0) break;
            if (_currentCell == null || _targetEndCell == null)
                break;

            // 获取当前路径
            var path = AStar.FindPath(_currentCell, _targetEndCell);
            if (path == null || path.Count < 2)
            {
                // 路径不可达或已到达终点
                if (path != null && path.Count == 1)
                {
                    // 更新当前单元格为终点
                    SetCurrentCell(_targetEndCell);
                }
                break;
            }

            // 获取下一个移动目标（跳过当前所在单元格）
            MyCell nextCell = path[1];

            // 移动到一个单元格
            Vector3 targetPos = nextCell.transform.position;
            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position, 
                    targetPos, 
                    moveSpeed * Time.deltaTime
                );
                yield return null;
            }
            transform.position = targetPos;

            // 更新当前单元格并记录
            SetCurrentCell(nextCell);
            AddRecentCell(nextCell);

            // 到达终点则停止
            if (nextCell == _targetEndCell) break;
            temp--;
        }

        IsMoving = false;
        _mouseController.ClearPathVisual();
    }

    private void AddRecentCell(MyCell cell)
    {
        _recentCells.Insert(0, cell);
        if (_recentCells.Count > 3)
        {
            _recentCells.RemoveAt(3);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Button")
        {
            HandleButtonTrigger();
        }
        else if (_canTeleport)
        {
            HandleCellTrigger(other);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        MyCell exitedCell = other.GetComponent<MyCell>();
        if (exitedCell != null && exitedCell == _currentCell)
        {
            _currentCell = null;
            _canTeleport = true;
        }
    }

    private void HandleButtonTrigger()
    {
        if (_recentCells.Count >= 3 && 
            _recentCells[0] != null && 
            _recentCells[2] != null && 
            _recentCells[0] == _recentCells[2])
        {
            if (R1 != null)
            {
                var rotateR1 = R1.GetComponent<RightClickRotate>();
                rotateR1?.RotateObject();
            }

            if (R2 != null)
            {
                var rotateR2 = R2.GetComponent<RightClickRotate>();
                rotateR2?.RotateObject();
            }
        }
    }

    private void HandleCellTrigger(Collider2D other)
    {
        MyCell newCell = other.GetComponent<MyCell>();
        if (newCell != null && newCell != _currentCell)
        {
            _currentCell = newCell;
            MapController.Instance.CheckTeleport(newCell, this);
        }
    }

    public void TeleportTo(MyCell targetCell)
    {
        if (!_canTeleport) return;

        _canTeleport = false;
        
        if (IsMoving)
        {
            if (_moveCoroutine != null)
                StopCoroutine(_moveCoroutine);
            IsMoving = false;
        }
        
        transform.position = targetCell.transform.position;
        SetCurrentCell(targetCell);
        _mouseController.ClearPathVisual();
        
        // 传送后如果还有目标终点且未到达，则继续移动
        if (_targetEndCell != null && targetCell != _targetEndCell)
        {
            _moveCoroutine = StartCoroutine(MoveStepByStep());
        }
    }
    
    public void SetCurrentCell(MyCell cell)
    {
        _currentCell = cell;
        if (cell != null)
        {
            cell.SetAsStartPoint();
        }
    }
}