﻿using System.Collections.Generic;
using System.Threading.Tasks;
using BioEngine.Common.Models;

namespace BioEngine.Site.ViewModels.Games
{
    public class GamePageViewModel : BaseViewModel
    {
        public GamePageViewModel(BaseViewModelConfig config, Game game, IEnumerable<Common.Models.News> lastNews,
            IEnumerable<Article> lastArticles, IEnumerable<File> lastFiles, IEnumerable<GalleryPic> lastPics)
            : base(config)
        {
            Game = game;
            LastNews = lastNews;
            LastArticles = lastArticles;
            LastFiles = lastFiles;
            LastPics = lastPics;
            //ImageUrl = new Uri(UrlManager.ParentIconUrl(game));
            Description = game.Desc;
        }

        public Game Game { get; }
        public IEnumerable<Common.Models.News> LastNews { get; }
        public IEnumerable<Article> LastArticles { get; }
        public IEnumerable<File> LastFiles { get; }
        public IEnumerable<GalleryPic> LastPics { get; }

        public override Task<string> Title()
        {
            return Task.FromResult(Game.Title);
        }
    }
}