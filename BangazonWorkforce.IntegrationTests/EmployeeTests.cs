using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using BangazonWorkforce.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace BangazonWorkforce.IntegrationTests
{
    public class EmployeeTests :
        IClassFixture<WebApplicationFactory<BangazonWorkforce.Startup>>
    {
        private readonly WebApplicationFactory<BangazonWorkforce.Startup> _factory;

        public EmployeeTests(WebApplicationFactory<BangazonWorkforce.Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Post_CreateAddsEmployee()
        {
            HttpClient client = _factory.CreateClient();
            string url = "/employee/create";
            HttpResponseMessage createPageResponse = await client.GetAsync(url);
            IHtmlDocument createPage = await HtmlHelpers.GetDocumentAsync(createPageResponse);


            string newFirstName = "FirstName - " + Guid.NewGuid().ToString();
            string newLastName = "LastName - " + Guid.NewGuid().ToString();
            string isSupervisor = "true";
            string departmentId = "3";
            HttpResponseMessage response = await client.SendAsync(
                createPage,
                new Dictionary<string, string>
                {
                    {"Employee_FirstName", newFirstName},
                    {"Employee_LastName", newLastName},
                    {"Employee_IsSupervisor", isSupervisor},
                    {"Employee_DepartmentId", departmentId}
                });
            IHtmlDocument indexPage = await HtmlHelpers.GetDocumentAsync(response);


            var lastRow = indexPage.QuerySelector("tbody tr:last-child");
            Assert.Contains(
                lastRow.QuerySelectorAll("td"),
                td => td.TextContent.Contains(newFirstName));

            Assert.Contains(
                lastRow.QuerySelectorAll("td"),
                td => td.TextContent.Contains(newLastName));

            IHtmlInputElement cb = (IHtmlInputElement)lastRow.QuerySelector("input[type='checkbox']");
            if (isSupervisor == "true")
            {
                Assert.True(cb.IsChecked);
            }
            else
            {
                Assert.False(cb.IsChecked);
            } 
        }
    }
}
