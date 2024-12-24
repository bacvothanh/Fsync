// See https://aka.ms/new-console-template for more information
using Fsync.Client;

Console.WriteLine("Hello, World!");

var client = new FileSyncClient("127.0.0.1", 5000, @"C:\ClientFolder");
client.Start();

Console.ReadLine();