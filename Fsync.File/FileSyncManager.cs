namespace Fsync.File
{
    public class FileSyncManager
    {
        private readonly string directoryPath;
        private FileSystemWatcher watcher;

        public FileSyncManager(string path)
        {
            directoryPath = path;
            watcher = new FileSystemWatcher(directoryPath)
            {
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size
            };
            watcher.Changed += OnChanged;
            watcher.Created += OnChanged;
            watcher.Deleted += OnChanged;
            watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            // Xử lý khi file thay đổi
            Console.WriteLine($"File {e.ChangeType}: {e.FullPath}");
            // Bạn có thể thêm logic để gửi file qua mạng tại đây.
        }
    }
}
