﻿using System.Linq;
using System.Threading.Tasks;
using BioEngine.Data.Core;
using BioEngine.Data.News.Queries;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace BioEngine.Data.News.Handlers
{
    [UsedImplicitly]
    internal class GetOneNewsHandler : QueryHandlerBase<GetOneNewsQuery, Common.Models.News>
    {
        public GetOneNewsHandler(HandlerContext<GetOneNewsHandler> context) : base(context)
        {
        }

        protected override async Task<Common.Models.News> RunQueryAsync(GetOneNewsQuery message)
        {
            var query = DBContext.News.AsQueryable();
            if (!message.WithUnPublishedNews)
                query = query.Where(x => x.Pub == 1);

            if (message.DateStart != null)
            {
                query = query.Where(x => x.Date >= message.DateStart);
            }

            if (message.DateEnd != null)
            {
                query = query.Where(x => x.Date <= message.DateEnd);
            }

            if (!string.IsNullOrEmpty(message.Url))
            {
                query = query.Where(x => x.Url == message.Url);
            }

            var news =
                await query
                    .Include(x => x.Author)
                    .Include(x => x.Game)
                    .Include(x => x.Developer)
                    .Include(x => x.Topic)
                    .FirstOrDefaultAsync();

            return news;
        }
    }
}