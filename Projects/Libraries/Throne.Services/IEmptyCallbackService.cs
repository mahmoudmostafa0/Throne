using System.ServiceModel;

namespace Throne.Framework.Services
{
    public interface IEmptyCallbackService
    {
        [OperationContract]
        void Ping();
    }
}