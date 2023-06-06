using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace DeliveryBro.Services
{
    public class BasedUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(ClaimTypes.Role)?.Value!;
        }
    }
}
