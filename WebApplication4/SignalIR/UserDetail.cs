using System;


namespace WebApplication4.SignalIR
{
    public class UserDetail
    {
        public string ConnectionId { get; set; }
        public Guid UserId { get; set; }
        public Guid UserProfileId { get; set; }
        public string UserName { get; set; }
    }
}