global using System.Net.Sockets;
global using System.Net;
global using System.Text;
class Program {
    public static void Main(string[] args) {
        var ch = new ConectionHandler(IPAddress.Parse("127.0.0.1"), 14000);
        ch.Start();
    }
}