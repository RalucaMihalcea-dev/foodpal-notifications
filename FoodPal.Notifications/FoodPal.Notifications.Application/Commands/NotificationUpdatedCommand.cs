using MediatR;

namespace FoodPal.Notifications.Processor.Commands
{
    public class NotificationUpdatedCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}