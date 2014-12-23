namespace Throne.Framework.Persistence.Interfaces
{
    public interface IActiveRecord
    {
        void Create();

        void Update();

        void Delete();
    }
}