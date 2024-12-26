using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using Xunit;
using FluentAssertions;
using System.Net;
namespace TestProject
{
    public class Tests: IClassFixture<WebApplicationFactory<Program>>
    {
        public readonly WebApplicationFactory<Program> application;
        public Tests(WebApplicationFactory<Program> factory)
        {
            this.application = factory;
        }
        [Fact]
        public async Task StatusOk()
        {
            var client = new WebApplicationFactory<Program>().CreateClient();
            var jsonContent = await File.ReadAllTextAsync("../../../input.json");
            var pars = JsonConvert.DeserializeObject<Parameters>(jsonContent);
            var response = await client.PutAsJsonAsync("https://localhost:52032/experiments", pars);
            var string_data = await response.Content.ReadAsStringAsync();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        [Fact]
        public async Task ResponceOk()
        {
            var client = new WebApplicationFactory<Program>().CreateClient();
            var jsonContent = await File.ReadAllTextAsync("../../../input.json");
            var pars = JsonConvert.DeserializeObject<Parameters>(jsonContent);
            var response = await client.PutAsJsonAsync("https://localhost:52032/cats", pars);
            response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound, HttpStatusCode.MethodNotAllowed);
        }
        [Fact]
        public async Task StepOk()
        {
            var client = new WebApplicationFactory<Program>().CreateClient();
            var jsonContent = await File.ReadAllTextAsync("../../../input.json");
            var response1 = await client.PutAsJsonAsync("https://localhost:52032/experiments", JsonConvert.DeserializeObject<Parameters>(jsonContent));
            var jsonresp1 = await response1.Content.ReadAsStringAsync();
            var resp1 = JsonConvert.DeserializeObject<Re>(jsonresp1);
            var id = resp1.expid;
  
            double prev = double.MaxValue;

            for (int i = 1; i < 10; ++i)
            {
                var responce = await client.PostAsJsonAsync($"https://localhost:52032/experiments/{id}", id);
                var jsonresp = await responce.Content.ReadAsStringAsync();
                var resp = JsonConvert.DeserializeObject<Res>(jsonresp);
                resp.epoch.Should().Be(i);
                resp.result.Should().BeLessThanOrEqualTo(prev);
                prev = resp.result;
            }
        }
        public record Re(Guid expid);
        public record Res(double result, int epoch);

    }

}