if (typeof (jQuery) != 'undefined') {
    // Track whether a search result or paging navigation has been clicked.
    // If not, we want to track all other visits using the onunload event.
    var resultOrPagingClicked = false;
    $(window).unload(function () {
        if (!resultOrPagingClicked) {
            if (typeof(ga) !== 'undefined') ga('send', 'event', 'search-results', "none clicked");
        }
    });

    $(function () {
        // Ignore any clicks on paging nav. This script will run again on next page.
        $(".pagingPages a").click(function() { resultOrPagingClicked = true; });

        // For any click on a search result...
        $(".article dt a").click(function () {

            // Which page of results are we on?
            var pageNumber = document.URL.match(/page=([0-9]+)/);
            pageNumber = pageNumber ? parseInt(pageNumber[1]) : 1;

            // Which result on the page?
            var positionClicked = $(this).parents("dl").children("dt").index($(this).parents("dt")) + 1;

            // So which result within the total set of results?
            positionClicked = (((pageNumber - 1) * 20) + positionClicked);

            // Track which result was clicked, and the file format as we suspect that may affect it
            if (typeof(ga) !== 'undefined') ga('send', 'event', 'search-results', positionClicked.toString(), this.className);

            // Don't track this a second time at onunload
            resultOrPagingClicked = true;
        });
    });
}