namespace HBDStack.AspNetCore.ViewModels;

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
    #region Properties

    public string CreatedBy { get; private set; }

    public DateTimeOffset CreatedOn { get; private set; }

    public string UpdatedBy { get; private set; }

    public DateTimeOffset? UpdatedOn { get; private set; }

    #endregion Properties
}

public abstract class ViewDetailsModel : ViewDetailsModel<Guid>, IViewDetailsModel
{
}