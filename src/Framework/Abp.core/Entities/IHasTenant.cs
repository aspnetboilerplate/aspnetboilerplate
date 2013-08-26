namespace Abp.Entities
{
    public interface IHasTenant
    {
        /// <summary>
        /// The account which this entity is belong to.
        /// </summary>
        int AccountId { get; set; }
    }
}