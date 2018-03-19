using Escc.EastSussexGovUK.Mvc;
using Escc.NavigationControls.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Escc.Search.Website
{
    /// <summary>
    /// View model for the search results page
    /// </summary>
    /// <seealso cref="Escc.EastSussexGovUK.Mvc.BaseViewModel" />
    public class SearchResultsViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets or sets the search term.
        /// </summary>
        public string SearchTerm { get; set; }

        /// <summary>
        /// Gets or sets the search term used to refine the original result set.
        /// </summary>
        /// </value>
        public string SearchTermWithinResults { get; set; }

        /// <summary>
        /// Gets or sets properties controlling the paging of the result set
        /// </summary>
        public PagingController Paging { get; set; } = new PagingController() {
            PageSize = 20,
            MaximumResultsAvailable = 200,
            ResultsTextSingular = "result",
            ResultsTextPlural = "results"
        };

        /// <summary>
        /// Gets or sets the search results.
        /// </summary>
        public IList<ISearchResult> Results { get; set; }

        /// <summary>
        /// Gets or sets whether the search engine indicated that it's able to deliver results.
        /// </summary>
        /// <value>
        ///   <c>true</c> if results available; otherwise, <c>false</c>.
        /// </value>
        public bool ResultsAvailable { get; set; }

        /// <summary>
        /// Gets or sets the spelling suggestions.
        /// </summary>
        public IList<string> SpellingSuggestions { get; set; }
    }
}