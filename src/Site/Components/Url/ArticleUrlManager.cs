﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BioEngine.Common.Base;
using BioEngine.Common.DB;
using BioEngine.Common.Models;
using BioEngine.Site.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace BioEngine.Site.Components.Url
{
    public class ArticleUrlManager : EntityUrlManager
    {
        public ArticleUrlManager(AppSettings settings, BWContext dbContext, IUrlHelper urlHelper,
            ParentEntityProvider parentEntityProvider)
            : base(settings, dbContext, urlHelper, parentEntityProvider)
        {
        }

        public async Task<string> PublicUrl(Article article)
        {
            await Poppulate(article);
            var url = await CatUrl(article.Cat) + "/" + article.Url;
            return GetUrl("Show", "Articles",
                new {parentUrl = await ParentUrl(article), url});
            /*return _urlHelper.Action<ArticlesController>(x => x.Show(ParentUrl(article), CatUrl(article), article.Url));*/
        }

        private async Task<string> CatUrl(ArticleCat cat)
        {
            await Poppulate(cat);
            var urls = new SortedList<int, string>();
            var i = 0;
            while (cat != null)
            {
                urls.Add(i, cat.Url);
                i++;
                cat = cat.ParentCat;
            }
            return string.Join("/", urls.Reverse().Select(x => x.Value).ToArray());
        }

        private async Task Poppulate(Article article)
        {
            if (article.Cat == null)
            {
                await DbContext.Entry(article).Reference(x => x.Cat).LoadAsync();
            }
            var cat = article.Cat;
            await Poppulate(cat);

            if (article.GameId > 0 && article.Game == null)
            {
                await DbContext.Entry(article).Reference(x => x.Game).LoadAsync();
            }
            if (article.DeveloperId > 0 && article.Developer == null)
            {
                await DbContext.Entry(article).Reference(x => x.Developer).LoadAsync();
            }
            if (article.TopicId > 0 && article.Topic == null)
            {
                await DbContext.Entry(article).Reference(x => x.Topic).LoadAsync();
            }
        }

        private async Task Poppulate(ArticleCat cat)
        {
            while (cat != null)
            {
                if (cat.ParentCat == null)
                {
                    if (cat.Pid > 0)
                    {
                        await DbContext.Entry(cat).Reference(x => x.ParentCat).LoadAsync();
                    }
                    else
                    {
                        break;
                    }
                }
                cat = cat.ParentCat;
            }
        }

        public async Task<string> CatPublicUrl(ArticleCat cat, int page = 1)
        {
            await Poppulate(cat);
            var url = await CatUrl(cat) + "/" + cat.Url;
            if (page > 1)
            {
                url += $"/page/{page}";
            }
            return GetUrl("Show", "Articles",
                new {parentUrl = await ParentUrl(cat), url});
        }

        public async Task<string> ParentArticlesUrl(Article article)
        {
            var parent = await ParentEntityProvider.GetModelParent(article);
            return ParentArticlesUrl((dynamic) parent);
        }

        public async Task<string> ParentArticlesUrl(ArticleCat articleCat)
        {
            var parent = await ParentEntityProvider.GetModelParent(articleCat);
            return ParentArticlesUrl((dynamic)parent);
        }

        public async Task<string> ParentArticlesUrl<T>(T parentModel) where T : ParentModel
        {
            return await Task.FromResult(UrlHelper.Action<ArticlesController>(x => x.ParentArticles(parentModel.ParentUrl)));
        }
    }
}