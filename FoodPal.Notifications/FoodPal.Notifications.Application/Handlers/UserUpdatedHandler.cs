﻿using AutoMapper;
using FluentValidation;
using FoodPal.Notifications.Application.Extensions;
using FoodPal.Notifications.Data.Abstractions;
using FoodPal.Notifications.Domain;
using FoodPal.Notifications.Processor.Commands;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FoodPal.Notifications.Application.Handlers
{
    public class UserUpdatedHandler : IRequestHandler<UserUpdatedCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<UserUpdatedCommand> _validator;

        public UserUpdatedHandler(IUnitOfWork unitOfWork, IMapper mapper, IValidator<UserUpdatedCommand> validator)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._validator = validator;
        }

        public async Task<bool> Handle(UserUpdatedCommand request, CancellationToken cancellationToken)
        {
            var userModel = this._mapper.Map<User>(request);

            this._validator.ValidateAndThrowEx(request);

            this._unitOfWork.GetRepository<User>().Update(userModel);
            return await this._unitOfWork.SaveChangesAsnyc();
        }
    }
}