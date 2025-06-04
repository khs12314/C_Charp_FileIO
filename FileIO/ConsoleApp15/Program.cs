using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;


namespace FileIOLib
{
    public enum DebugCode
    {
        Initialize,
        Bind,
        Listen,
        Accept,
        Connect,
        Receive,
        Send,
        Close

    }
    
    public interface IFileIO
    {
        public string FileName { get; set; }

        public string? FilePath { get; set; }
        /// <summary>
        /// 파일을 생성하는 메소드 입니다.
        /// </summary>
        public void Initialize();

        public void Read();
        public string UsePath => FilePath ?? FileName;


        /// <summary>
        /// Wirte 메서드는 디버그 코드를 파일에 덮어쓰는 방식으로 기록합니다. 디버그 코드만 가져옵니다.
        /// </summary>
        /// <param name="debugCode">디버그 코드는 소켓 통신의 플로우 차트를 기준으로 상태를 디버그한다.</param>
        public void Write(DebugCode debugCode);


    }

    public class TextFile : IFileIO
    {

        public string FileName { get; set; }
        public string UsePath => FilePath ?? FileName;
        public string? FilePath { get; set; }

        /// <summary>
        /// 경로 미지정 생성자 입니다.
        /// </summary>
        public TextFile()
        {
            this.FilePath = null;
            this.FileName = "Log.txt";
        }
        public TextFile(string name)
        {

            Console.WriteLine("객체 생성");
            this.FileName = name + ".txt";
            string basePath = Directory.GetCurrentDirectory();
            string relativePath = Path.Combine("Log");
            FilePath = Path.Combine(basePath, relativePath);
            Console.WriteLine($"상대 경로: {relativePath}");
            Console.WriteLine($"결합된 전체 경로: {FilePath}");
            if (!string.IsNullOrEmpty(FilePath) && !Directory.Exists(FilePath))
            {
                try
                {
                    Directory.CreateDirectory(FilePath);
                    Console.WriteLine("디렉터리 생성");

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"디렉터리 생성 오류 : {ex.Message}");
                    throw;
                }
            }
            FilePath = Path.Combine(FilePath, FileName);
            Console.WriteLine($"결합된 전체 경로: {FilePath}");
        }
        /// <summary>
        /// 기본 타입 입니다. 해당 메소드는 디버그 코드의 타입을 정하고 저장할 문자열을 결정하는 메소드입니다.
        /// </summary>
        /// <param name="debugCode">디버그 코드는 소켓 통신의 플로우 차트를 기준으로 상태를 디버그한다.</param>
        private string TextTypeSet(DebugCode debugCode)
        {
            string text = " ";

            switch (debugCode)
            {
                case DebugCode.Initialize:
                    text = "Initialize";
                    break;
                case DebugCode.Bind:
                    text = "Bind";
                    break;
                case DebugCode.Listen:
                    text = "Listen";
                    break;
                case DebugCode.Accept:
                    text = "Accept";
                    break;
                case DebugCode.Receive:
                    text = "Receive";
                    break;
                case DebugCode.Send:
                    text = "Send";
                    break;
                case DebugCode.Close:
                    text = "close";
                    break;
                case DebugCode.Connect:
                    text = "Connet";
                    break;
                default:
                    return "잘못된 입력입니다.";


            }
            return text;
        }//일반 디버그 타입
        /// <summary>
        /// 기본 TextTypeSet 메서드를 참조하세요.
        /// </summary>
        private string TextTypeSet(DebugCode debugCode, IPEndPoint? iP)
        {
            string text = $"[{DateTime.Now.ToString()}] [{iP?.Address.ToString()}] : ";

            switch (debugCode)
            {
                case DebugCode.Initialize:
                    return $"{text} : Initialize";
                case DebugCode.Bind:
                    return $"{text} : Bind";
                case DebugCode.Listen:
                    return $"{text} : Listen";
                case DebugCode.Accept:
                    return $"{text} : Accept";
                case DebugCode.Receive:
                    return $"{text} : Receive";
                case DebugCode.Send:
                    return $"{text} : Send";
                case DebugCode.Close:
                    return $"{text} : Close";
                case DebugCode.Connect:
                    return $"{text} : Connect";
                default:
                    return "잘못된 입력입니다.";

            }

        }//ip타입


        public void Initialize()
        {
            string path = UsePath;
            try
            {
                using (StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8))
                {
                    sw.WriteLine(DateTime.Now.ToString());
                    Console.WriteLine("파일 생성");
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                throw;
            }
        }

        public void Write(DebugCode debugCode, IPEndPoint ip)//ip타입
        {
            string text = TextTypeSet(debugCode, ip);
            string path = UsePath; // FilePath가 null인 경우 FileName 사용
            try
            {
                using (StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8))
                {
                    sw.WriteLine($"{DateTime.Now}: {text}");


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"파일 쓰기 오류 : {ex.Message}");
                throw;
            }
        }

        public void Write(DebugCode debugCode)//일반 디버그 타입
        {
            string text = TextTypeSet(debugCode);
            string path = UsePath; // FilePath가 null인 경우 FileName 사용

            try
            {

                using (StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8))
                {
                    sw.WriteLine($"{DateTime.Now}: {text}");


                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"파일 쓰기 오류 : {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 파일에 로그를 추가합니다. 덮어쓰지않습니다.
        /// </summary>
        /// <param name="debugCode">열거형 타입 기준으로 로그 타입이 결정됩니다.</param>
        /// <param name="ip">로그 작성 시 호스트의 ip입니다.</param>
        public void Append(DebugCode debugCode, IPEndPoint ip)//ip타입
        {
            string text = TextTypeSet(debugCode, ip);
            string path = UsePath; // FilePath가 null인 경우 FileName 사용
            try
            {
                using (StreamWriter sw = new StreamWriter(path, true, Encoding.UTF8))
                {
                    sw.WriteLine(text);


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"파일 쓰기 오류 : {ex.Message}");
                throw;
            }
        }
        public void Append(DebugCode debugCode)//일반 디버그 타입  
        {
            string text = TextTypeSet(debugCode);
            string path = UsePath; // FilePath가 null인 경우 FileName 사용
            try
            {
                using (StreamWriter sw = new StreamWriter(path, true, Encoding.UTF8))
                {
                    sw.WriteLine(text);


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"파일 쓰기 오류 : {ex.Message}");
                throw;
            }
        }

        public void Read()
        {
            string path = UsePath; // FilePath가 null인 경우 FileName 사용
            try
            {
                using (StreamReader sr = new StreamReader(path, Encoding.UTF8))
                {
                    string Content = sr.ReadToEnd();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"파일 읽기 오류 : {ex.Message}");
                throw;
            }
        }

    }

    public class JsonFile : IFileIO
    {
        public string FileName { get; set; }
        public string UsePath => FilePath ?? FileName;
        public string? FilePath { get; set; }
        private List<JsonDataStruct> jsonLogData;
        //json 구조 더 짜기
        public JsonFile(string fileName)
        {
            jsonLogData = new List<JsonDataStruct>();
            Console.WriteLine("객체 생성");
            this.FileName = fileName + ".json";
            string basePaht = Directory.GetCurrentDirectory();
            string relativePath = Path.Combine("Log");
            FilePath = Path.Combine(basePaht, relativePath);
            Console.WriteLine($"상대 경로: {relativePath}");
            Console.WriteLine($"결합된 전체 경로: {FilePath}");
            if (!string.IsNullOrEmpty(FilePath) && !Directory.Exists(FilePath))
            {
                try
                {
                    Directory.CreateDirectory(FilePath);
                    Console.WriteLine("디렉터리 생성");

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"디렉터리 생성 오류 : {ex.Message}");
                    throw;
                }
            }
            FilePath = Path.Combine(FilePath, FileName);
            Console.WriteLine($"결합된 전체 경로: {FilePath}");
            
        }
        
        /// <summary>
        /// 기본 타입 입니다. 해당 메소드는 디버그 코드의 타입을 정하고 저장할 문자열을 결정하는 메소드입니다.
        /// </summary>
        /// <param name="debugCode">디버그 코드는 소켓 통신의 플로우 차트를 기준으로 상태를 디버그한다.</param>
        private string TextTypeSet(DebugCode debugCode)
        {
            string text = " ";

            switch (debugCode)
            {
                case DebugCode.Initialize:
                    text = "Initializie";
                    break;
                case DebugCode.Bind:
                    text = "Bind";
                    break;
                case DebugCode.Listen:
                    text = "Listen";
                    break;
                case DebugCode.Accept:
                    text = "Accept";
                    break;
                case DebugCode.Receive:
                    text = "Receive";
                    break;
                case DebugCode.Send:
                    text = "Send";
                    break;
                case DebugCode.Close:
                    text = "close";
                    break;
                case DebugCode.Connect:
                    text = "Connet";
                    break;
                default:
                    return "잘못된 입력입니다.";


            }
            return text;
        }//일반 디버그 타입

        /// <summary>
        /// 기본 TextTypeSet 메서드를 참조하세요.
        /// </summary>
        private string TextTypeSet(DebugCode debugCode, IPEndPoint? iP)
        {
            string text = $"[{DateTime.Now.ToString()}] [{iP?.Address.ToString()}] : ";
            iP?.Address.ToString();
            switch (debugCode)
            {
                case DebugCode.Initialize:
                    return $"{text} : Initialize";
                case DebugCode.Bind:
                    return $"{text} : Bind";
                case DebugCode.Listen:
                    return $"{text} : Listen";
                case DebugCode.Accept:
                    return $"{text} : Accept";
                case DebugCode.Receive:
                    return $"{text} : Receive";
                case DebugCode.Send:
                    return $"{text} : Send";
                case DebugCode.Close:
                    return $"{text} : Close";
                case DebugCode.Connect:
                    return $"{text} : Connect";
                default:
                    return "잘못된 입력입니다.";

            }

        }//ip타입

        public void Initialize()    
        {
            string path = UsePath;
            try
            {
                using (StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8))
                {

                }

            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                throw;
            }
        }
        public void Write(DebugCode debugcode)
        {
            string path = UsePath;
            try
            {
                jsonLogData.Add(new JsonDataStruct
                {
                    TimeStemp = DateTime.Now,
                    DebugCode = TextTypeSet(debugcode)
                });

                using (StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8))
                {
                    string jsonString = JsonConvert.SerializeObject(jsonLogData, Formatting.Indented);
                    sw.WriteLine(jsonString);
                    Console.WriteLine("JSON 파일 생성 완료");
                }

            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                throw;
            }
        }
        public void Read()
        {
            string path = UsePath; // FilePath가 null인 경우 FileName 사용
            try
            {
                if (!File.Exists(FilePath))
                {
                    Console.WriteLine($"읽을 파일이 존재하지 않습니다.{FilePath}");

                }
                using (StreamReader sr = new StreamReader(path, Encoding.UTF8))
                {
                    string content = File.ReadAllText(path, Encoding.UTF8);
                    var logs = JsonConvert.DeserializeObject<List<JsonDataStruct>>(content);
                    foreach (var log in logs)
                    {
                        Console.WriteLine($"시간: {log.TimeStemp}, 디버그 코드: {log.DebugCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"파일 쓰기 오류 : {ex.Message}");
                throw;
            }
        }
        class JsonDataStruct
        {
            public DateTime? TimeStemp { get; set; }
            public string? DebugCode { get; set; }

        }
        public class DebugingService
        {
            public IFileIO fileIO;

            public DebugingService(IFileIO fileIO)
            {
                this.fileIO = fileIO;
            }

        }

            internal class Program
        {
            static void Main(string[] args)
            {
                JsonFile jsonFile = new JsonFile("DebugLog");
                DebugingService debugingService = new DebugingService(jsonFile);
                debugingService.fileIO.Initialize();
                debugingService.fileIO.Write(DebugCode.Initialize);
                debugingService.fileIO.Write(DebugCode.Bind);
                debugingService.fileIO.Write(DebugCode.Listen);

            }
        }
    }

}
