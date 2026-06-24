namespace VenusHR.API.Notifications
{
    public interface IOnlineUserTracker
    {
        void AddConnection(int employeeId, string connectionId);
        void RemoveConnection(int employeeId, string connectionId);
        bool IsOnline(int employeeId);
    }
}
