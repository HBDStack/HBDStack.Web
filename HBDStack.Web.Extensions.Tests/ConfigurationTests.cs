using System;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
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
        Environment.SetEnvironmentVariable("ConfigKey","TUxYWmtDaE5QR2pVS0svSEkxSFY1eFVLaUFDN1BjVlZ0YVpsbVMvS3FLOD06MkJDRmdlQ0JTcFMyeEppQi8rMkdqdz09" );
        Environment.SetEnvironmentVariable("ConnectionStrings:EncryptedConn","A8YMLArE8qziKEsvZfKBKw==" );

        var s = "Hoang Bao Duy";
        
        var config = new ConfigurationBuilder()
            .AddEncryptEnvironmentVariables("ConfigKey")
            .Build();

        config.GetConnectionString("EncryptedConn")
            .Should().Be(s);
    }
    
    [Fact]
    public void EncryptedBothTests()
    {
        Environment.SetEnvironmentVariable("ConfigKey","TUxYWmtDaE5QR2pVS0svSEkxSFY1eFVLaUFDN1BjVlZ0YVpsbVMvS3FLOD06MkJDRmdlQ0JTcFMyeEppQi8rMkdqdz09" );
        Environment.SetEnvironmentVariable("ConnectionStrings:EncryptedConn1","A8YMLArE8qziKEsvZfKBKw==" );
        Environment.SetEnvironmentVariable("ConnectionStrings:EncryptedConn2","QThZTUxBckU4cXppS0VzdlpmS0JLdz09" );

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
    }
}