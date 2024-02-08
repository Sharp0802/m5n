// See https://aka.ms/new-console-template for more information

using System.Text;
using M5N.Slave;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding  = Encoding.UTF8;

Console.WriteLine("Hello, World!");

Python.Call("test", "main", "test");
