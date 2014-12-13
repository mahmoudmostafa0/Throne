namespace Throne.Shared.Runtime
{
    public interface IMemberwiseSerializable<T>
    {
        T Serialize();
    }
}