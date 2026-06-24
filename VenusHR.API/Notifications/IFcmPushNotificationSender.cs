namespace VenusHR.API.Notifications
{
    public interface IFcmPushNotificationSender
    {
        Task<bool> SendAsync(IEnumerable<string> tokens, string title, string body, IDictionary<string, string>? data, CancellationToken cancellationToken);
    }
}
