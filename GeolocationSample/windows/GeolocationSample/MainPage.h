#pragma once
#include "MainPage.g.h"
#include <winrt/Microsoft.ReactNative.h>


namespace winrt::GeolocationSample::implementation
{
    struct MainPage : MainPageT<MainPage>
    {
        MainPage();
    };
}

namespace winrt::GeolocationSample::factory_implementation
{
    struct MainPage : MainPageT<MainPage, implementation::MainPage>
    {
    };
}


