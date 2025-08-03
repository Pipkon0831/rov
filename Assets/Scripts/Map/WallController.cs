using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class WallController : MonoBehaviour
{
    [Header("碰撞设置")]
    [SerializeField] private LayerMask _cellLayer = 1 << 6;
    [SerializeField] private Color _gizmoColor = new Color(1, 0, 0, 0.3f);

    private BoxCollider2D _collider;
    private Vector3 _lastPosition;
    private Vector3 _lastScale;
    private Quaternion _lastRotation;
    private HashSet<MyCell> _blockedCells = new HashSet<MyCell>();

    void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        _lastPosition = transform.position;
        _lastScale = transform.lossyScale;
        _lastRotation = transform.rotation;
        UpdateBlockedCells();
    }

    void Update()
    {
        if (transform.hasChanged)
        {
            UpdateBlockedCells();
            _lastPosition = transform.position;
            _lastScale = transform.lossyScale;
            _lastRotation = transform.rotation;
            transform.hasChanged = false;
        }
    }

    void UpdateBlockedCells()
    {
        // 计算精确碰撞区域
        Vector2 center = transform.TransformPoint(_collider.offset);
        Vector2 size = Vector2.Scale(_collider.size, transform.lossyScale);
        float angle = transform.eulerAngles.z;

        // 获取当前覆盖的单元格
        var hits = Physics2D.OverlapBoxAll(center, size, angle, _cellLayer);
        var newBlocks = new HashSet<MyCell>();

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out MyCell cell))
            {
                newBlocks.Add(cell);
                if (!_blockedCells.Contains(cell))
                {
                    cell.SetPermanentWall();
                }
            }
        }

        // 清理不再覆盖的单元格
        foreach (var oldCell in _blockedCells)
        {
            if (!newBlocks.Contains(oldCell))
            {
                oldCell?.ClearPermanentWall();
            }
        }

        _blockedCells = newBlocks;
    }

    void OnDestroy()
    {
        foreach (var cell in _blockedCells)
        {
            cell?.ClearPermanentWall();
        }
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