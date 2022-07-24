namespace HBDStack.Web.Swagger;

public class SwaggerInfo
{
    #region Properties

    public static string Name => "Swagger";

    public string Contact { get; set; } = default!;

    public string Description { get; set; } = default!;

    public string License { get; set; } = default!;

    public string TermsOfService { get; set; } = default!;

    public string Title { get; set; } = default!;

    public string Version { get; set; } = default!;

    #endregion Properties
}