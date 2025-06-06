using Autodesk.SDKManager;

namespace WebApplication.Definitions
{
    public static class SdkManagerExtensions
    {
        public static void SetEnvFromAuthAddress(this SDKManager sdkManager, string authAddress)
        {
            sdkManager.ApsConfiguration = new ApsConfiguration(
                  authAddress.Contains("-dev") ? AdskEnvironment.Dev : (
                  authAddress.Contains("-stg") ? AdskEnvironment.Stg : AdskEnvironment.Prd
            ));
        }
    }
}