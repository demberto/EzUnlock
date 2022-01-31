#ifndef EZUNLOCK_H_
#define EZUNLOCK_H_

#include "pch.h"

#ifdef UNICODE
#define EzDeleteFile EzDeleteFileW
#else
#define EzDeleteFile EzDeleteFileA
#endif // UNICODE

#ifdef EZUNLOCK_EXPORTS
#define SYMBOL extern "C" __declspec(dllexport)
#else
#define SYMBOL extern "C" __declspec(dllimport)
#endif

static const bool _EzDeleteFile(const tstring&);

/// <summary>
/// Tries to unlock a file-like object. Success is not guaranteed.
/// Use <seealso cref="EzUnlockFileW"/> for Unicode strings.
/// </summary>
/// <example>
/// <code>
/// bool result = EzUnlockFileA("C:\\MyFile.exe");
/// </code>
/// </example>
/// <param name="path">Path of the file-like object to be unlocked</param>
/// <returns>Whether the operation was successful</returns>
SYMBOL bool EzUnlockFileA(const char* path);

/// <summary>
/// Tries to unlock a file-like object. Success is not guaranteed.
/// Use <seealso cref="EzUnlockFileA"/> for ANSI strings.
/// </summary>
/// <example>
/// <code>
/// bool result = EzUnlockFileW(L"C:\\MyFile.exe");
/// </code>
/// </example>
/// <param name="path">Path of the file-like object to be unlocked</param>
/// <returns>Whether the operation was successful</returns>
SYMBOL bool EzUnlockFileW(const wchar_t* path);

/// <summary>
/// Tries to delete a file-like object normally, first. If it fails, the file
/// is unlocked by calling <seealso cref="EzUnlockFileA"/> first and then 
/// deleted. Success is not guaranteed. Use <seealso cref="EzDeleteFileW"/>
/// for Unicode strings.
/// </summary>
/// <example>
/// <code>
/// bool result = EzDeleteFileA("C:\\MyFile.exe");
/// </code>
/// </example>
/// <param name="path">Path of the file-like object to be deleted</param>
/// <returns>Whether the operation was successful</returns>
SYMBOL bool EzDeleteFileA(const char* path);

/// <summary>
/// Tries to delete a file-like object normally, first. If it fails, the file
/// is unlocked by calling <seealso cref="EzUnlockFileA"/> first and then
/// deleted. Success is not guaranteed. Use <seealso cref="EzDeleteFileA"/>
/// for ANSI strings.
/// </summary>
/// <example>
/// <code>
/// bool result = EzDeleteFileW(L"C:\\MyFile.exe");
/// </code>
/// </example>
/// <param name="path">Path of the file-like object to be deleted</param>
/// <returns>Whether the operation was successful</returns>
SYMBOL bool EzDeleteFileW(const wchar_t* path);

#endif // !EZUNLOCK_H_
