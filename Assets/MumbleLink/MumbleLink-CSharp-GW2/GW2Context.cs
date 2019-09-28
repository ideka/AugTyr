using System;
using System.Runtime.InteropServices;
using System.Text;

namespace MumbleLink_CSharp_GW2
{

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct GW2Context
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 28)]
        public readonly byte[] ServerAddress;

        public readonly uint MapId;
        public readonly uint MapType;
        public readonly uint ShardId;
        public readonly uint Instance;
        public readonly uint BuildId;


        public override string ToString()
        {
            var str = new StringBuilder();
            var t = Enum.Parse(typeof(GW2Race), 1.ToString());
            str.Append("Server Address : [");
            for (int i = 0; i < ServerAddress.Length; i++)
            {
                str.Append(ServerAddress[i].ToString());
                if (i != ServerAddress.Length - 1)
                {
                    str.Append(", ");
                }
            }

            str.AppendLine("]");

            str.AppendLine("MapId : " + MapId);
            str.AppendLine("MapType : " + MapType);
            str.AppendLine("SharId : " + ShardId);
            str.AppendLine("Instance : " + Instance);
            str.AppendLine("BuildId : " + BuildId);

            return str.ToString();
        }
    }
}