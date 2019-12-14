#pragma once

#include "App.xaml.g.h"



namespace winrt::NativeModulesSample::implementation
{
    struct App : AppT<App>
    {
        App() noexcept;
    };
} // namespace winrt::NativeModulesSample::implementation


