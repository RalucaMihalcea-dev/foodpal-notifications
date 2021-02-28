using FluentValidation;
using FoodPal.Notifications.Processor.Commands;

namespace FoodPal.Notifications.Validations
{
    public class NotificationUpdatedCommandValidator : InternalValidator<NotificationUpdatedCommand>
    {
        public NotificationUpdatedCommandValidator()
        {
            this.RuleFor(x => x.Id).NotEmpty();
        }
    }
}