namespace Book_Lending_System.Core.Contracts.Persistence
{
    public interface IStoreDbContextInitializer
    {
        Task InitializeAsync();
        Task SeedAsync();
    }
}
