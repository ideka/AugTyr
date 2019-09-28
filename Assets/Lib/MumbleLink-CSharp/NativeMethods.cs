using System;
using System.Runtime.InteropServices;

namespace MumbleLink_CSharp
{
    [Flags]
    internal enum FileMapAccess : uint
    {
        FileMapCopy = 0x0001,
        FileMapWrite = 0x0002,
        FileMapRead = 0x0004,
        FileMapAllAccess = 0x001f,
        fileMapExecute = 0x0020,
    }

    [Flags]
    internal enum FileMapProtection : uint
    {
        PageReadonly = 0x02,
        PageReadWrite = 0x04,
        PageWriteCopy = 0x08,
        PageExecuteRead = 0x20,
        PageExecuteReadWrite = 0x40,
        SectionCommit = 0x8000000,
        SectionImage = 0x1000000,
        SectionNoCache = 0x10000000,
        SectionReserve = 0x4000000,
    }


    internal static class NativeMethods
    {
        [DllImport("Kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr CreateFileMapping(IntPtr hFile, IntPtr lpAttributes, FileMapProtection flProtect, Int32 dwMaxSizeHi, Int32 dwMaxSizeLow, string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr OpenFileMapping(FileMapAccess DesiredAccess, bool bInheritHandle, string lpName);

        [DllImport("Kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr MapViewOfFile(IntPtr hFileMapping, FileMapAccess dwDesiredAccess, Int32 dwFileOffsetHigh, Int32 dwFileOffsetLow, Int32 dwNumberOfBytesToMap);


        [DllImport("kernel32", SetLastError = true)]
        internal static extern bool CloseHandle(IntPtr hFile);

        [DllImport("kernel32")]
        internal static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);
    }
}