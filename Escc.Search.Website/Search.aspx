<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="Escc.Search.Website.SearchPage" ValidateRequest="false" %>
<asp:Content runat="server" ContentPlaceHolderID="metadata">
    <Metadata:MetadataControl runat="server" 
        Title="Search results"
        DateCreated="2012-03-16"
   		IpsvPreferredTerms="Local government"
		LgtlType="Search results"
		IsInSearch="false"
    />
    <ClientDependency:Css runat="server" Files="SearchSmall;FormsSmall;ContentSmall" Moveable="False" />
    <EastSussexGovUK:ContextContainer runat="server" Desktop="true">
        <ClientDependency:Css runat="server" Files="ContentMedium;SearchMedium;FormsMedium" MediaConfiguration="Medium" Moveable="False"/>
        <ClientDependency:Css runat="server" Files="ContentLarge;FormsLarge" MediaConfiguration="Large" Moveable="False"/>
    </EastSussexGovUK:ContextContainer>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="content">
    <div class="article">
        <div class="text-content content">
            <h1 id="heading" runat="server">Search results</h1>
            <asp:placeholder runat="server" id="spelling" visible="false">
                <p class="spelling"><strong>Did you mean <asp:placeholder id="suggestions" runat="server" />?</strong></p>
            </asp:placeholder>

            <NavigationControls:PagingController runat="server" PageSize="20" ID="paging" MaximumResultsAvailable="1000" />
            <NavigationControls:PagingBarControl runat="server" PagingControllerId="paging" />

            <asp:placeholder id="noResults" runat="server" visible="false">
    	        <p>Sorry, there are no pages on this site that match your search.</p>

                <p>Things you could do:</p>

                <ul>
                <li>Check your spelling</li>
                <li>Use other words related to your topic</li>
                <li>Try navigating to the information you need, starting with the main menu at the top of this page</li>
                </ul>	
        	
                <p>We want to make this site easy for everyone to use. Please tell us if you still need help, or want to report your problem:</p>
        	
                <p><a href="/contactus/emailus/feedback.aspx">Contact the web team</a> for help, and to let us know.</p>
	        </asp:placeholder>

            <asp:placeholder runat="server" id="resultsUnavailable" visible="false">
                <p>Sorry, only the first 1000 results are available to view.</p>
                <p>We are using Google search, and this restriction is a part of their service.</p>
            </asp:placeholder>

            <asp:repeater runat="server" id="results">
                <HeaderTemplate>
                    <dl>
                </HeaderTemplate>
                <ItemTemplate>
                    <dt><a href="<%# Eval("Url") %>" class="<%# System.IO.Path.GetExtension(new Uri(Eval("Url").ToString()).AbsolutePath).Trim('.') %>"><%# (Eval("Title") == null) ? System.IO.Path.GetFileName(Eval("Url").ToString()) : FormatTitle(Eval("Title").ToString())%></a></dt>
                    <dd>
                        <p class="url"><%# EsccWebTeam.Data.Web.Iri.ShortenForDisplay(Eval("Url") as Uri) %></p>
                        <p><%# FormatExcerpt(Eval("Excerpt").ToString()) %></p>
                    </dd>
                </ItemTemplate>
                <FooterTemplate>
                    </dl>
                </FooterTemplate>
            </asp:repeater>
            <NavigationControls:PagingBarControl runat="server" PagingControllerId="paging" />
        </div>
    </div>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="afterForm">
    <div class="supporting text-content content-small content-medium refine">
        <form method="get" action="search.aspx" class="form short-form">
        <h2>Search within these results</h2>
        <label for="refine" class="aural">Search for:</label>
        <input type="hidden" name="q" value="<%= HttpUtility.HtmlEncode(Request.QueryString["q"]) %>" />
        <input type="search" name="refine" id="refine" value="<%= HttpUtility.HtmlEncode(Request.QueryString["refine"]) %>" />
        <input type="submit" value="Search" class="button" />
        </form>
    </div>

    <div class="supporting text-content content-small content-medium">
        <p>These search results are from <img src="<%= ResolveUrl("~/") %>img/google.png" alt="Google logo" width="80" height="27" class="google" /></p>
    </div>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="supporting" />

<asp:Content runat="server" ContentPlaceHolderID="javascript">
    <ClientDependency:Script runat="server" Files="Search" />
</asp:Content>