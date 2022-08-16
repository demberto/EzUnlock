#include "EzUnlock.Native.h"
#include <msclr/marshal_cppstd.h>

using namespace msclr::interop;

namespace EzUnlock {
	bool Unlocker::Unlock(System::String^ path)
	{
		const auto &path_tstring = marshal_as<tstring>(path);

		if (System::IO::File::Exists(path))
		{
			return unlocker::File(path_tstring).Unlock();
		}
		else if (System::IO::Directory::Exists(path))
		{
			return unlocker::Dir(path_tstring).Unlock();
		}
		return false;
	}

	bool Unlocker::Delete(System::String^ path)
	{
		const auto& path_tstring = marshal_as<tstring>(path);

		if (System::IO::File::Exists(path))
		{
			return unlocker::File(path_tstring).ForceDelete();
		}
		else if (System::IO::Directory::Exists(path))
		{
			return unlocker::Dir(path_tstring).Delete();
		}
		return false;
	}
}