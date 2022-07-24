
// ReSharper disable All

namespace HBDStack.AspNetCore.Extensions.Internals
{
    public sealed class GroupData
    {
        public string DisplayName { get; set; }

        public string Id { get; set; }

        public bool? SecurityEnabled { get; set; }
        
    }

    public class GroupResult
    {
        #region Properties

        public IList<GroupData> Value { get; } = new List<GroupData>();

        #endregion Properties
    }
}