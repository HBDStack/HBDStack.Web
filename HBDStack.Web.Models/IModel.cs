namespace HBDStack.Web.Models;

public interface IModel<TKey>
{
    TKey Id { get; set; }

    byte[] RowVersion { get; set; }
}

public interface IModel : IModel<Guid?>
{
}

public abstract class Model<TKey> : IModel<TKey>
{
    public virtual TKey Id { get; set; } = default!;

    public virtual byte[] RowVersion { get; set; }= default!;
}

public abstract class Model : Model<Guid?>, IModel
{
}