using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class PathController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask _cellLayer = 1 << 6;
    [SerializeField] private Color _gizmoColor = new Color(0, 1, 0, 0.3f);

    private BoxCollider2D _collider;
    private Vector3 _lastPosition;
    private Vector3 _lastScale;
    private Quaternion _lastRotation;
    private HashSet<MyCell> _affectedCells = new HashSet<MyCell>();
    private Dictionary<MyCell, bool> _originalStates = new Dictionary<MyCell, bool>();

    void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        _lastPosition = transform.position;
        _lastScale = transform.lossyScale;
        _lastRotation = transform.rotation;
        UpdatePathCells();
    }

    void Update()
    {
        if (transform.hasChanged)
        {
            RevertOriginalStates();
            UpdatePathCells();
            _lastPosition = transform.position;
            _lastScale = transform.lossyScale;
            _lastRotation = transform.rotation;
            transform.hasChanged = false;
        }
    }

    void UpdatePathCells()
    {
        // 计算精确碰撞区域
        Vector2 center = transform.TransformPoint(_collider.offset);
        Vector2 size = Vector2.Scale(_collider.size, transform.lossyScale);
        float angle = transform.eulerAngles.z;

        var hits = Physics2D.OverlapBoxAll(center, size, angle, _cellLayer);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out MyCell cell))
            {
                // 记录原始状态
                if (!_originalStates.ContainsKey(cell))
                {
                    _originalStates[cell] = cell.IsBlocked;
                }

                // 强制设置为路径
                if (cell.IsBlocked)
                {
                    cell.SetAsPath();
                    cell.ClearPermanentWall();
                }
                
                _affectedCells.Add(cell);
            }
        }
    }

    void RevertOriginalStates()
    {
        foreach (var cell in _affectedCells)
        {
            if (cell == null) continue;

            // 恢复原始状态
            if (_originalStates.TryGetValue(cell, out bool originalState))
            {
                if (originalState)
                {
                    cell.SetAsWall();
                }
                else
                {
                    cell.SetAsPath();
                }
            }
        }
        
        _affectedCells.Clear();
        _originalStates.Clear();
    }

    void OnDestroy()
    {
        RevertOriginalStates();
    }

    void OnDrawGizmosSelected()
    {
        if (_collider == null) _collider = GetComponent<BoxCollider2D>();
        
        Vector2 center = transform.TransformPoint(_collider.offset);
        Vector2 size = Vector2.Scale(_collider.size, transform.lossyScale);

        Gizmos.color = _gizmoColor;
        Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, Vector3.one);
        Gizmos.DrawCube(Vector2.zero, size);
    }
}