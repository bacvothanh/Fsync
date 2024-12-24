using System.Net.Sockets;
using System.Text;

namespace Fsync.Client
{
    public class FileSyncClient
    {
        private readonly string serverAddress;
        private readonly int serverPort;
        private readonly string localFolderPath;

        public FileSyncClient(string serverAddress, int serverPort, string localFolderPath)
        {
            this.serverAddress = serverAddress;
            this.serverPort = serverPort;
            this.localFolderPath = localFolderPath;
        }

        public void Start()
        {
            Console.WriteLine("Connecting to server...");
            using var client = new TcpClient(serverAddress, serverPort);
            using var stream = client.GetStream();
            using var reader = new StreamReader(stream, Encoding.UTF8);
            using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

            Console.WriteLine(reader.ReadLine()); // Welcome message

            while (true)
            {
                string command = reader.ReadLine();
                if (command == "SYNC_FILE")
                {
                    ReceiveFile(reader, stream);
                }
            }
        }

        private void ReceiveFile(StreamReader reader, NetworkStream stream)
        {
            // Nhận tên file
            string fileName = reader.ReadLine();
            string filePath = Path.Combine(localFolderPath, fileName);

            // Nhận nội dung file và lưu
            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            stream.CopyTo(fileStream);

            Console.WriteLine($"File synced: {fileName}");
        }
    }
}
