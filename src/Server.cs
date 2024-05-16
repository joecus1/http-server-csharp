using System.Net;
using System.Net.Sockets;
using System.Text;

TcpListener server = new TcpListener(IPAddress.Any, 4221);
server.Start();

Socket socket = server.AcceptSocket(); // wait for client
Console.Write("Connected!");

var responseString = "HTTP/1.1 200 OK\r\n\r\n";
Byte[] responseBytes = Encoding.ASCII.GetBytes(responseString);
int i = socket.Send(responseBytes);
Console.WriteLine($"Responded: {responseString}");
