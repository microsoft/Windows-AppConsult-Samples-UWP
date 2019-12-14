#pragma once

#include "winrt/Microsoft.ReactNative.Bridge.h"



using namespace winrt::Microsoft::ReactNative::Bridge;

namespace winrt::NativeModulesSample::implementation
{

    struct ReactPackageProvider : winrt::implements<ReactPackageProvider, IReactPackageProvider>
    {
    public: // IReactPackageProvider
        void CreatePackage(IReactPackageBuilder const &packageBuilder) noexcept;
    };

} // namespace winrt::NativeModulesSample::implementation


