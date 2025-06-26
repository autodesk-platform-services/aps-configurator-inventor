using Autodesk.Forge.Core;
using Autodesk.SDKManager;
using Microsoft.Extensions.Options;
using WebApplication.Definitions;
using WebApplication.Utilities;

namespace WebApplication.Services
{
    public class SDKManagerProvider
    {
        ForgeConfiguration Configuration;

        public SDKManagerProvider(IOptions<ForgeConfiguration> optionsAccessor)
        {
            Configuration = optionsAccessor.Value.Validate();
        }

        public SDKManager ProvideSDKManager()
        {
            SDKManager sdkManager = SdkManagerBuilder.Create().Build();

            // Set environment from AuthenticationAddress
            sdkManager.SetEnvFromAuthAddress(Configuration.AuthenticationAddress.AbsoluteUri);

            return sdkManager;
        }
    }
}
