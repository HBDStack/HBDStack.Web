namespace HBDStack.Web.Models;

/// <summary>
/// All Props of VIew model should be read-only, private set allow Auto Mapper to write data from Database when querying
/// </summary>
public interface IViewDetailsModel<TKey> : IViewMode<TKey>
{
    #region Properties

    string CreatedBy { get; }

    DateTimeOffset CreatedOn { get; }

    string UpdatedBy { get; }

    DateTimeOffset? UpdatedOn { get; }

    #endregion Properties
}

/// <summary>
/// All Props of VIew model should be read-only, private set allow Auto Mapper to write data from Database when querying
/// </summary>
public interface IViewDetailsModel : IViewDetailsModel<Guid>, IViewMode
{
}

public abstract class ViewDetailsModel<TKey> : ViewModel<TKey>, IViewDetailsModel<TKey>
{

    public string CreatedBy { get; private set; } = default!;

    public DateTimeOffset CreatedOn { get; private set; } = default!;

    public string UpdatedBy { get; private set; } = default!;

    public DateTimeOffset? UpdatedOn { get; private set; } = default!;
    
}

public abstract class ViewDetailsModel : ViewDetailsModel<Guid>, IViewDetailsModel
{
}