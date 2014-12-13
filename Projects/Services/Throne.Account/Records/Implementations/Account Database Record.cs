using Throne.Shared.Persistence.Interfaces;

namespace Throne.Login.Records.Implementations
{
    public abstract class AccountDatabaseRecord : IActiveRecord
    {
        public virtual void Create()
        {
            AuthServer.Instance.AccountDbContext.PostAsync(x => x.Commit(this));
        }

        public virtual void Update()
        {
            AuthServer.Instance.AccountDbContext.PostAsync(x => x.Update(this));
        }

        public virtual void Delete()
        {
            AuthServer.Instance.AccountDbContext.PostAsync(x => x.Delete(this));
        }
    }
}