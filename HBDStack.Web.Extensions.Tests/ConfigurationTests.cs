using System;
using FluentAssertions;
using HBDStack.Framework.Extensions.Encryption;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Xunit;

namespace HBDStack.Web.Extensions.Tests;

public class ConfigurationTests
{
    [Fact]
    public void EncryptedJsonTests()
    {
        var s = "Hoang Bao Duy";
        
        var config = new ConfigurationBuilder()
            .AddEncryptJsonFile("appsettings.json","ConfigKey")
            .AddEnvironmentVariables()
            .Build();

        config.GetConnectionString("EncryptedConn")
            .Should().Be(s);
    }
    
    [Fact]
    public void EncryptedEnvTests()
    {
        Environment.SetEnvironmentVariable("ConfigKey","M0REYjV4RE5tanpCVXJkQWVjWGhER0czZEIzV2VpeFg=" );
        Environment.SetEnvironmentVariable("ConnectionStrings:EncryptedConn","2WCxdmC7XEbeuAm2S0xw9G/OjwX8Izbpcs47DSMJ3DM=" );
        
        var s = "Hoang Bao Duy";
        
        var config = new ConfigurationBuilder()
            .AddEncryptEnvironmentVariables("ConfigKey")
            .Build();

        config.GetConnectionString("EncryptedConn")
            .Should().Be(s);
    }
}