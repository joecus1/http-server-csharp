using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

TcpListener server = new TcpListener(IPAddress.Any, 4221);
server.Start();

Socket socket = server.AcceptSocket(); // wait for client
Console.Write("Connected!\n");

HashSet<string> endPoints = ["/"];

byte[] receivedBytes = new byte[256];

int byteCount = socket.Receive(receivedBytes);

if (byteCount == 0)
{
    Console.WriteLine("No bytes received!\n");
    throw new Exception("Bad request");
}

var httpRequest = new HttpRequest(receivedBytes, byteCount);

string sendString;
if (endPoints.Contains(httpRequest.RequestLine.Target))
{
    sendString = "HTTP/1.1 200 OK\r\n\r\n";
}
else
{
    sendString = "HTTP/1.1 404 Not Found\r\n\r\n";
}

byte[] sendBytes = Encoding.ASCII.GetBytes(sendString);
int i = socket.Send(sendBytes);
Console.WriteLine($"Responded: {sendString}\n");


class RequestLine
{
    public string Method { get; set; }
    public string Target { get; set; }
    public string Version { get; set; }
    public RequestLine(string line)
    {
        var tokens = line.Split(" ");
        Method = tokens[0];
        Target = tokens[1];
        Version = tokens[2];
    }
}

class HttpRequest
{
    public RequestLine RequestLine { get; set; }
    public List<string> Headers = new List<string>();
    public HttpRequest(byte[] receivedBytes, int byteCount)
    {
        char[] receivedChars = new char[256];
        int charCount = Encoding.ASCII.GetChars(receivedBytes, 0, byteCount, receivedChars, 0);

        var receivedLines = new string(receivedChars).Split("\r\n");
        RequestLine = new RequestLine(receivedLines[0]);
    }
}