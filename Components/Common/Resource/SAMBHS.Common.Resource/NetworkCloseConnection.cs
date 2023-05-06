using System.Runtime.InteropServices;
using NET_API_STATUS = System.UInt32;
using LPWSTR = System.String;
using DWORD = System.UInt32;

public class NetworkShareCloser
{
    [DllImport("NetApi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern NET_API_STATUS NetUseDel(
      LPWSTR UncServerName,
      LPWSTR UseName,
      DWORD ForceCond);

    public static void CloseConnection(string UncPath)
    {
        NetUseDel(null, UncPath, 2);
    }
}
