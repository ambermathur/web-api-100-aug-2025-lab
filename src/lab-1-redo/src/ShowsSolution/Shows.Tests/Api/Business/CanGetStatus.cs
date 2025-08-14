using Alba;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using Shows.Api.Api.Business;

namespace Shows.Tests.Api.Business;

public class CanGetStatus
{
    [Fact]
    public async Task DuringOpenHours()
    {
        FakeTimeProvider provider = null!;
        var host = await AlbaHost.For<Program>(config =>
        {
            config.ConfigureTestServices(services =>
            {
                provider =  new FakeTimeProvider(
                    new DateTimeOffset(2025, 8,14, 14, 37, 00, TimeSpan.FromHours(-4)));
                services.AddSingleton<TimeProvider>(provider);
;            });
        });

        var response = await host.Scenario(api =>
        {
            api.Get.Url("/api/status");
            api.StatusCodeShouldBe(200);
        });
        provider.Advance(TimeSpan.FromDays(3));

        var statusMessage = response.ReadAsJson<StatusResponse>();
        Assert.NotNull(statusMessage);

        Assert.True(statusMessage.IsOpen);
    }

    [Fact]
    public async Task DuringClosedHours()
    {
        var host = await AlbaHost.For<Program>(config =>
        {
            config.ConfigureTestServices(services =>
            {
                var fakeTime = new FakeTimeProvider(
                    new DateTimeOffset(1969,4,20,23,59,00, TimeSpan.FromHours(-4)));
                services.AddSingleton<TimeProvider>(fakeTime);
                ;
            });
        });

        var response = await host.Scenario(api =>
        {
            api.Get.Url("/api/status");
            api.StatusCodeShouldBe(200);
        });

        var statusMessage = response.ReadAsJson<StatusResponse>();
        Assert.NotNull(statusMessage);

        Assert.False(statusMessage.IsOpen);
    }
}
