using System;
using System.Net.Security;
using System.ServiceModel;

namespace Throne.Framework.Services.Account
{
    [ServiceContract(ProtectionLevel = ProtectionLevel.None, SessionMode = SessionMode.Required,
        CallbackContract = typeof (IEmptyCallbackService))]
    public interface IAccountService
    {
        [OperationContract]
        AccountData GetAccount(int session);

        [OperationContract]
        bool Authorize(int session, int password);

        [OperationContract]
        void SetOnline(Int32 guid, Boolean value);
    }
}