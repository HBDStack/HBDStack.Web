using FluentAssertions;
using HBDStack.AspNetCore.Extensions;
using HBDStack.AspNetCore.Extensions.Providers;
using HBDStack.AzProxy.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Security.Claims;
using System.Security.Principal;

namespace HBD.AspNetCore.Tests;

[TestClass]
public class JwtTokenHeaderProviderTests
{
    #region Methods

    [TestMethod]
    public async System.Threading.Tasks.Task GetTokenFromClaimsAsync()
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

        var h = new JwtTokenHeaderProvider(httpMock.Object);

        await foreach (var c in h.GetHeaderAsync())
        {
            c.Key.Should().Be(HeaderKeys.Authorization);
            c.Value.Should().Be("mytoken");
        }
    }

    #endregion Methods
}