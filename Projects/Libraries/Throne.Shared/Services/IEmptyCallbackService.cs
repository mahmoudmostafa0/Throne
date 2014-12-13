using System.ServiceModel;

namespace Throne.Shared.Services
{
    public interface IEmptyCallbackService
    {
        [OperationContract]
        void Ping();
    }
}