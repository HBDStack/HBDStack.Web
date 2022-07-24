using System.ComponentModel.DataAnnotations;

namespace HBDStack.Web.Models;

public interface IViewMode<out TKey>
{
    [Required] TKey Id { get; }

    byte[] RowVersion { get; }
}

/// <summary>
/// All Props of VIew model should be read-only, private set allow Auto Mapper to write data from Database when querying
/// </summary>
public interface IViewMode : IViewMode<Guid>
{
}

public abstract class ViewModel : ViewModel<Guid>, IViewMode
{
}

public abstract class ViewModel<TKey> : IViewMode<TKey>
{
    public virtual TKey Id { get; private set; } = default!;

    public byte[] RowVersion { get; private set; } = default!;
}