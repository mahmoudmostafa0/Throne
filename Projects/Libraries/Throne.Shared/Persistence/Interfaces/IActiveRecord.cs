namespace Throne.Shared.Persistence.Interfaces
{
    public interface IActiveRecord
    {
        void Create();

        void Update();

        void Delete();
    }
}