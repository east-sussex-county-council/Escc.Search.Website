using Escc.Net;
using Escc.Search.Google;
using Escc.Web;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Escc.Search.Website
{
    public class SearchController : Controller
    {
        // GET: Search
        public ActionResult Index()
        {
            RedirectOldUrls();

            var model = new SearchResultsViewModel();
            model.SearchTerm = Request.QueryString["q"];
            model.SearchTermWithinResults = Request.QueryString["refine"];
            model.Metadata.Title = "Search results";
            model.Metadata.DateCreated = "2012-03-16";
            model.Metadata.IpsvPreferredTerms = "Local government";
            model.Metadata.LgtlType = "Search results";
            model.Metadata.IsInSearch = false;

            // If there's a search query
            if (!String.IsNullOrEmpty(model.SearchTerm))
            {
                // Redisplay search term
                if (!String.IsNullOrEmpty(model.SearchTermWithinResults))
                {
                    model.Metadata.Title = "Search results for '" + HttpUtility.HtmlEncode(model.SearchTermWithinResults) + "' within '" + HttpUtility.HtmlEncode(model.SearchTerm) + "'";
                }
                else
                {
                    model.Metadata.Title = "Search results for '" + HttpUtility.HtmlEncode(model.SearchTerm) + "'";
                }

                // Search Google with standard options
                var service = new GoogleCustomSearch(ConfigurationManager.AppSettings["GoogleSearchApiKey"], ConfigurationManager.AppSettings["GoogleSearchEngineId"], new ConfigurationProxyProvider());
                var cacheHours = new TimeSpan(Int32.Parse(ConfigurationManager.AppSettings["CacheHours"], CultureInfo.CurrentCulture), 0, 0);
                service.CacheStrategy = new FileCacheStrategy(Server.MapPath(ConfigurationManager.AppSettings["CacheFilePath"]), cacheHours);
                var query = new GoogleQuery(model.SearchTerm);
                if (!String.IsNullOrEmpty(model.SearchTermWithinResults)) query.QueryWithinResultsTerms = model.SearchTermWithinResults;
                query.PageSize = model.Paging.PageSize;
                query.Page = model.Paging.CurrentPage;

                var response = service.Search(query);

                // Display results
                model.Paging.TotalResults = response.TotalResults;
                model.Results = response.Results();
                model.SpellingSuggestions = response.SpellingSuggestions();

#if (!DEBUG)
                new HttpCacheHeaders().CacheUntil(Response.Cache, DateTime.Now.AddDays(1));
#endif
            }
            else
            {
                Response.StatusCode = 400;
            }


            return View(model);
        }

        private void RedirectOldUrls()
        {
            // Use standard parameter instead of the old tQ
            if (String.IsNullOrEmpty(Request.QueryString["q"]) && !String.IsNullOrEmpty(Request.QueryString["tq"]))
            {
                var query = HttpUtility.ParseQueryString(Request.QueryString.ToString());
                query.Remove("tq");
                query.Add("q", Request.QueryString["tq"]);
                var revisedUrl = new Uri(Request.Url, new Uri(Request.Url.AbsolutePath + "?" + query, UriKind.Relative));
                new HttpStatus().MovedPermanently(revisedUrl);
            }
        }
    }
}