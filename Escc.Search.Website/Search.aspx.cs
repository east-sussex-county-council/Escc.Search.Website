using System;
using System.Configuration;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Xml;
using Escc.Search.Google;
using EsccWebTeam.EastSussexGovUK.MasterPages;
using Exceptionless;
using Escc.Web;

namespace Escc.Search.Website
{
    public partial class SearchPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var skinnable = Master as BaseMasterPage;
            if (skinnable != null)
            {
                skinnable.Skin = new CustomerFocusSkin(ViewSelector.CurrentViewIs(MasterPageFile));
            }
            
            // Use standard parameter instead of the old tQ
            if (String.IsNullOrEmpty(Request.QueryString["q"]) && !String.IsNullOrEmpty(Request.QueryString["tq"]))
            {
                var query = HttpUtility.ParseQueryString(Request.QueryString.ToString());
                query.Remove("tq");
                query.Add("q", Request.QueryString["tq"]);
                var revisedUrl = new Uri(Request.Url.Scheme + "://" + Request.Url.Authority + Request.Url.AbsolutePath + "?" + query);
                new HttpStatus().MovedPermanently(revisedUrl);
            }

            // If there's a search query
            if (!String.IsNullOrEmpty(Request.QueryString["q"]))
            {
                // Redisplay search term
                if (!String.IsNullOrEmpty(Request.QueryString["refine"]))
                {
                    this.Title = "Search results for '" + HttpUtility.HtmlEncode(Request.QueryString["refine"]) + "' within '" + HttpUtility.HtmlEncode(Request.QueryString["q"]) + "'";
                }
                else
                {
                    this.Title = "Search results for '" + HttpUtility.HtmlEncode(Request.QueryString["q"]) + "'";
                }
                this.heading.InnerHtml = this.Title;

                // Search Google with standard options
                var service = new GoogleSiteSearch(ConfigurationManager.AppSettings["GoogleSearchEngineId"]);
                var cacheHours = new TimeSpan(Int32.Parse(ConfigurationManager.AppSettings["CacheHours"], CultureInfo.CurrentCulture), 0, 0);
                service.CacheStrategy = new FileCacheStrategy(Server.MapPath(ConfigurationManager.AppSettings["CacheFilePath"]), cacheHours);
                var query = new GoogleQuery(Request.QueryString["q"]);
                if (!String.IsNullOrEmpty(Request.QueryString["refine"])) query.QueryWithinResultsTerms = Request.QueryString["refine"];
                query.PageSize = this.paging.PageSize;
                query.Page = this.paging.CurrentPage;

                try
                {
                    var response = service.Search(query);

                    // Display results
                    this.paging.TotalResults = response.TotalResults;
                    this.noResults.Visible = (this.paging.TotalResults == 0 && response.ResultsAvailable);
                    this.resultsUnavailable.Visible = (this.paging.TotalResults == 0 && !response.ResultsAvailable);

                    var searchResults = response.Results();
                    this.results.DataSource = searchResults;
                    this.results.DataBind();

                    // Display spelling suggestions
                    if (response.SpellingSuggestions().Count > 0)
                    {
                        this.spelling.Visible = true;
                        foreach (string suggestion in response.SpellingSuggestions())
                        {
                            if (this.suggestions.Controls.Count > 0) this.suggestions.Controls.Add(new LiteralControl(" or "));
                            using (var spellingLink = new HtmlAnchor())
                            {
                                spellingLink.InnerText = suggestion;
                                spellingLink.HRef = Request.Url.LocalPath + "?q=" + HttpUtility.UrlEncode(suggestion);
                                this.suggestions.Controls.Add(spellingLink);
                            }
                        }
                    }
                }
                catch (XmlException ex)
                {
                    // This catches where Google has a 500 error and sends back malformed XML, <GSP VER="3.2"> <ERROR>500</ERROR>. 
                    // Exception is "Unexpected end of file has occurred. The following elements are not closed: GSP. Line 3, position 19."

                    this.noResults.Visible = true;
                    new HttpStatus().BadGateway();
                    ex.ToExceptionless().Submit();
                }
            }
            else
            {
                this.noResults.Visible = true;
                new HttpStatus().BadRequest();
            }

#if (!DEBUG)
            new HttpCacheHeaders().CacheUntil(Response.Cache, DateTime.Now.AddDays(1));
#endif
        }

        protected static string FormatTitle(string title)
        {
            if (String.IsNullOrEmpty(title)) return title;

            return title
                .Replace(" – East Sussex County Council", String.Empty) // remove standard suffix from title
                .Replace(" – East Sussex County <b>...</b>", String.Empty)
                .Replace(" – East Sussex <b>...</b>", String.Empty)
                .Replace(" – East <b>...</b>", String.Empty)
                .Replace("<b>...</b>", "&#8230;") // use correct character
                .Replace("<b>", "<mark>").Replace("</b>", "</mark>"); // use correct markup
        }

        protected static string FormatExcerpt(string excerpt)
        {
            if (String.IsNullOrEmpty(excerpt)) return excerpt;

            return excerpt.Replace("<br>", String.Empty) // allow text to wrap freely as window is resized
                .Replace("<b>...</b>", "<span class=\"ellipsis\">&#8230;</span>") // use correct character
                .Replace("<b>", "<mark>").Replace("</b>", "</mark>"); // use correct markup
        }
    }
}