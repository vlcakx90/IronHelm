using Microsoft.CodeAnalysis;

namespace CodeBuilder
{
    public sealed class MBOptionsStaged : MBOptions
    {
        public override MB_Type mBType => MB_Type.STAGED;

        public MB_Get_Staged? mB_Get_Staged = null;

        public override string GetAssemblyFullName()
        {
            return assemblyNameBase + "_Staged" + base.GetExt(compileType);
        }
    }

    public sealed class MBOptionsStageless : MBOptions
    {
        public override MB_Type mBType => MB_Type.STAGELESS;

        public MB_Get_Staged? mB_Get_Staged = null;

        public override string GetAssemblyFullName()
        {
            return assemblyNameBase + "_Stageless" + base.GetExt(compileType);
        }
    }

    public abstract class MBOptions : CompileOptions
    {
        public abstract MB_Type mBType { get; }
        public MB_Format? mB_Format = null;
        public MB_Execute? mB_Execute = null;
        public MB_CleanUp? mB_CleanUp = null;
    }
    
    //public sealed class KnightEgress : KnightOptions
    //{
    //    public string? ip = null;
    //    public int? port = null;
    //    public bool tls = false;
    //    public string[]? header4passwd = null;
    //    public string? headerpasswd = null;
    //    public string[]? getpaths = null;
    //    public string[]? postpaths = null;
    //    public override string GetAssemblyFullName()
    //    {
    //        return assemblyNameBase + "_Egress" + base.GetExt(compileType);
    //    }
    //}
    //public sealed class KnightSmbServer: KnightOptions
    //{
    //    public string? pipename = null;
    //    public override string GetAssemblyFullName()
    //    {
    //        return assemblyNameBase + "_Namedpipe_Server" + base.GetExt(compileType);
    //    }
    //}
    //public sealed class KnightSmbClient : KnightOptions
    //{
    //    public string? pipename = null;
    //    public string? hostname = null;
    //    public override string GetAssemblyFullName()
    //    {
    //        return assemblyNameBase + "_Namedpipe_Client" + base.GetExt(compileType);
    //    }
    //}
    //public sealed class KnightTcpServer : KnightOptions
    //{
    //    public bool loopback = false;
    //    public int? port = null;
    //    public override string GetAssemblyFullName()
    //    {
    //        return assemblyNameBase + "_Tcp_Server" + base.GetExt(compileType);
    //    }
    //}
    //public sealed class KnightTcpClient: KnightOptions
    //{
    //    public string? remoteaddress = null;
    //    public int? port = null;
    //    public override string GetAssemblyFullName()
    //    {
    //        return assemblyNameBase + "_Tcp_Client" + base.GetExt(compileType);
    //    }
    //}

    public sealed class KnightOptions : CompileOptions
    {
        public Knight_Type? KnightType = null;

        public override string GetAssemblyFullName()
        {
            return assemblyNameBase + base.GetExt(compileType);
        }
    }

    public sealed class SharedArsenalOptions : CompileOptions
    {
        public override string GetAssemblyFullName()
        {
            return assemblyNameBase + base.GetExt(compileType);
        }
    }

    public abstract class CompileOptions
    {
        public string assemblyNameBase = string.Empty;
        public string outputDirectory = string.Empty;
        public string projectSourcePath = string.Empty;
        public List<MetadataReference> metaRefs = new List<MetadataReference>();
        public List<string> macros = new List<string>();
        public IEnumerable<ResourceDescription>? dependencyResource = new List<ResourceDescription>();
        public Compile_Type? compileType = null;
        public abstract string GetAssemblyFullName();
        //public string GetAssemblyFullName()
        //{
        //    return assemblyNameBase + GetExt(compileType);
        //}
        public string GetOutputFullPath()
        {
            return outputDirectory + GetAssemblyFullName();
        }

        public OutputKind GetOutputKind()
        {
            switch (compileType)
            {
                case Compile_Type.EXE:
                    return OutputKind.ConsoleApplication;
                case Compile_Type.DLL:
                    return OutputKind.DynamicallyLinkedLibrary;
                case Compile_Type.SRV:
                    return OutputKind.WindowsApplication;
                default:
                    return OutputKind.ConsoleApplication;
            }
        }

        public string GetExt(Compile_Type? compileType)
        {
            switch (compileType)
            {
                case Compile_Type.EXE:
                    return ".exe";
                case Compile_Type.DLL:
                    return ".dll";
                case Compile_Type.SRV:
                    return ".exe";
                default:
                    return ".exe";
            }
        }
    }

    public enum MB_Type
    {
        STAGED,
        STAGELESS
    }

    public enum MB_Get_Staged
    {
        HttpUri,
        WebSocketUri,
        QUICUri
    }

    public enum MB_Get_Stageless
    {
        Resource,
        Blob,
        File
    }

    public enum MB_Format
    {
        None,
        Base64,
        Aes256,
        Aes128,
    }
    public enum MB_Execute
    {
        ExecuteAssembly,
        Shellcode_SelfInject,
        Shellcode_SelfRemoteInject,
        Shellcode_SelfSpawnInject,
        DllExported_RemoteInject,
    }
    public enum MB_CleanUp
    {
        None,
        PowerShell,
        Cmd
    }

    public enum Knight_Type
    {
        Egress,
        P2P_TCP_SERVER,
        P2P_TCP_CLIENT,
        P2P_NAMEDPIPE_SERVER,
        P2P_NAMEDPIPE_CLIENT
    }

    public enum SharedArsenal_Interop
    {
        PI_Kernel32, // Will probably remove later
        DI_Kernel32,
        DI_Ntdll,
        DI_Syscalls
    }
    public enum Compile_Type
    {
        EXE,
        DLL,
        SRV,
        BIN
    }
}
