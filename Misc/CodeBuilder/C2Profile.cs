
namespace CodeBuilder
{
    public sealed class C2Profile
    {
        public string Name { get; set; } = string.Empty; // same as file name
        public string Description { get; set; } = string.Empty;
        public Http_Profile Http { get; set; } = new Http_Profile();
        public SMB_Profile SMB { get; set; } = new SMB_Profile();
        public TCP_Profile TCP { get; set; } = new TCP_Profile();
        public SharedArsenal_Profile SharedArsenal { get; set; } = new SharedArsenal_Profile();
        public MB_Staged_Profile MB_Staged { get; set; } = new MB_Staged_Profile();
        public MB_Stageless_Profile MB_Stageless { get; set; } = new MB_Stageless_Profile();
        //public Knight_Profile Knight { get; set; } = new Knight_Profile();

        //-------------------- Networking
        public sealed class Http_Profile
        {
            public string Ip { get; set; } = "127.0.0.1";
            public int Port { get; set; } = 8080;
            public bool Tls { get; set; } = false;
            public int Sleep { get; set; } = 2;// interval
            public int Jitter { get; set; } = 1;
            public string PasswdHeader { get; set; } = "HelmOfIron";
            public string Passwd { get; set; } = "HelmOfIron";
            public string MetadataHeader { get; set; } = "Authorization";
            public string MetadataValue { get; set; } = "Bearer "; // Keep trailing space

            //public string[] ServerHeaders { get; set; } = { "X-Header1 Value1" }; // !!! Need to string.split() each header to get header/value when setting in Castle !!!
            //public string[] ClientHeaders { get; set; } = { "X-Header1 Value1" }; // !!! Need to string.split() each header to get header/value when setting in Knight !!!
            public string[] GetPaths { get; set; } = { "/index.php" };
            public string[] PostPaths { get; set; } = { "/submit.php" };
        }

        //public sealed class WebSocket
        //{
        //    public int Sleep { get; set; } = 2;// interval
        //    public int Jitter { get; set; } = 1;

        //    public string Passwd { get; set; } = "HelmOfIron";
        //    public string[] GetPaths { get; set; } = { "/index.php" };
        //    public string[] PostPaths { get; set; } = { "/submit.php" };
        //}

        //public sealed class QUIC
        //{
        //    public int Sleep { get; set; } = 2;// interval
        //    public int Jitter { get; set; } = 1;

        //    public string Passwd { get; set; } = "HelmOfIron";
        //    public string[] GetPaths { get; set; } = { "/index.php" };
        //    public string[] PostPaths { get; set; } = { "/submit.php" };
        //}

        public sealed class SMB_Profile
        {
            public string Pipename { get; set; } = "ironhelm";
        }

        public sealed class TCP_Profile
        {
            public bool Loopback { get; set; } = true;
            public int Port { get; set; } = 4444;
        }

        //-------------------- Payload
        public sealed class SharedArsenal_Profile
        {
            public string ResourceName { get; set; } = "SharedArsenal.dll";
            //public SharedArsenal_Interop InterOp { get; set; } = SharedArsenal_Interop.PI_Kernel32;
            //public string[] ReMap { get; set; } = { "kernel32.dll", "ntdll.dll" };
        }
        public sealed class MB_Staged_Profile
        {
            //public Compile_Type CompType { get; set; } = Compile_Type.EXE;
            public MB_Get_Staged Get { get; set; } = MB_Get_Staged.HttpUri;
            public string GetUri { get; set; } = "http://127.0.0.1/a";
            public MB_Format Format { get; set; } = MB_Format.None;
            public MB_Execute Execute { get; set; } = MB_Execute.ExecuteAssembly;
            public MB_CleanUp CleanUp { get; set; } = MB_CleanUp.None;
        }

        public sealed class MB_Stageless_Profile
        {
            //public Compile_Type CompType { get; set; } = Compile_Type.EXE;
            public MB_Get_Stageless Get { get; set; } = MB_Get_Stageless.Resource;
            public string GetName { get; set; } = "knight.exe";
            public MB_Format Format { get; set; } = MB_Format.None;
            public MB_Execute Execute { get; set; } = MB_Execute.ExecuteAssembly;
            public MB_CleanUp CleanUp { get; set; } = MB_CleanUp.None;
        }

        //public sealed class Knight_Profile
        //{
        //    //public Compile_Type CompType { get; set; } = Compile_Type.EXE;
        //}
    }
}