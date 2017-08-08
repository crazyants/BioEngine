﻿using BioEngine.Common.Models;
using BioEngine.Data.Core;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace BioEngine.Data.Base.Queries
{
    public class GetDeveloperByIdQuery : SingleModelQueryBase<Developer>
    {
        public int Id { get; }

        public GetDeveloperByIdQuery(int id)
        {
            Id = id;
        }
    }
}