public static class UserConnectionMapping
{
    private static readonly Dictionary<int, HashSet<string>> _connections = new();

    public static void Add(int userId, string connectionId)
    {
        lock (_connections)
        {
            if (!_connections.ContainsKey(userId))
                _connections[userId] = new HashSet<string>();
            _connections[userId].Add(connectionId);
        }
    }

    public static void Remove(int userId, string connectionId)
    {
        lock (_connections)
        {
            if (_connections.ContainsKey(userId))
            {
                _connections[userId].Remove(connectionId);
                if (_connections[userId].Count == 0)
                    _connections.Remove(userId);
            }
        }
    }

    public static IEnumerable<string> GetConnections(int userId)
    {
        if (_connections.ContainsKey(userId))
            return _connections[userId];
        return Enumerable.Empty<string>();
    }
}
