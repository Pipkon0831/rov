using UnityEngine;
using System.Collections.Generic;

public class MouseController : MonoBehaviour
{
    [Header("Path Display")]
    [SerializeField] private Color pathColor = Color.green;

    private List<MyCell> _currentPath = new List<MyCell>();
    private PlayerController _playerController;

    void Start()
    {
        _playerController = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        if (!_playerController.IsMoving && Input.GetMouseButtonDown(0))
        {
            var cell = GetCellUnderMouse();
            if (cell != null && !cell.IsBlocked)
            {
                ProcessCellClick(cell);
            }
        }
    }

    MyCell GetCellUnderMouse()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        return hit.collider?.GetComponent<MyCell>();
    }

    void ProcessCellClick(MyCell endCell)
    {
        ClearPathVisual();

        MyCell startCell = _playerController._currentCell;
        if (startCell == null)
        {
            Debug.LogWarning("Player has no starting position!");
            return;
        }

        if (startCell == endCell)
        {
            Debug.Log("Start and end are the same!");
            return;
        }

        var path = AStar.FindPath(startCell, endCell);
        if (path == null || path.Count == 0)
        {
            Debug.LogWarning("Path is unreachable!");
            return;
        }

        ShowValidPath(path);
    }

    void ShowValidPath(List<MyCell> path)
    {
        _currentPath = path;
        
        foreach (var cell in _currentPath)
        {
            if (!cell.IsActive)
                cell.SetPathVisual(pathColor);
        }

        _playerController.MoveToCell(path[path.Count - 1]);
    }

    public void ClearPathVisual()
    {
        foreach (var cell in _currentPath)
        {
            if (cell != null && !cell.IsActive && !cell.IsBlocked)
            {
                cell.ResetVisual();
            }
        }
        _currentPath.Clear();
    }

    void HandleInvalidPath(MyCell endCell)
    {
        endCell.ForceDeactivate();
        Debug.Log("Path invalid, selection canceled");
        ClearPathVisual();
    }
}