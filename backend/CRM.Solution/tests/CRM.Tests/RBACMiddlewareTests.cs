using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Xunit;
namespace CRM.Tests;

public class RBACMiddlewareTests 
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;


    public RBACMiddlewareTests(
        WebApplicationFactory<Program> factory)
    {
        _client = factory
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Development");
            })
            .CreateClient();
    }



    [Fact]
    public async Task Unauthorized_User_Should_Not_Access_Users_API()
    {
        var response =
            await _client.GetAsync("/api/Users");


        var body =
            await response.Content.ReadAsStringAsync();


        Console.WriteLine(
            "STATUS: " + response.StatusCode);


        Console.WriteLine(
            "BODY: " + body);



        response.StatusCode
            .Should()
            .Be(HttpStatusCode.Unauthorized);
    }




    [Fact]
    public async Task Invalid_JWT_Should_Return_401()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                "invalid-token"
            );


        var response =
            await _client.GetAsync("/api/Users");


        var body =
            await response.Content.ReadAsStringAsync();


        Console.WriteLine(
            "STATUS: " + response.StatusCode);


        Console.WriteLine(
            "BODY: " + body);



        response.StatusCode
            .Should()
            .Be(HttpStatusCode.Unauthorized);
    }




    [Fact]
    public async Task Health_API_Should_Work_Without_Login()
    {
        var response =
            await _client.GetAsync("/api/Health");


        var body =
            await response.Content.ReadAsStringAsync();


        Console.WriteLine(
            "STATUS: " + response.StatusCode);


        Console.WriteLine(
            "BODY: " + body);



        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);
    }
}