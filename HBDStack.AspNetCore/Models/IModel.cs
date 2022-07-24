namespace HBDStack.AspNetCore.Models;

public interface IModel<TKey>
{
    #region Properties

    TKey Id { get; set; }

    byte[] RowVersion { get; set; }

    #endregion Properties
}

public interface IModel : IModel<Guid?>
{
}

public abstract class Model<TKey> : IModel<TKey>
{
    #region Properties

    public virtual TKey Id { get; set; }

    public virtual byte[] RowVersion { get; set; }

    #endregion Properties
}

public abstract class Model : Model<Guid?>, IModel
{
}