﻿using System.Collections.Generic;
using BioEngine.Common.Base;
using BioEngine.Common.Models;
using BioEngine.Site.Components.Url;

namespace BioEngine.Site.ViewModels.Articles
{
    public class ParentArticlesViewModel : BaseViewModel
    {
        public List<CatsTree<ArticleCat, Article>> Cats;
        public ParentModel Parent;

        public ParentArticlesViewModel(BaseViewModelConfig config, ParentModel parent,
            List<CatsTree<ArticleCat, Article>> cats) : base(config)
        {
            Parent = parent;
            Cats = cats;
            BreadCrumbs.Add(new BreadCrumbsItem(UrlManager.ParentUrl(Parent), Parent.DisplayTitle));
            Title = $"{Parent.DisplayTitle} - Статьи";
        }

        public string ParentArticlesUrl => UrlManager.Articles.ParentArticlesUrl((dynamic) Parent);
        public string ParentIconUrl => UrlManager.ParentIconUrl((dynamic) Parent);
    }
}