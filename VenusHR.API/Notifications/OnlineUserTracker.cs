using System.Collections.Concurrent;

namespace VenusHR.API.Notifications
{
    public class OnlineUserTracker : IOnlineUserTracker
    {
        private readonly ConcurrentDictionary<int, ConcurrentDictionary<string, byte>> _connections = new();

        public void AddConnection(int employeeId, string connectionId)
        {
            var userConnections = _connections.GetOrAdd(employeeId, _ => new ConcurrentDictionary<string, byte>());
            userConnections.TryAdd(connectionId, 0);
        }

        public void RemoveConnection(int employeeId, string connectionId)
        {
            if (!_connections.TryGetValue(employeeId, out var userConnections))
            {
                return;
            }

            userConnections.TryRemove(connectionId, out _);
            if (userConnections.IsEmpty)
            {
                _connections.TryRemove(employeeId, out _);
            }
        }

        public bool IsOnline(int employeeId)
        {
            return _connections.TryGetValue(employeeId, out var userConnections) && !userConnections.IsEmpty;
        }
    }
}
