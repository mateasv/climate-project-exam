using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace XamarinBase.Services
{
    /// <summary>
    /// Interface used in the Android project, to implement an insecure handler, which is used
    /// to setup the Http Client in the DatabaseService
    /// </summary>
    public interface IHTTPClientHandlerCreationService
    {
        HttpClientHandler GetInsecureHandler();
    }
}
