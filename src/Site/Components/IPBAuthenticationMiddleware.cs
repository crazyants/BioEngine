﻿using System.Text.Encodings.Web;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BioEngine.Site.Components
{
    [UsedImplicitly]
    public class IPBAuthenticationMiddleware : AuthenticationMiddleware<IpbAuthenticationOptions>
    {
        public IPBAuthenticationMiddleware(RequestDelegate next, IOptions<IpbAuthenticationOptions> options,
            ILoggerFactory loggerFactory, UrlEncoder encoder) : base(next, options, loggerFactory, encoder)
        {
        }

        protected override AuthenticationHandler<IpbAuthenticationOptions> CreateHandler()
        {
            return new IpbAuthenticationHandler();
        }
    }

    public class IpbAuthenticationOptions : AuthenticationOptions
    {
        public string ForumUrl { get; set; }
    }
}