using System;
using System.Runtime.InteropServices;

namespace UnitTests
{
    [ComImport, Guid("F414C260-6AC0-11CF-B6D1-00AA00BBBB58")]
    internal class JScriptEngine
    {
    }

    [ComImport, Guid("B54F3741-5B07-11cf-A4B0-00AA004A55E8")]
    internal class VBScriptEngine
    {
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("DB01A1E3-A42B-11cf-8F20-00805F2CD064")]
    internal interface IActiveScriptSite
    {
        [PreserveSig] int GetLCID(out uint id);
        [PreserveSig] int GetItemInfo([In, MarshalAs(UnmanagedType.BStr)] string pstrName, [In, MarshalAs(UnmanagedType.U4)] uint dwReturnMask, [Out, MarshalAs(UnmanagedType.IUnknown)] out object item, out IntPtr ppti);
        [PreserveSig] int GetDocVersionString(out string v);
        void OnScriptTerminate(ref object result, ref EXCEPINFO info);
        void OnStateChange(SCRIPTSTATE state);
        void OnScriptError([In] IActiveScriptError err);
        void OnEnterScript();
        void OnLeaveScript();
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("BB1A2AE1-A4F9-11cf-8F20-00805F2CD064")]
    internal interface IActiveScript
    {
        void SetScriptSite([In, MarshalAs(UnmanagedType.Interface)] IActiveScriptSite site);
        void GetScriptSite(ref Guid riid, out IntPtr ppvObject);
        void SetScriptState(SCRIPTSTATE ss);
        void GetScriptState(out uint ss);
        void Close();
        void AddNamedItem([In, MarshalAs(UnmanagedType.BStr)] string pstrName, [In, MarshalAs(UnmanagedType.U4)] uint dwFlags);
        void AddTypeLib(ref Guid rguidTypeLib, uint dwMajor, uint dwMinor, uint dwFlags);
        void GetScriptDispatch(string pstrItemName, [Out, MarshalAs(UnmanagedType.IDispatch)] out object ppdisp);
        void GetCurrentScriptThreadiD(out uint id);
        void GetScriptThreadID(uint threadid, out uint id);
        void GetScriptThreadState(uint id, out uint state);
        void InterruptScriptThread(uint id, ref EXCEPINFO info, uint flags);
        void Clone(out IActiveScript item);
    };

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("BB1A2AE2-A4F9-11cf-8F20-00805F2CD064")]
    internal interface IActiveScriptParse
    {
        void InitNew();
        void AddScriptlet(string defaultName,
                    string code,
                    string itemName,
                    string subItemName,
                    string eventName,
                    string delimiter,
                    uint sourceContextCookie,
                    uint startingLineNumber,
                    uint flags,
                    out string name,
                    out EXCEPINFO info);

        [PreserveSig]
        int ParseScriptText(
                string code,
                string itemName,
                IntPtr context,
                string delimiter,
                uint sourceContextCookie,
                uint startingLineNumber,
                SCRIPTITEM flags,
                [Out, MarshalAs(UnmanagedType.Struct)] out object result,
                out EXCEPINFO info);
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("EAE1BA61-A4ED-11CF-8F20-00805F2CD064")]
    internal interface IActiveScriptError
    {
        void GetExceptionInfo([Out, MarshalAs(UnmanagedType.Struct)] out EXCEPINFO info);
        void GetSourcePosition(
            [Out, MarshalAs(UnmanagedType.U4)] out uint sourceContext,
            [Out, MarshalAs(UnmanagedType.U4)] out uint lineNumber,
            [Out, MarshalAs(UnmanagedType.U4)] out int characterPosition);
        void GetSourceLineText(out string sourceLine);
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("4954E0D0-FBC7-11D1-8410-006008C3FBFC")]
    internal interface IActiveScriptProperty
    {
        void GetProperty(SCRIPTPROP dwProperty, IntPtr pvarIndex, [Out] out object pvarValue);
        void SetProperty(SCRIPTPROP dwProperty, IntPtr pvarIndex, [In] ref object pvarValue);
    }

    

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct EXCEPINFO
    {
        public ushort wCode;
        public ushort wReserved;
        [MarshalAs(UnmanagedType.BStr)]
        public string bstrSource;
        [MarshalAs(UnmanagedType.BStr)]
        public string bstrDescription;
        [MarshalAs(UnmanagedType.BStr)]
        public string bstrHelpFile;
        public uint dwHelpContext;
        public IntPtr pvReserved;
        public IntPtr pfnDeferredFillIn;
        [MarshalAs(UnmanagedType.Error)]
        public int scode;
    }

    internal enum SCRIPTSTATE
    {
        UNINITIALIZED = 0,
        INITIALIZED = 5,
        STARTED = 1,
        CONNECTED = 2,
        DISCONNECTED = 3,
        CLOSED = 4
    }

    [Flags]
    internal enum SCRIPTITEM : uint
    {
        ISVISIBLE = 0x00000002,
        ISSOURCE = 0x00000004,
        GLOBALMEMBERS = 0x00000008,
        ISEXPRESSION = 0x00000020,
        ISPERSISTENT = 0x00000040,
        CODEONLY = 0x00000200,
        NOCODE = 0x00000400,
        ALL_FLAGS = ISSOURCE |
                        ISVISIBLE |
                        ISPERSISTENT |
                        GLOBALMEMBERS |
                        NOCODE |
                        CODEONLY
    }

    internal enum SCRIPTTHREADSTATE : uint
    {
        NOTINSCRIPT = 0,
        RUNNING = 1,
    }

    [Flags]
    internal enum SCRIPTINFOFLAGS : uint
    {
        IUNKNOWN = 0x00000001,
        ITYPEINFO = 0x00000002,
        ALL_FLAGS = IUNKNOWN | ITYPEINFO
    }

    internal enum SCRIPTPROP : uint
    {
        INTEGERMODE = 0x00003000,
        STRINGCOMPAREINSTANCE = 0x00003001,
        ABBREVIATE_GLOBALNAME_RESOLUTION = 0x70000002,
        INVOKEVERSIONING = 0x00004000,
    }

    internal enum SCRIPTLANGUAGEVERSION : int
    {
        SCRIPTLANGUAGEVERSION_DEFAULT = 0,
        V5_7 = 1,
        V5_8 = 2,
    }
}
