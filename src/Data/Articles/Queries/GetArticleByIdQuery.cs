﻿using BioEngine.Common.Models;
using BioEngine.Data.Core;

namespace BioEngine.Data.Articles.Queries
{
    public class GetArticleByIdQuery : SingleModelQueryBase<Article>
    {
        public GetArticleByIdQuery(int id)
        {
            Id = id;
        }

        public int Id { get; }
    }
}