using System.Text;
using System.Text.Json;

namespace VenusHR.API.Notifications
{
    public class FcmPushNotificationSender : IFcmPushNotificationSender
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<FcmPushNotificationSender> _logger;

        public FcmPushNotificationSender(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<FcmPushNotificationSender> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendAsync(IEnumerable<string> tokens, string title, string body, IDictionary<string, string>? data, CancellationToken cancellationToken)
        {
            var tokenList = tokens
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();

            if (!tokenList.Any())
            {
                return false;
            }

            var serverKey = _configuration["Fcm:ServerKey"];
            if (string.IsNullOrWhiteSpace(serverKey))
            {
                _logger.LogWarning("FCM ServerKey is missing. Push notification skipped.");
                return false;
            }

            var endpoint = _configuration["Fcm:Endpoint"] ?? "https://fcm.googleapis.com/fcm/send";
            var client = _httpClientFactory.CreateClient(nameof(FcmPushNotificationSender));
            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"key={serverKey}");

            var atLeastOneSucceeded = false;
            foreach (var token in tokenList)
            {
                var payload = new
                {
                    to = token,
                    notification = new
                    {
                        title,
                        body
                    },
                    data = data ?? new Dictionary<string, string>()
                };

                var json = JsonSerializer.Serialize(payload);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(endpoint, content, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    atLeastOneSucceeded = true;
                }
                else
                {
                    _logger.LogWarning("FCM push failed for one token. Status code: {StatusCode}", response.StatusCode);
                }
            }

            return atLeastOneSucceeded;
        }
    }
}
