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
            .Build();

        config.GetConnectionString("EncryptedConn")
            .Should().Be(s);
    }
    
    [Fact]
    public void EncryptedEnvTests()
    {
        Environment.SetEnvironmentVariable("ConfigKey","M0REYjV4RE5tanpCVXJkQWVjWGhER0czZEIzV2VpeFg=" );
        Environment.SetEnvironmentVariable("ConnectionStrings:EncryptedConn","2WCxdmC7XEbeuAm2S0xw9G/OjwX8Izbpcs47DSMJ3DM=" );
        Environment.SetEnvironmentVariable("ConnectionStrings:EncryptedConn2","MldDeGRtQzdYRWJldUFtMlMweHc5Ry9PandYOEl6YnBjczQ3RFNNSjNETT0=" );
        
        var s = "Hoang Bao Duy";
        
        var config = new ConfigurationBuilder()
            .AddEncryptEnvironmentVariables("ConfigKey")
            .Build();

        config.GetConnectionString("EncryptedConn")
            .Should().Be(s);
        config.GetConnectionString("EncryptedConn2")
            .Should().Be(s);
    }
    
    [Fact]
    public void EncryptedBothTests()
    {
        Environment.SetEnvironmentVariable("ConfigKey","M0REYjV4RE5tanpCVXJkQWVjWGhER0czZEIzV2VpeFg=" );
        Environment.SetEnvironmentVariable("ConnectionStrings:EncryptedConn1","2WCxdmC7XEbeuAm2S0xw9G/OjwX8Izbpcs47DSMJ3DM=" );
        Environment.SetEnvironmentVariable("ConnectionStrings:EncryptedConn2","MldDeGRtQzdYRWJldUFtMlMweHc5Ry9PandYOEl6YnBjczQ3RFNNSjNETT0=" );
        Environment.SetEnvironmentVariable("ConnectionStrings:EncryptedConn3","VDhaQUczWUJtb25ObGVnaHNSNkNjOWt4TFZhRThSNmdPMGlhUmphMGNIV0J3ckpGVlJKUzBPRU9hYjFOcQ==" );
        
        var s = "Hoang Bao Duy";
        
        var config = new ConfigurationBuilder()
            .AddEncryptJsonFile("appsettings.json","ConfigKey")
            .AddEncryptEnvironmentVariables("ConfigKey")
            .Build();

        config.GetConnectionString("EncryptedConn")
            .Should().Be(s);
        config.GetConnectionString("EncryptedConn1")
            .Should().Be(s);
        config.GetConnectionString("EncryptedConn2")
            .Should().Be(s);
        config.GetConnectionString("EncryptedConn3")
            .Should().Be("frvmsdcportalsandbox");
    }
}