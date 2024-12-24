using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Fsync.Server
{
    public class FileSyncServer
    {
        private readonly string folderPath;
        private readonly TcpListener server;
        private readonly List<TcpClient> clients = new List<TcpClient>();
        private FileSystemWatcher watcher;

        public FileSyncServer(string folderPath, int port)
        {
            this.folderPath = folderPath;
            server = new TcpListener(IPAddress.Any, port);
        }

        public void Start()
        {
            Console.WriteLine("Server started...");
            server.Start();

            watcher = new FileSystemWatcher(folderPath)
            {
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size,
                IncludeSubdirectories = false,
                EnableRaisingEvents = true
            };

            watcher.Created += OnFileChanged;
            watcher.Changed += OnFileChanged;

            Thread clientThread = new Thread(HandleClients);
            clientThread.Start();

            Console.WriteLine($"Monitoring folder: {folderPath}");
            Console.WriteLine("Waiting for clients...");
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"Detected change: {e.FullPath}");
            BroadcastFile(e.FullPath);
        }

        private void HandleClients()
        {
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                lock (clients)
                {
                    clients.Add(client);
                }
                Console.WriteLine("Client connected!");
            }
        }

        private void BroadcastFile(string filePath)
        {
            lock (clients)
            {
                foreach (var client in clients)
                {
                    try
                    {
                        if (client.Connected)
                        {
                            using var stream = client.GetStream();
                            using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                            // Gửi thông tin file
                            writer.WriteLine("SYNC_FILE");
                            writer.WriteLine(Path.GetFileName(filePath));

                            // Gửi nội dung file
                            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                            fileStream.CopyTo(stream);

                            Console.WriteLine($"File sent to client: {Path.GetFileName(filePath)}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error sending file to client: {ex.Message}");
                    }
                }
            }
        }
    }
}
