#pragma once

#include "unlocker.hpp"

using namespace System;

namespace EzUnlock {
	public ref class Unlocker
	{
	public:
		static bool Unlock(System::String^ path);
		static bool Delete(System::String^ path);
	};
}
