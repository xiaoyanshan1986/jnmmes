using System;
using System.Collections.Generic;

using System.Text;
using System.Runtime.InteropServices;
namespace IV.Common
{
    public class LPTControl
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct OVERLAPPED
        {
            int Internal;
            int InternalHigh;
            int Offset;
            int OffSetHigh;
            int hEvent;
        }

        [DllImport("kernel32.dll")]
        private static extern int CreateFile(
        string lpFileName,
        uint dwDesiredAccess,
        int dwShareMode,
        int lpSecurityAttributes,
        int dwCreationDisposition,
        int dwFlagsAndAttributes,
        int hTemplateFile
        );


        [DllImport("kernel32.dll")]
        private static extern bool WriteFile(
        int hFile,
        byte[] lpBuffer,
        int nNumberOfBytesToWrite,
        out   int lpNumberOfBytesWritten,
        out   OVERLAPPED lpOverlapped
        );


        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(
        int hObject
        );

        private int iHandle;
        public bool Open()
        {
            iHandle = CreateFile("lpt1", 0x40000000, 0, 0, 3, 0, 0);
            if (iHandle != -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Write(String Mystring)
        {
            if (iHandle != -1)
            {
                int i;
                OVERLAPPED x;
                byte[] mybyte = System.Text.Encoding.Default.GetBytes(Mystring);
                return WriteFile(iHandle, mybyte, mybyte.Length, out  i, out  x);
            }
            else
            {
                throw new Exception("打印機端口未打開!");
            }
        }

        public bool Close()
        {
            return CloseHandle(iHandle);
        }
    }
}
