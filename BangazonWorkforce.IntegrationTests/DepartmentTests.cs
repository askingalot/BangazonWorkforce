using AngleSharp.Dom.Html;
using BangazonWorkforce.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace BangazonWorkforce.IntegrationTests
{
    public class DepartmentTests :
        IClassFixture<WebApplicationFactory<BangazonWorkforce.Startup>>
    {
        private readonly WebApplicationFactory<BangazonWorkforce.Startup> _factory;

        public DepartmentTests(WebApplicationFactory<BangazonWorkforce.Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_IndexReturnsSuccessAndCorrectContentType()
        {
            string url = "/department";
            HttpClient client = _factory.CreateClient();
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task Post_CreateAddsDepartment()
        {
            HttpClient client = _factory.CreateClient();
            string url = "/department/create";
            HttpResponseMessage createPageResponse = await client.GetAsync(url);
            IHtmlDocument createPage = await HtmlHelpers.GetDocumentAsync(createPageResponse);


            string newDepartmentName = "Dept -" + Guid.NewGuid().ToString();
            string newDepartmentBudget = new Random().Next().ToString();
            HttpResponseMessage response = await client.SendAsync(
                createPage,
                new Dictionary<string, string>
                {
                    {"Name", newDepartmentName},
                    {"Budget", newDepartmentBudget}
                });
            IHtmlDocument indexPage = await HtmlHelpers.GetDocumentAsync(response);


            Assert.Contains(
                indexPage.QuerySelectorAll("td"), 
                td => td.TextContent.Contains(newDepartmentName));
        }
    }
}
