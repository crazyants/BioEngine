﻿using System;
using System.Threading.Tasks;
using BioEngine.Data.Core;
using BioEngine.Data.News.Commands;
using BioEngine.Data.Search.Commands;
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

        protected override async Task<bool> ExecuteCommandAsync(UpdateNewsCommand command)
        {
            var needSocialUpd = command.Model.Pub == 1 &&
                                (command.Title != command.Model.Title || command.Url != command.Model.Url);

            Mapper.Map(command, command.Model);

            if (command.Model.Pub == 1)
            {
                await Mediator.Publish(new CreateOrUpdateNewsForumTopicCommand(command.Model));
            }

            command.Model.LastChangeDate = DateTimeOffset.Now.ToUnixTimeSeconds();

            DBContext.Update(command.Model);
            await DBContext.SaveChangesAsync();

            if (command.Model.Pub == 1)
            {
                await Mediator.Send(new PublishNewsToSocialCommand(command.Model, needSocialUpd));
                await Mediator.Publish(new IndexEntityCommand<Common.Models.News>(command.Model));
            }

            return true;
        }
    }
}