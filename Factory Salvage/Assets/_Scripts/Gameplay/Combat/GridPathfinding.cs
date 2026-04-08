using System.Collections.Generic;
using UnityEngine;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// A* pathfinding on the grid. Caches collections to avoid GC.
    /// </summary>
    public static class GridPathfinding
    {
        #region Fields

        // Cached collections to avoid GC allocation
        private static readonly List<Vector2Int> _path = new(64);
        private static readonly List<PathNode> _openList = new(128);
        private static readonly HashSet<Vector2Int> _closedSet = new(128);
        private static readonly Dictionary<Vector2Int, PathNode> _nodeMap = new(128);

        private static readonly Vector2Int[] Neighbors =
        {
            new(0, 1), new(0, -1), new(1, 0), new(-1, 0)
        };

        #endregion

        #region Public Methods

        public static List<Vector2Int> FindPath(Vector2Int start, Vector2Int end, GridManager grid)
        {
            _path.Clear();
            _openList.Clear();
            _closedSet.Clear();
            _nodeMap.Clear();

            if (!grid.IsInBounds(start) || !grid.IsInBounds(end))
                return _path;

            var startNode = new PathNode(start, 0, Heuristic(start, end), null);
            _openList.Add(startNode);
            _nodeMap[start] = startNode;

            while (_openList.Count > 0)
            {
                // Find lowest F cost
                int bestIndex = 0;
                for (int i = 1; i < _openList.Count; i++)
                {
                    if (_openList[i].F < _openList[bestIndex].F)
                        bestIndex = i;
                }

                var current = _openList[bestIndex];
                _openList.RemoveAt(bestIndex);

                if (current.Position == end)
                {
                    ReconstructPath(current);
                    return _path;
                }

                _closedSet.Add(current.Position);

                foreach (var dir in Neighbors)
                {
                    var neighborPos = current.Position + dir;

                    if (_closedSet.Contains(neighborPos)) continue;
                    if (!grid.IsInBounds(neighborPos)) continue;
                    if (!grid.IsCellWalkable(neighborPos) && neighborPos != end) continue;

                    float newG = current.G + 1f;

                    if (_nodeMap.TryGetValue(neighborPos, out var existing))
                    {
                        if (newG < existing.G)
                        {
                            existing.G = newG;
                            existing.Parent = current;
                        }
                    }
                    else
                    {
                        var neighbor = new PathNode(neighborPos, newG, Heuristic(neighborPos, end), current);
                        _openList.Add(neighbor);
                        _nodeMap[neighborPos] = neighbor;
                    }
                }
            }

            return _path; // No path found
        }

        #endregion

        #region Private Methods

        private static float Heuristic(Vector2Int a, Vector2Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        private static void ReconstructPath(PathNode node)
        {
            _path.Clear();
            while (node != null)
            {
                _path.Add(node.Position);
                node = node.Parent;
            }
            _path.Reverse();
        }

        #endregion

        #region PathNode

        private class PathNode
        {
            public Vector2Int Position;
            public float G;
            public float H;
            public float F => G + H;
            public PathNode Parent;

            public PathNode(Vector2Int pos, float g, float h, PathNode parent)
            {
                Position = pos;
                G = g;
                H = h;
                Parent = parent;
            }
        }

        #endregion
    }
}
