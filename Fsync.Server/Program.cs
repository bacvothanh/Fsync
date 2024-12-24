// See https://aka.ms/new-console-template for more information
using Fsync.Server;

Console.WriteLine("Hello, World!");

var server = new FileSyncServer(@"C:\SyncFolder", 5000);
server.Start();


Console.ReadLine();
