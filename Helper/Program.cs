using System.Runtime.InteropServices;
using System.IO.Pipes;
using System.IO;
using System;

namespace EzUnlock
{
    internal class Helper
    {
        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern bool EzUnlockFileW(string path);

        [DllImport("Core.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern bool EzDeleteFileW(string path);

        internal enum ReturnCode
        {
            NullPath = -3,
            InvalidOperation,
            IPCError
        }

        public static int Main()
        {
            using (var client = new NamedPipeClientStream("EzUnlock"))
            {
                client.Connect();

                using (var reader = new StreamReader(client))
                using (var writer = new StreamWriter(client))
                {
                    writer.AutoFlush = true;
                    try
                    {
                        Func<string, bool> operationFunc = null;
                        while (true)
                        {
                            var pathOrCmd = reader.ReadLine();
                            if (pathOrCmd == "UNLOCK")
                            {
                                operationFunc = EzUnlockFileW;
                                continue;
                            }
                            else if (pathOrCmd == "DELETE")
                            {
                                operationFunc = EzDeleteFileW;
                                continue;
                            }
                            else if (pathOrCmd == null)
                                return (int)ReturnCode.NullPath;
                            else
                            {
                                if (operationFunc == null)
                                    return (int)ReturnCode.InvalidOperation;
                            }

                            bool result = false;
                            try
                            {
                                result = operationFunc(pathOrCmd);
                            }
                            catch (Exception)
                            {
                            }
                            if (result)
                                writer.WriteLine("SUCCESS");
                            else
                                writer.WriteLine("FAILURE");
                            client.WaitForPipeDrain();
                        }
                    }
                    catch (IOException)
                    {
                        return (int)ReturnCode.IPCError;
                    }
                }
            }
        }
    }
}