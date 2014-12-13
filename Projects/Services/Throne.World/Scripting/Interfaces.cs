
namespace Throne.World.Scripting
{
    public interface IScript
    {
        bool Init();
    }

    public interface IAutoLoader
    {
        void AutoLoad();
    }
}