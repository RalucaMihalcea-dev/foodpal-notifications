using FoodPal.Notifications.Data.Abstractions;
using FoodPal.Notifications.Domain;
using FoodPal.Notifications.Dto.Intern;
using FoodPal.Notifications.Service;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FoodPal.Notifications.Processor
{
    public class ResendNotificationErrorWorkerService : BackgroundService
    {
        private readonly ILogger<ResendNotificationErrorWorkerService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public ResendNotificationErrorWorkerService(ILogger<ResendNotificationErrorWorkerService> logger, IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _logger = logger;
            this._unitOfWork = unitOfWork;
            this._notificationService = notificationService;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var notifications = this._unitOfWork.GetRepository<Notification>().Find(x => x.Status == Common.Enums.NotificationStatusEnum.Error);
                
                foreach (var notificationModel in notifications)
                {
                    var userModel = await this._unitOfWork.GetRepository<User>().FindByIdAsync(notificationModel.UserId);
                    var notificationServiceDto = new NotificationServiceDto
                    {
                        Body = notificationModel.Message,
                        Email = userModel.Email,
                        Subject = notificationModel.Title,
                        PhoneNo = userModel.PhoneNo
                    };
                    var sent = await this._notificationService.Send(notificationModel.Type, notificationServiceDto);

                    if (sent)
                        _logger.LogInformation("Notification was sent at: {time}", DateTimeOffset.Now);
                }

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(60 * 5000, stoppingToken);
            }
        }
    }
}