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
        private readonly HttpClient _client;

        public DepartmentTests(WebApplicationFactory<BangazonWorkforce.Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_IndexReturnsSuccessAndCorrectContentType()
        {
            string url = "/department";
            HttpResponseMessage response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task Post_CreateAddsDepartment()
        {
            string url = "/department/create";
            HttpResponseMessage createPageResponse = await _client.GetAsync(url);
            IHtmlDocument createPage = await HtmlHelpers.GetDocumentAsync(createPageResponse);


            string newDepartmentName = "Dept -" + Guid.NewGuid().ToString();
            string newDepartmentBudget = new Random().Next().ToString();
            HttpResponseMessage response = await _client.SendAsync(
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
