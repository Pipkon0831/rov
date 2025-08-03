using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MyCell : MonoBehaviour
{
    // 修改静态列表初始化方式，并添加场景重置逻辑
    private static List<MyCell> _activeCells = new List<MyCell>();
    public static IReadOnlyList<MyCell> ActiveCells => _activeCells.AsReadOnly();

    [Header("颜色设置")]
    [SerializeField] private Color activeColor = Color.blue;
    [SerializeField] private Color pathColor = Color.white;
    [SerializeField] private Color wallColor = Color.red;

    [Header("阻挡状态")]
    [SerializeField] private bool permanentlyBlocked = false;

    [Header("A* 参数")]
    [HideInInspector] public float G;
    [HideInInspector] public float H;
    [HideInInspector] public MyCell CameFrom;
    [HideInInspector] public Vector2Int GridPosition;

    private SpriteRenderer _renderer;

    public int CellNumber { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsBlocked { get; private set; }
    public float F => G + H;

    // 确保场景重新加载时重置静态数据
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatic()
    {
        _activeCells.Clear();
    }

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        
        if (!TryGetComponent<BoxCollider2D>(out _))
        {
            gameObject.AddComponent<BoxCollider2D>();
        }
    }

    void OnDestroy()
    {
        // 确保对象销毁时从列表中移除
        _activeCells.Remove(this);
    }

    public void Initialize(int number, int gridX, int gridY)
    {
        CellNumber = number;
        GridPosition = new Vector2Int(gridX, gridY);
        ResetToDefaultState();
    }

    public void SetAsPath()
    {
        if (permanentlyBlocked) return;
        UpdateState(IsActive, false);
        UpdateVisual();
    }

    public void SetAsWall()
    {
        if (permanentlyBlocked) return;
        UpdateState(false, true);
        UpdateVisual();
    }

    public void SetPermanentWall()
    {
        UpdateState(false, true);
        permanentlyBlocked = true;
        UpdateVisual();
    }

    public void ClearPermanentWall()
    {
        permanentlyBlocked = false;
        UpdateState(IsActive, false);
        UpdateVisual();
    }

    public void SetAsStartPoint()
    {
        ClearAllActive();
        UpdateState(true, false);
        UpdateVisual();
        _activeCells.Add(this);
        
        if (MapController.Instance?.Player != null)
        {
            MapController.Instance.Player.position = transform.position;
        }
    }

    public void SetActive()
    {
        if (IsBlocked || IsActive) return;

        while (_activeCells.Count >= 2)
        {
            var cell = _activeCells.FirstOrDefault();
            cell?.ForceDeactivate();
        }

        UpdateState(true, false);
        _activeCells.Add(this);
        UpdateVisual();
    }

    public void ForceDeactivate()
    {
        UpdateState(false, false);
        _activeCells.Remove(this);
        UpdateVisual();
    }

    public List<MyCell> GetNeighbors()
    {
        var neighbors = new List<MyCell>();
        var map = MapController.Instance;

        CheckAndAddNeighbor(map?.GetCellByGridPos(GridPosition.x + 1, GridPosition.y));
        CheckAndAddNeighbor(map?.GetCellByGridPos(GridPosition.x - 1, GridPosition.y));
        CheckAndAddNeighbor(map?.GetCellByGridPos(GridPosition.x, GridPosition.y + 1));
        CheckAndAddNeighbor(map?.GetCellByGridPos(GridPosition.x, GridPosition.y - 1));

        return neighbors;

        void CheckAndAddNeighbor(MyCell cell)
        {
            if (cell != null && !cell.IsBlocked)
            {
                neighbors.Add(cell);
            }
        }
    }

    public void SetPathVisual(Color color)
    {
        if (!IsActive && !IsBlocked && _renderer != null)
        {
            _renderer.color = color;
        }
    }

    public void ResetVisual()
    {
        if (!IsActive && !IsBlocked && _renderer != null)
        {
            _renderer.color = pathColor;
        }
    }

    public void ResetToDefaultState()
    {
        permanentlyBlocked = false;
        UpdateState(false, false);
        UpdateVisual();
    }

    private void UpdateState(bool active, bool blocked)
    {
        IsActive = active;
        IsBlocked = blocked;
    }

    private void UpdateVisual()
    {
        if (_renderer == null)
        {
            // 确保在编辑器异常销毁时不会报错
            #if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying) return;
            #endif
            
            Debug.LogWarning($"SpriteRenderer缺失: {gameObject.name}");
            return;
        }

        _renderer.color = IsActive ? activeColor :
            IsBlocked ? wallColor : pathColor;
    }

    private void ClearAllActive()
    {
        foreach (var cell in _activeCells.ToArray())
        {
            if (cell != null)
            {
                cell.ForceDeactivate();
            }
        }
        _activeCells.Clear();
    }
}