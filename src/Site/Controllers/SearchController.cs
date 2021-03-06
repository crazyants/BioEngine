﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BioEngine.Common.Base;
using BioEngine.Common.Interfaces;
using BioEngine.Common.Models;
using BioEngine.Data.Articles.Queries;
using BioEngine.Data.Base.Queries;
using BioEngine.Data.Files.Queries;
using BioEngine.Data.Gallery.Queries;
using BioEngine.Data.News.Queries;
using BioEngine.Data.Search.Commands;
using BioEngine.Data.Search.Queries;
using BioEngine.Site.Base;
using BioEngine.Site.ViewModels.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
// ReSharper disable once RedundantUsingDirective
using Microsoft.Extensions.DependencyInjection;
using BioEngine.Routing;
using BioEngine.Search.Interfaces;
using BioEngine.Web;
using MediatR;

namespace BioEngine.Site.Controllers
{
    public class SearchController : BaseController
    {
        private readonly IContentHelperInterface _contentHelper;

        public SearchController(IMediator mediator, IOptions<AppSettings> appSettingsOptions,
            IContentHelperInterface contentHelper)
            : base(mediator, appSettingsOptions, contentHelper)
        {
            _contentHelper = contentHelper;
        }

        public async Task<IActionResult> Index(string query, string block)
        {
            var viewModel = new SearchViewModel(ViewModelConfig, query);
            if (!string.IsNullOrEmpty(query))
            {
                var hasBlock = !string.IsNullOrEmpty(block);
                var limit = hasBlock ? 0 : 5;
                if (!hasBlock || block == "games")
                {
                    var games = await SearchEntities<Game>(query, limit);
                    var gamesCount = await CountEntities<Game>(query);
                    var searchBlock = CreateSearchBlock("Игры", Url.Search().BlockUrl("games", query), gamesCount,
                        games, x => x.Title,
                        x => Url.Base().PublicUrl(x),
                        x => _contentHelper.ReplacePlaceholdersAsync(x.Desc));
                    viewModel.AddBlock(await searchBlock);
                }

                if (!hasBlock || block == "news")
                {
                    var news = await SearchEntities<News>(query, limit);
                    var newsCount = await CountEntities<News>(query);
                    var searchBlock = CreateSearchBlock("Новости", Url.Search().BlockUrl("news", query), newsCount,
                        news, x => x.Title,
                        x => Url.News().PublicUrl(x),
                        x => _contentHelper.ReplacePlaceholdersAsync(x.ShortText));
                    viewModel.AddBlock(await searchBlock);
                }

                if (!hasBlock || block == "articles")
                {
                    var articles = await SearchEntities<Article>(query, limit);
                    var articlesCount = await CountEntities<Article>(query);
                    var searchBlock = CreateSearchBlock("Статьи", Url.Search().BlockUrl("articles", query),
                        articlesCount,
                        articles, x => x.Title,
                        x => Url.Articles().PublicUrl(x),
                        x => _contentHelper.ReplacePlaceholdersAsync(x.Announce));
                    viewModel.AddBlock(await searchBlock);
                }

                if (!hasBlock || block == "articlesCats")
                {
                    var articlesCats = await SearchEntities<ArticleCat>(query, limit);
                    var articlesCatsCount = await CountEntities<ArticleCat>(query);
                    var searchBlock = CreateSearchBlock("Категории статей",
                        Url.Search().BlockUrl("articlesCats", query),
                        articlesCatsCount,
                        articlesCats, x => x.Title,
                        x => Url.Articles().CatPublicUrl(x),
                        x => _contentHelper.ReplacePlaceholdersAsync(x.Descr));
                    viewModel.AddBlock(await searchBlock);
                }

                if (!hasBlock || block == "files")
                {
                    var files = await SearchEntities<File>(query, limit);
                    var filesCount = await CountEntities<File>(query);
                    var searchBlock = CreateSearchBlock("Файлы", Url.Search().BlockUrl("files", query),
                        filesCount,
                        files, x => x.Title,
                        x => Url.Files().PublicUrl(x),
                        x => _contentHelper.ReplacePlaceholdersAsync(x.Announce));
                    viewModel.AddBlock(await searchBlock);
                }

                if (!hasBlock || block == "filesCat")
                {
                    var fileCats = await SearchEntities<FileCat>(query, limit);
                    var fileCatsCount = await CountEntities<FileCat>(query);
                    var searchBlock = CreateSearchBlock("Категории файлов",
                        Url.Search().BlockUrl("filesCat", query),
                        fileCatsCount,
                        fileCats, x => x.Title,
                        x => Url.Files().CatPublicUrl(x),
                        x => _contentHelper.ReplacePlaceholdersAsync(x.Descr));
                    viewModel.AddBlock(await searchBlock);
                }

                if (!hasBlock || block == "galleryCats")
                {
                    var galleryCats = await SearchEntities<GalleryCat>(query, limit);
                    var galleryCatsCount = await CountEntities<GalleryCat>(query);
                    var searchBlock = CreateSearchBlock("Категории картинок",
                        Url.Search().BlockUrl("galleryCats", query),
                        galleryCatsCount,
                        galleryCats, x => x.Title,
                        x => Url.Gallery().CatPublicUrl(x),
                        x => _contentHelper.ReplacePlaceholdersAsync(x.Desc));
                    viewModel.AddBlock(await searchBlock);
                }
            }
            return View(viewModel);
        }

        private async Task<SearchBlock> CreateSearchBlock<T>(string title, Uri url, long totalCount,
            IEnumerable<T> items,
            Func<T, string> getTitle, Func<T, Uri> getUrl, Func<T, Task<string>> getDesc)
        {
            var block = new SearchBlock(title, url, totalCount);
            foreach (var item in items)
            {
                block.AddItem(getTitle(item), getUrl(item), await getDesc(item));
            }
            return block;
        }

        private async Task<IEnumerable<T>> SearchEntities<T>(string query, int limit = 0) where T : IBaseModel
        {
            return await Mediator.Send(new SearchEntitiesQuery<T>(query, limit));
        }

        private async Task<long> CountEntities<T>(string query) where T : IBaseModel
        {
            return await Mediator.Send(new CountEntitiesQuery<T>(query));
        }

        public async Task<string> Reindex()
        {
            await Mediator.Publish(
                new IndexEntitiesCommand<Game>((await Mediator.Send(new GetGamesQuery())).models));
            await Mediator.Publish(
                new IndexEntitiesCommand<News>((await Mediator.Send(new GetNewsQuery())).models));
            await Mediator.Publish(
                new IndexEntitiesCommand<Article>((await Mediator.Send(new GetArticlesQuery())).models));
            await Mediator.Publish(
                new IndexEntitiesCommand<ArticleCat>((await Mediator.Send(new GetArticlesCategoriesQuery())).models));
            await Mediator.Publish(
                new IndexEntitiesCommand<File>((await Mediator.Send(new GetFilesQuery())).models));
            await Mediator.Publish(
                new IndexEntitiesCommand<FileCat>((await Mediator.Send(new GetFilesCategoriesQuery())).models));
            await Mediator.Publish(
                new IndexEntitiesCommand<GalleryCat>((await Mediator.Send(new GetGalleryCategoriesQuery())).models));
            return "done";
        }

        public async Task<IActionResult> Clean(string accessToken,
            [FromServices] IOptions<AdminAccessConfig> adminAccessConfig,
            [FromServices] IEnumerable<ISearchProvider> searchProviders)
        {
            if (accessToken != adminAccessConfig.Value.AdminAccessToken)
            {
                return Forbid();
            }
            foreach (var searchProvider in searchProviders)
            {
                await searchProvider.DeleteIndexAsync();
            }
            return Ok("done");
        }
    }
}