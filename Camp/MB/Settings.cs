//-------------------- Test
//#define DUMMY
// Get Staged
//#define HttpUri
//#define WebSocketUri
//#define QUICUri
// Get Stageless
//#define Resource
//#define Blob
//#define File
//--------------------

namespace MB1
{
    static public class Settings
    {
#if !DUMMY        
        public static readonly string SharedArsenal = "{{R_SharedArsenal}}";
#if HttpUri
        public static readonly string HttpUri = "{{R_HttpUri}}";
#elif WebSocketUri
#elif QUICUri
#elif Resource
        public static readonly string Resource = "{{R_Resource}}";
#elif Blob
        public static readonly string Blob = "{{R_Blob}}";
#elif File
        public static readonly string File = "{{R_File}}";
#endif

        //-------------------- DUMMY for testing
#else
        public static readonly string SharedArsenal = "SharedArsenal.dll";
#if HttpUri
        public static readonly string HttpUri = "http://127.0.0.1:8000/a";
#elif WebSocketUri
#elif QUICUri
#elif Resource
        public static readonly string Resource = "knight";
#elif Blob
        public static readonly string Blob = "";
#elif File
        public static readonly string File = "C:\\Users\\vlcak\\Desktop\\Test\\RosylnOut_Knight_Egress.exe";
        //public static readonly string File = "C:\\Payloads\\LabFiles\\msf_msgbx.bin";
#endif
#endif
    }
}
