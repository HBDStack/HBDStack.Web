using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HBDStack.AspNetCore.Handlers;

public sealed class ClientCertValidationHandler : ActionValidationHandler
{
    #region Constructors

    public ClientCertValidationHandler(params string[] thumbprints) => Thumbprints = thumbprints;

    #endregion Constructors

    #region Properties

    public string[] Thumbprints { get; }

    #endregion Properties

    #region Methods

    protected override IActionResult Validate(ActionExecutingContext context)
    {
        if (Thumbprints is not {Length: > 0}) return null;

        var cert = context.GetClientCert() ?? context.GetForwardingCertificate();
        if (cert == null) return new UnauthorizedResult();

        return Thumbprints.Any(t => cert.Thumbprint?.Equals(t, StringComparison.OrdinalIgnoreCase)==true) ? null : new UnauthorizedResult();
    }

    #endregion Methods
}