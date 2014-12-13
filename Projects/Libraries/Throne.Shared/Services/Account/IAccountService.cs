using System;
using System.Net.Security;
using System.ServiceModel;

namespace Throne.Shared.Services.Account
{
    [ServiceContract(ProtectionLevel = ProtectionLevel.None, SessionMode = SessionMode.Required,
        CallbackContract = typeof (IEmptyCallbackService))]
    public interface IAccountService
    {
        [OperationContract]
        AccountData? GetAccount(int session);

        [OperationContract]
        Boolean Authorize(int session, int password);
    }
}