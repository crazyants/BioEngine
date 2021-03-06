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
    internal class UnPublishNewsHandler : RestCommandHandlerBase<UnPublishNewsCommand, bool>
    {
        public UnPublishNewsHandler(HandlerContext<UnPublishNewsHandler> context,
            IValidator<UnPublishNewsCommand>[] validators) : base(context, validators)
        {
        }

        protected override async Task<bool> ExecuteCommandAsync(UnPublishNewsCommand command)
        {
            command.Model.LastChangeDate = DateTimeOffset.Now.ToUnixTimeSeconds();
            command.Model.Pub = 0;

            await Mediator.Publish(new CreateOrUpdateNewsForumTopicCommand(command.Model));

            DBContext.Update(command.Model);
            await DBContext.SaveChangesAsync();

            await Mediator.Publish(new DeleteEntityFromIndexCommand<Common.Models.News>(command.Model));
            await Mediator.Send(new DeleteNewsFromSocialCommand(command.Model));

            return true;
        }
    }
}