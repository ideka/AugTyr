using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace MumbleLink_CSharp
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct MumbleLinkedMemory
    {
        public readonly uint UiVersion;
        public readonly uint UiTick;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public readonly float[] FAvatarPosition;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public readonly float[] FAvatarFront;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public readonly float[] FAvatarTop;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public readonly string Name;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public readonly float[] FCameraPosition;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public readonly float[] FCameraFront;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public readonly float[] FCameraTop;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public readonly string Identity;
        public readonly uint ContextLen;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public readonly byte[] Context;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2048)]
        public readonly string Description;

        public override string ToString()
        {
            var str = new StringBuilder();

            str.AppendLine("UiVersion : " + UiVersion);
            str.AppendLine("UiTick : " + UiTick);
            str.AppendFormat("FAvatarPosition : [{0}, {1}, {2}]\n", FAvatarPosition[0],
                FAvatarPosition[1], FAvatarPosition[2]);
            str.AppendFormat("FAvatarFront : [{0}, {1}, {2}]\n", FAvatarFront[0],
                FAvatarFront[1], FAvatarFront[2]);
            str.AppendFormat("FAvatarTop : [{0}, {1}, {2}]\n", FAvatarTop[0],
                FAvatarTop[1], FAvatarTop[2]);
            str.AppendLine("Name : " + Name);
            str.AppendFormat("FCameraPosition : [{0}, {1}, {2}]\n", FCameraPosition[0],
                FCameraPosition[1], FCameraPosition[2]);
            str.AppendFormat("FCameraFront : [{0}, {1}, {2}]\n", FCameraFront[0],
                FCameraFront[1], FCameraFront[2]);
            str.AppendFormat("FCameraTop : [{0}, {1}, {2}]\n", FCameraTop[0],
                FCameraTop[1], FCameraTop[2]);

            str.AppendLine("Identity : " + Identity);

            str.AppendLine("ContextLen : " + ContextLen);

            str.AppendLine("Description : " + Description);

            return str.ToString();
        }
    }
}