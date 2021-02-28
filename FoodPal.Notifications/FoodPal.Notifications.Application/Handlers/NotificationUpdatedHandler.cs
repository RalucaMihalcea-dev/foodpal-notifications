using AutoMapper;
using FluentValidation;
using FoodPal.Notifications.Application.Extensions;
using FoodPal.Notifications.Data.Abstractions;
using FoodPal.Notifications.Domain;
using FoodPal.Notifications.Dto.Intern;
using FoodPal.Notifications.Processor.Commands;
using FoodPal.Notifications.Service;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FoodPal.Notifications.Application.Handlers
{
    public class NotificationUpdatedHandler : IRequestHandler<NotificationUpdatedCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<NotificationUpdatedCommand> _validator;
        private readonly INotificationService _notificationService;

        public NotificationUpdatedHandler(IUnitOfWork unitOfWork, IMapper mapper, IValidator<NotificationUpdatedCommand> validator, INotificationService notificationService)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._validator = validator;
            this._notificationService = notificationService;
        }

        public async Task<bool> Handle(NotificationUpdatedCommand request, CancellationToken cancellationToken)
        {
            var notificationModel = this._mapper.Map<Notification>(request);

            this._validator.ValidateAndThrowEx(request);

            var notif = await this._unitOfWork.GetRepository<Notification>().FindByIdAsync(notificationModel.Id);
            notif.Status = Common.Enums.NotificationStatusEnum.Viewed;

            this._unitOfWork.GetRepository<Notification>().Update(notif);
            var updated = await this._unitOfWork.SaveChangesAsnyc();

            var sent = false;
            var userModel = await this._unitOfWork.GetRepository<User>().FindByIdAsync(notif.UserId);
            if (userModel != null)
            {
                var notificationServiceDto = new NotificationServiceDto
                {
                    Body = notif.Message,
                    Email = userModel.Email,
                    Subject = notif.Title,
                    PhoneNo = userModel.PhoneNo
                };
                sent = await this._notificationService.Send(notif.Type, notificationServiceDto);
            }

            return updated && sent;
        }
    }
}