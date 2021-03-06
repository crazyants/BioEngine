﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace BioEngine.Site.ViewModels.News
{
    public class NewsListViewModel : BaseViewModel
    {
        public NewsListViewModel(BaseViewModelConfig config, IEnumerable<Common.Models.News> news, int totalNews,
            int currentPage) : base(config)
        {
            News = news;
            TotalNews = totalNews;
            CurrentPage = currentPage;
        }

        public IEnumerable<Common.Models.News> News { get; }

        public int TotalNews { get; }
        public int CurrentPage { get; }

        public override string Title()
        {
            return SiteTitle;
        }

        protected override async Task<string> GetDescription()
        {
            return await Task.FromResult("Самые последние новости из мира BioWare");
        }
    }
}