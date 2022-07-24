using FluentAssertions;
using HBDStack.AspNetCore.Extensions;
using HBDStack.AspNetCore.Extensions.Internals;
using HBDStack.AspNetCore.Extensions.JwtAuth;
using HBDStack.AzProxy.Core.Providers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace HBD.AspNetCore.Tests;

[TestClass]
public class AzAdGroupClaimsProviderTests
{
    #region Methods

    [TestMethod]
    public async Task GetGroupsFromAdAsync()
    {
        var token =
            "eyJ0eXAiOiJKV1QiLCJub25jZSI6InUzdFpncjBPcjZNOGVaLUk5THNQMjFVSm9faWxPcnk4eFVPc2hDWHdUNWciLCJhbGciOiJSUzI1NiIsIng1dCI6Im5PbzNaRHJPRFhFSzFqS1doWHNsSFJfS1hFZyIsImtpZCI6Im5PbzNaRHJPRFhFSzFqS1doWHNsSFJfS1hFZyJ9.eyJhdWQiOiJodHRwczovL2dyYXBoLm1pY3Jvc29mdC5jb20iLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC82N2EwZDI4ZC1lYThiLTRmNTktYjdhMy1lNDljZDA2MzhiNTYvIiwiaWF0IjoxNjI1NjQ0MjkzLCJuYmYiOjE2MjU2NDQyOTMsImV4cCI6MTYyNTY0ODE5MywiYWNjdCI6MCwiYWNyIjoiMSIsImFjcnMiOlsidXJuOnVzZXI6cmVnaXN0ZXJzZWN1cml0eWluZm8iLCJ1cm46bWljcm9zb2Z0OnJlcTEiLCJ1cm46bWljcm9zb2Z0OnJlcTIiLCJ1cm46bWljcm9zb2Z0OnJlcTMiLCJjMSIsImMyIiwiYzMiLCJjNCIsImM1IiwiYzYiLCJjNyIsImM4IiwiYzkiLCJjMTAiLCJjMTEiLCJjMTIiLCJjMTMiLCJjMTQiLCJjMTUiLCJjMTYiLCJjMTciLCJjMTgiLCJjMTkiLCJjMjAiLCJjMjEiLCJjMjIiLCJjMjMiLCJjMjQiLCJjMjUiXSwiYWdlR3JvdXAiOiIzIiwiYWlvIjoiQVVRQXUvOFRBQUFBQ2dIMU0rSHUzd1hpTFNXMUpBUUY5MjYxc1phenpObTNhaFhqaGlvYWJuUHVDcHRMVzBWVjRueFI1M2k3QXI0d3JXemNuUXdoNGZqWGtHV3VHSHY5WlE9PSIsImFtciI6WyJyc2EiLCJtZmEiXSwiYXBwX2Rpc3BsYXluYW1lIjoiZGV2LVJhZGlvbWlyIiwiYXBwaWQiOiIwZjNhNjIzZi03MTE5LTQ5NWMtYmI0NC1lOTQ3N2E0YWNhMDYiLCJhcHBpZGFjciI6IjAiLCJmYW1pbHlfbmFtZSI6IkhvYW5nIiwiZ2l2ZW5fbmFtZSI6IlN0ZXZlbiIsImhhc3dpZHMiOiJ0cnVlIiwiaWR0eXAiOiJ1c2VyIiwiaXBhZGRyIjoiMTIxLjYuNDUuNDEiLCJuYW1lIjoiU3RldmVuIEhvYW5nIiwib2lkIjoiMGEwM2FlYmItMjM4MS00NDQ3LWI3NWEtNDRjZTJkNmUxZjcxIiwicGxhdGYiOiI1IiwicHVpZCI6IjEwMDMyMDAwODkxRDIwMDAiLCJyaCI6IjAuQVZNQWpkS2daNHZxV1UtM28tU2MwR09MVmo5aU9nOFpjVnhKdTBUcFIzcEt5Z1pUQUc0LiIsInNjcCI6IkRpcmVjdG9yeS5SZWFkLkFsbCBvcGVuaWQgcHJvZmlsZSBVc2VyLlJlYWQgZW1haWwiLCJzaWduaW5fc3RhdGUiOlsiaW5rbm93bm50d2siLCJrbXNpIl0sInN1YiI6InF5RktwSGtlUkg3UmxZTEpMQTZrSVh4VWxNeHJHbjdSa01YYXRxT3NDNHciLCJ0ZW5hbnRfcmVnaW9uX3Njb3BlIjoiQVMiLCJ0aWQiOiI2N2EwZDI4ZC1lYThiLTRmNTktYjdhMy1lNDljZDA2MzhiNTYiLCJ1bmlxdWVfbmFtZSI6InN0ZXZlbi5ob2FuZ0BUcmFuc3dhcC5jb20iLCJ1cG4iOiJzdGV2ZW4uaG9hbmdAVHJhbnN3YXAuY29tIiwidXRpIjoidG9LSWE5WkJnVTZQR2wxWHNFZlRBUSIsInZlciI6IjEuMCIsInhtc19zdCI6eyJzdWIiOiJHU2ZoTDF5N2ZnbkJVTzdCbkNHQllPZ0s2Zm1fcDRRdGJNbE5Ya3JIWVZnIn0sInhtc190Y2R0IjoxNTE2ODQ1OTE0fQ.GKGk7L_DYFwRsW5vKXGr83lkYNRTfjGwGbOKZq4ljXGzP-Hd1B7TPAJJs0orAKX9VYnYa4iE1QwQxo5ED9OAtFqjDo2BXWrNKpvzdomLFLw6qST5W3Ue8XE1h1cf2BIz6FDZp7hpOvdA9qGZMp2nOEhAiy7cBip-ItPSonIAmJTbJbf3u5R5BXciM_zpg1PtVCwR1aaYiQDxDr7zF0Js4CSqkPrEieMttrQA-2mrN0xxL6i9Go_vQH4pUmR5IrK0Jc_ZFvgEIGQJnVWMI-fE6yDlSEX1IjiQFIjHe42Ksv2B2mktUemvPn_xaPeNYstYorbNjAfybFqtvUE2jBmMlw";
        var http = new Mock<HttpContext>();
        http.SetupGet(h => h.User.Claims).Returns(new List<Claim>
            { new(JwtRegisteredClaimNames.UniqueName, "baoduy@testing.com") });
        http.SetupGet(h => h.Request.Headers)
            .Returns(new HeaderDictionary { ["Authorization"] = $"beare {token}" });

        var context = new TokenValidatedContext(http.Object,
            new AuthenticationScheme(JwtBearerDefaults.AuthenticationScheme, "Beare", typeof(JwtBearerHandler)),
            new JwtBearerOptions()) { SecurityToken = new JwtSecurityToken(token) };

        var graph = new Mock<IGraphClient>();

        graph.Setup(s => s.GetGroups(It.IsAny<string>()))
            .Returns(Task.FromResult(new GroupResult
            {
                Value =
                {
                    new GroupData
                    {
                        Id = "ROL BIZ NON-PRD TESTAPP CONTRIBUTOR",
                        DisplayName = "ROL BIZ NON-PRD TESTAPP CONTRIBUTOR", SecurityEnabled = true
                    },
                    new GroupData
                    {
                        Id = "ROL BIZ NON-PRD TESTAPP APPROVER", DisplayName = "ROL BIZ NON-PRD TESTAPP APPROVER",
                        SecurityEnabled = true
                    },
                    new GroupData
                    {
                        Id = "ROL BIZ NON-PRD TESTAPP READONLY", DisplayName = "ROL BIZ NON-PRD TESTAPP READONLY",
                        SecurityEnabled = true
                    }
                }
            }))
            .Verifiable();

        var p = new AzAdGroupClaimsProvider(new AzAdGroupClaimsValidator("NON-PRD"), graph.Object);
        var claims = await p.GetClaimsAsync(context);

        claims.Should().NotBeEmpty();
        claims.Should().HaveCountGreaterOrEqualTo(2);

        graph.VerifyAll();
    }

    [TestMethod]
    public async Task TestAddAdGroupClaimsSetupAsync()
    {
        var context = new DefaultHttpContext
        {
            // User is logged in
            User = new GenericPrincipal(new GenericIdentity("steven.hoang@mock.com"), new string[0])
        };

        context.User.AddIdentity(new ClaimsIdentity(new[]
            { new Claim(ClaimsProviderKeys.AccessToken, "mytoken") }));

        var httpMock = new Mock<IHttpContextAccessor>();
        httpMock.Setup(i => i.HttpContext).Returns(context);

        var provider = new ServiceCollection()
            .AddMemoryCache()
            .AddSingleton(httpMock.Object)
            .AddAzAdGroupClaimsProvider(new AzAdGroupClaimsValidator("NON PRD"))
            .AddJwtTokenClaimsProvider()
            .AddSingleton<DefaultJwtBearerEvents>()
            .BuildServiceProvider();

        using var scope = provider.CreateScope();
        var e = scope.ServiceProvider.GetService<DefaultJwtBearerEvents>();
        e.Should().NotBeNull();
        e!.ClaimsProviders.Should().HaveCountGreaterOrEqualTo(2);

        var h = scope.ServiceProvider.GetService<IAuthTokenHeaderProvider>();
        h.Should().NotBeNull();

        await foreach (var c in h!.GetHeaderAsync())
        {
            c.Key.Should().Be(HBDStack.AzProxy.Core.HeaderKeys.Authorization);
            c.Value.Should().Be("mytoken");
        }
    }

    [Ignore]
    [TestMethod]
    public async Task TestAzADGroupShouldIgnoreAsync()
    {
        var context = new DefaultHttpContext
        {
            // User is logged in
            User = new GenericPrincipal(new GenericIdentity("steven.hoang@mock.com"), new string[0])
        };

        context.User.AddIdentity(new ClaimsIdentity(new[]
            { new Claim(ClaimsProviderKeys.AccessToken, "mytoken") }));

        var Tokencontext = new TokenValidatedContext(context,
            new AuthenticationScheme(JwtBearerDefaults.AuthenticationScheme,
                JwtBearerDefaults.AuthenticationScheme,
                typeof(JwtBearerHandler)), new JwtBearerOptions());

        var provider =
            new Mock<AzAdGroupClaimsProvider>(new AzAdGroupClaimsValidator("NON PRD", scheme: "AAA"), null);

        provider.Protected().Setup<Task<IEnumerable<GroupData>>>("GetGroups", ItExpr.IsAny<Tokens>()).CallBase();

        var claims = await provider.Object.GetClaimsAsync(Tokencontext);

        claims.Should().BeEmpty();
        provider.Protected().Verify("GetGroups", Times.Never(), ItExpr.IsAny<Tokens>());
    }

    #endregion Methods
}