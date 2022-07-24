using HBDStack.AzProxy.Core.Providers;
using HBDStack.AzProxy.Storage;
using HBDStack.AzProxy.Vault;
using HBDStack.AzProxy.Vault.Configs;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// ReSharper disable ClassNeverInstantiated.Global

namespace HBDStack.AspNetCore.Extensions;

public static class DataProtectionExtensions
{
    #region Methods

    public static IServiceCollection AddDataProtection(this IServiceCollection services, Action<IDataProtectionBuilder> builder)
    {
        var p = services.AddDataProtection()
            .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration
            {
                EncryptionAlgorithm = EncryptionAlgorithm.AES_256_GCM,
                ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
            });
        builder?.Invoke(p);
        return services;
    }

    public static IServiceCollection AddDataProtectionWithCert(this IServiceCollection services, ICertProvider certProvider, Action<IDataProtectionBuilder> builder = null,
        ILogger logger = null)
        => services.AddDataProtection(b =>
        {
            using var cert = certProvider.GetCertAsync().GetAwaiter().GetResult();

            if (cert != null)
            {
                b.ProtectKeysWithCertificate(cert);
                builder?.Invoke(b);
            }
            else logger?.LogWarning($"The {certProvider} is not found.");
        });

    public static IServiceCollection AddDataProtectionWithCert(this IServiceCollection services, string certPath, string certPass,
        Action<IDataProtectionBuilder> builder = null, ILogger logger = null)
        => services.AddDataProtectionWithCert(new CertFileProvider(certPath, certPass), builder, logger);

    public static IServiceCollection AddDataProtectionWithVault(this IServiceCollection services, VaultConfig vaultConfig)
    {
        if (vaultConfig == null) throw new ArgumentNullException(nameof(vaultConfig));
        return services.AddDataProtectionWithVaultAndStorage(vaultConfig);
    }

    public static IServiceCollection AddDataProtectionStorage(this IServiceCollection services, AzKeyBlobInfo blobInfo)
    {
        if (blobInfo == null) throw new ArgumentNullException(nameof(blobInfo));
        return services.AddDataProtectionWithVaultAndStorage(blobInfo: blobInfo);
    }

    public static IServiceCollection AddDataProtectionWithVaultAndStorage(this IServiceCollection services, VaultConfig vaultConfig = null, AzKeyBlobInfo blobInfo = null)
        => services.AddDataProtection(builder =>
        {
            if (blobInfo != null)
            {
                if (string.IsNullOrEmpty(blobInfo.BlobName) || string.IsNullOrEmpty(blobInfo.ConnectionString))
                    throw new ArgumentNullException(nameof(blobInfo));

                var container = new AccountManager(blobInfo.ConnectionString).GetOrCreateContainerAsync(blobInfo.ContainerName ?? "ProtectedKeys").GetAwaiter().GetResult();
                builder.PersistKeysToAzureBlobStorage(container.GetUrlAsync(blobInfo.BlobName).GetAwaiter().GetResult());
            }

            if (vaultConfig != null)
            {
                builder.ProtectKeysWithAzureKeyVault(VaultClientCreator.Create(vaultConfig.CreateCredentials()), vaultConfig.VaultName);
            }
        });

    #endregion Methods
}

public sealed class AzKeyBlobInfo
{
    #region Properties

    public string BlobName { get; set; }

    public string ConnectionString { get; set; }

    public string ContainerName { get; set; }

    #endregion Properties
}