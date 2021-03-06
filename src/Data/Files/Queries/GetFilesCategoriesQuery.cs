﻿using BioEngine.Common.Interfaces;
using BioEngine.Common.Models;
using BioEngine.Data.Core;

namespace BioEngine.Data.Files.Queries
{
    public class GetFilesCategoriesQuery : ModelsListQueryBase<FileCat>, ICategoryQuery<FileCat>
    {
        public bool OnlyRoot { get; set; }
        public IParentModel Parent { get; set; }
        public bool LoadChildren { get; set; }
        public FileCat ParentCat { get; set; }
        public int? LoadLastItems { get; set; }
        public string Url { get; set; }
    }
}