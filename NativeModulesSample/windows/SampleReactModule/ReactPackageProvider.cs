using Microsoft.ReactNative.Bridge;
using Microsoft.ReactNative.Managed;

namespace SampleReactModule
{
    public sealed class ReactPackageProvider : IReactPackageProvider
    {
        public void CreatePackage(IReactPackageBuilder packageBuilder)
        {
            packageBuilder.AddAttributedModules();
        }
    }
}
