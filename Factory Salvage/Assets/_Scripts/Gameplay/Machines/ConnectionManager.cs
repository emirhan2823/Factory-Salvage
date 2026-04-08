using System.Collections.Generic;
using UnityEngine;

namespace FactorySalvage.Gameplay
{
    /// <summary>
    /// Manages creating/removing conveyor connections between machines.
    /// </summary>
    public class ConnectionManager : MonoBehaviour
    {
        #region Fields

        [SerializeField] private GridManager _gridManager;

        private readonly List<ConveyorConnection> _connections = new();
        private ProcessingMachine _pendingSource;

        #endregion

        #region Properties

        public bool HasPendingConnection => _pendingSource != null;
        public IReadOnlyList<ConveyorConnection> Connections => _connections;

        #endregion

        #region Public Methods

        public void StartConnection(ProcessingMachine source)
        {
            _pendingSource = source;
        }

        public void CancelConnection()
        {
            _pendingSource = null;
        }

        public bool CompleteConnection(ProcessingMachine destination)
        {
            if (_pendingSource == null || destination == null) return false;
            if (_pendingSource == destination) return false;

            // Check if connection already exists
            foreach (var conn in _connections)
            {
                if (conn.Source == _pendingSource && conn.Destination == destination)
                {
                    _pendingSource = null;
                    return false;
                }
            }

            // Create connection
            var connGo = new GameObject($"Connection_{_pendingSource.name}→{destination.name}");
            connGo.transform.SetParent(transform);

            var connection = connGo.AddComponent<ConveyorConnection>();
            connection.Initialize(_pendingSource, destination);

            // Add visual
            var visual = connGo.AddComponent<ConveyorVisual>();
            visual.Initialize(_pendingSource.transform, destination.transform);

            _connections.Add(connection);
            _pendingSource = null;
            return true;
        }

        public void RemoveConnection(ConveyorConnection connection)
        {
            _connections.Remove(connection);
            if (connection != null)
            {
                Destroy(connection.gameObject);
            }
        }

        public void RemoveAllConnections(ProcessingMachine machine)
        {
            for (int i = _connections.Count - 1; i >= 0; i--)
            {
                var conn = _connections[i];
                if (conn.Source == machine || conn.Destination == machine)
                {
                    _connections.RemoveAt(i);
                    Destroy(conn.gameObject);
                }
            }
        }

        #endregion
    }
}
