using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace MumbleLink_CSharp
{

    public class MumbleLink : IDisposable
    {
        private const string Name = "MumbleLink";

        private readonly int _memSize;


        private IntPtr _mappedFile;
        private IntPtr _mapView;
        private readonly byte[] _buffer;
        private GCHandle _bufferHandle;
        private readonly UnmanagedMemoryStream _unmanagedStream;


        // Constructor
        public MumbleLink()
        {
            unsafe
            {
                _memSize = Marshal.SizeOf(typeof(MumbleLinkedMemory));

                _mappedFile = NativeMethods.OpenFileMapping(FileMapAccess.FileMapRead, false, Name);

                if (_mappedFile == IntPtr.Zero)
                {
                    _mappedFile = NativeMethods.CreateFileMapping(IntPtr.Zero, IntPtr.Zero, FileMapProtection.PageReadWrite, 0,
                        _memSize, Name);
                    if (_mappedFile == IntPtr.Zero)
                    {
                        throw new Exception("Unable to create file Mapping");
                    }
                }

                _mapView = NativeMethods.MapViewOfFile(_mappedFile, FileMapAccess.FileMapRead, 0, 0, _memSize);

                if (_mapView == IntPtr.Zero)
                {
                    throw new Exception("Unable to map view of file");
                }

                _buffer = new byte[_memSize];

                _bufferHandle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);

                byte* p = (byte*)_mapView.ToPointer();

                _unmanagedStream = new UnmanagedMemoryStream(p, _memSize, _memSize, FileAccess.Read);

            }
        }


        public MumbleLinkedMemory Read()
        {
            _unmanagedStream.Position = 0;
            _unmanagedStream.Read(_buffer, 0, _memSize);
            return
                (MumbleLinkedMemory)Marshal.PtrToStructure(_bufferHandle.AddrOfPinnedObject(), typeof(MumbleLinkedMemory));
        }

        public void Dispose()
        {
            if (_unmanagedStream != null)
                _unmanagedStream.Dispose();
            if (_bufferHandle != null)
                _bufferHandle.Free();
            if (_mapView != IntPtr.Zero)
            {
                NativeMethods.UnmapViewOfFile(_mapView);
                _mapView = IntPtr.Zero;
            }
            if (_mappedFile != IntPtr.Zero)
            {
                NativeMethods.CloseHandle(_mappedFile);
                _mappedFile = IntPtr.Zero;
            }
        }
    }
}