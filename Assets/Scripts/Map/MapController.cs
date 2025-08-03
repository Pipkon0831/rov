using UnityEngine;
using System.Collections.Generic;

public class MapController : MonoBehaviour
{
    public static MapController Instance { get; private set; }
    
    [Header("Player Settings")]
    [SerializeField] public Transform Player;

    [Header("Map Generation")]
    [SerializeField] private GameObject _mapCellPrefab;
    [SerializeField] public int columns = 10;
    [SerializeField] public int rows = 10;
    [SerializeField] public float spacing = 0.1f;
    [SerializeField] public float cellSize = 1f;
    [SerializeField] public int startCellNumber = 0;
    private bool IsAllRoad = false;

    [Header("传送门设置")]
    public List<TeleportGroup> teleportGroups = new List<TeleportGroup>();

    public List<MyCell> Cells { get; private set; } = new List<MyCell>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        GenerateGrid();
    }

    void Start()
    {
        InitializeStartCell();
    }

    void GenerateGrid()
    {
        Cells.Clear();
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Vector3 position = new Vector3(
                    x * (cellSize + spacing) - (columns - 1) * (cellSize + spacing) / 2,
                    y * (cellSize + spacing) - (rows - 1) * (cellSize + spacing) / 2,
                    0
                );

                GameObject cellObj = Instantiate(_mapCellPrefab, position, Quaternion.identity, transform);
                cellObj.transform.localScale = Vector3.one * cellSize;

                MyCell cell = cellObj.GetComponent<MyCell>();
                cell.Initialize(y * columns + x, x, y);
                
                if (IsAllRoad) cell.SetAsPath();
                else cell.SetAsWall();
                
                Cells.Add(cell);
            }
        }
    }

    // 唯一正确的传送检测方法
    public void CheckTeleport(MyCell cell, PlayerController player)
    {
        foreach (var group in teleportGroups)
        {
            // 跳过已使用的一次性传送门
            if (group.isOneTime && group.hasBeenUsed)
                continue;

            if (group.cellNumbers.Contains(cell.CellNumber))
            {
                int currentIndex = group.cellNumbers.IndexOf(cell.CellNumber);
                int nextIndex = (currentIndex + 1) % group.cellNumbers.Count;
                int targetCellNumber = group.cellNumbers[nextIndex];
                MyCell targetCell = GetCell(targetCellNumber);
                
                if (targetCell != null && !targetCell.IsBlocked && targetCell != cell)
                {
                    // 标记一次性传送门为已使用
                    if (group.isOneTime)
                    {
                        group.hasBeenUsed = true;
                        // 可在此处添加禁用传送门特效的代码
                        // cell.DisableTeleportEffect();
                    }
                    
                    player.TeleportTo(targetCell);
                    player.SetCurrentCell(targetCell);
                    break;
                }
            }
        }
    }

    // 重置所有传送门状态
    public void ResetTeleporters()
    {
        foreach (var group in teleportGroups)
        {
            group.hasBeenUsed = false;
            // 可在此处添加重置传送门特效的代码
        }
    }

    public MyCell GetCell(int number) => Cells.Find(c => c.CellNumber == number);

    public MyCell GetCellByGridPos(int x, int y)
    {
        if (x < 0 || x >= columns || y < 0 || y >= rows) return null;
        return Cells[y * columns + x];
    }

    void InitializeStartCell()
    {
        startCellNumber = Mathf.Clamp(startCellNumber, 0, columns * rows - 1);
        MyCell startCell = GetCell(startCellNumber);
        if (startCell)
        {
            startCell.SetAsPath();
            startCell.SetAsStartPoint();
            
            if (Player != null)
            {
                Player.position = startCell.transform.position;
                Player.GetComponent<PlayerController>().SnapToStartPoint();
            }
        }
    }
}

[System.Serializable]
public class TeleportGroup
{
    [Tooltip("属于这个传送门组的单元格编号")]
    public List<int> cellNumbers;
    
    [Tooltip("是否使用后立即失效")]
    public bool isOneTime = false;

    [System.NonSerialized] // 不需要序列化保存状态
    public bool hasBeenUsed; // 运行时记录使用状态
}