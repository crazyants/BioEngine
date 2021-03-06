﻿using System.Threading.Tasks;
using BioEngine.Common.Interfaces;
using BioEngine.Data.Base.Queries;
using BioEngine.Data.Core;
using JetBrains.Annotations;

namespace BioEngine.Data.Base.Handlers
{
    [UsedImplicitly]
    internal class GetParentByUrlHandler : QueryHandlerBase<GetParentByUrlQuery, IParentModel>
    {
        private readonly ParentEntityProvider _provider;

        public GetParentByUrlHandler(HandlerContext<GetParentByUrlHandler> context, ParentEntityProvider provider) :
            base(context)
        {
            _provider = provider;
        }

        protected override async Task<IParentModel> RunQueryAsync(GetParentByUrlQuery message)
        {
            return await _provider.GetParenyByUrlAsync(message.Url);
        }
    }
}