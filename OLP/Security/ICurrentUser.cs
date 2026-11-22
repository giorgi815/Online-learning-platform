namespace OLP.Security
{
    public interface ICurrentUser
    {
        Guid AccountId { get; }
        Task<Guid> ProfileIdAsync();
    }
}
