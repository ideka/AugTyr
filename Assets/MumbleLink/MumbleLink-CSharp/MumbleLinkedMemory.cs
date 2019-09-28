using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace MumbleLink_CSharp
{
	[CLSCompliant( false )]
	[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode )]
	public struct MumbleLinkedMemory {

		public uint UiVersion;

		public uint UiTick;

		[MarshalAs( UnmanagedType.ByValArray, SizeConst = 3 )]
		public float[] FAvatarPosition;

		[MarshalAs( UnmanagedType.ByValArray, SizeConst = 3 )]
		public float[] FAvatarFront;

		[MarshalAs( UnmanagedType.ByValArray, SizeConst = 3 )]
		public float[] FAvatarTop;

		[MarshalAs( UnmanagedType.ByValTStr, SizeConst = 256 )]
		public string Name;

		[MarshalAs( UnmanagedType.ByValArray, SizeConst = 3 )]
		public float[] FCameraPosition;

		[MarshalAs( UnmanagedType.ByValArray, SizeConst = 3 )]
		public float[] FCameraFront;

		[MarshalAs( UnmanagedType.ByValArray, SizeConst = 3 )]
		public float[] FCameraTop;

		[MarshalAs( UnmanagedType.ByValTStr, SizeConst = 256 )]
		public string Identity;

		public uint ContextLen;

		[MarshalAs( UnmanagedType.ByValArray, SizeConst = 256 )]
		public byte[] Context;

		[MarshalAs( UnmanagedType.ByValTStr, SizeConst = 2048 )]
		public string Description;


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