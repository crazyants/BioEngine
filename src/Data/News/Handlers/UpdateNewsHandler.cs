﻿using System;
using System.Threading.Tasks;
using BioEngine.Data.Core;
using BioEngine.Data.News.Commands;
using FluentValidation;
using JetBrains.Annotations;

namespace BioEngine.Data.News.Handlers
{
    [UsedImplicitly]
    internal class UpdateNewsHandler : RestCommandHandlerBase<UpdateNewsCommand, bool>
    {
        public UpdateNewsHandler(HandlerContext<UpdateNewsHandler> context, IValidator<UpdateNewsCommand>[] validators)
            : base(context, validators)
        {
        }

        protected override async Task<bool> ExecuteCommand(UpdateNewsCommand command)
        {
            command.LastChangeDate = DateTimeOffset.Now.ToUnixTimeSeconds();

            await Validate(command);

            Mapper.Map(command, command.Model);

            DBContext.Update(command.Model);
            await DBContext.SaveChangesAsync();
            return true;
        }
    }
}