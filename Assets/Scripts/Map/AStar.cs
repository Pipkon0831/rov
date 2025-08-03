using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AStar
{
    public static List<MyCell> FindPath(MyCell start, MyCell end)
    {
        var openSet = new List<MyCell> { start };
        var closedSet = new HashSet<MyCell>();

        foreach (var cell in MapController.Instance.Cells)
        {
            cell.G = float.MaxValue;
            cell.H = 0;
            cell.CameFrom = null;
        }

        start.G = 0;
        start.H = Heuristic(start, end);

        while (openSet.Count > 0)
        {
            var current = openSet.OrderBy(c => c.F).First();

            if (current == end)
                return ReconstructPath(current);

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (var neighbor in current.GetNeighbors())
            {
                if (closedSet.Contains(neighbor)) continue;

                var tentativeG = current.G + Vector2Int.Distance(
                    current.GridPosition, 
                    neighbor.GridPosition
                );

                if (tentativeG < neighbor.G)
                {
                    neighbor.CameFrom = current;
                    neighbor.G = tentativeG;
                    neighbor.H = Heuristic(neighbor, end);

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }
        return null;
    }

    private static float Heuristic(MyCell a, MyCell b)
    {
        return Mathf.Abs(a.GridPosition.x - b.GridPosition.x) + 
               Mathf.Abs(a.GridPosition.y - b.GridPosition.y);
    }

    private static List<MyCell> ReconstructPath(MyCell end)
    {
        var path = new List<MyCell>();
        while (end != null)
        {
            path.Add(end);
            end = end.CameFrom;
        }
        path.Reverse();
        return path;
    }
}