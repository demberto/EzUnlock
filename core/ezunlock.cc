#include "ezunlock.h"

#include "pch.h"

#include <filesystem>
#include <string>

static const bool _EzDeleteFile(const tstring& tpath) {
	const auto& file = std::make_unique<unlocker::File>(tpath);
	if (!file->Delete())
		return file->ForceDelete();
	return true;
}

extern "C" __declspec(dllexport) bool EzUnlockFileA(const char* path) {
#ifdef UNICODE
	const std::wstring& tpath = std::filesystem::path(path).wstring();
#else
	const std::string& tpath{ path };
#endif // UNICODE

	return unlocker::File(tpath).Unlock();
}

extern "C" __declspec(dllexport) bool EzUnlockFileW(const wchar_t* path) {
#ifdef UNICODE
	const std::wstring& tpath{ path };
#else
	const std::string& tpath = std::filesystem::path(path).string();
#endif // UNICODE

	return unlocker::File(tpath).Unlock();
}

extern "C" __declspec(dllexport) bool EzDeleteFileA(const char* path) {
#ifdef UNICODE
	const auto& tpath = std::filesystem::path(path).wstring();
#else
	const auto& tpath{ path };
#endif // UNICODE

	return _EzDeleteFile(tpath);
}

extern "C" __declspec(dllexport) bool EzDeleteFileW(const wchar_t* path) {
#ifdef UNICODE
	const std::wstring& tpath{ path };
#else
	const std::string& tpath = std::filesystem::path(path).string();
#endif // UNICODE

	return _EzDeleteFile(tpath);
}
