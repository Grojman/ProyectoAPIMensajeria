public class ConectionHandler {
    Socket server;
    private bool running;
    public ConectionHandler(IPAddress ip, int port, int maxCon) {
        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        server.Bind(new IPEndPoint(ip, port));
        server.Listen(maxCon);
        running = true;
        Console.WriteLine($"Opened server at [{ip}:{port}]");
        Thread mainThread = new Thread(Handle);
        mainThread.Start();
    }
    private void Handle() {
        while(running) {
            Socket client = server.Accept();
            Thread thread = new Thread(() => ThreadHandler(client));
            thread.Start();
        }
    }
    private void ThreadHandler(Socket client) {
        Console.WriteLine("Usuario conectado");
        bool threadIsRunning = true;


        //RECIVIR ENCABEZADO Y DEVOLVER COSA
        byte[] headerBytes = new byte[1024];
        client.Receive(headerBytes);
        string data = Encoding.UTF8.GetString(headerBytes);
        const string eol = "\r\n"; // HTTP/1.1 defines the sequence CR LF as the end-of-line marker
        byte[] response = Encoding.UTF8.GetBytes("HTTP/1.1 101 Switching Protocols" + eol
            + "Connection: Upgrade" + eol
            + "Upgrade: websocket" + eol
            + "Sec-WebSocket-Accept: " + Convert.ToBase64String(
                System.Security.Cryptography.SHA1.Create().ComputeHash(
                    Encoding.UTF8.GetBytes(
                        new System.Text.RegularExpressions.Regex("Sec-WebSocket-Key: (.*)").Match(data).Groups[1].Value.Trim() + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"
                    )
                )
            ) + eol
            + eol);
        client.Send(response);
        while(threadIsRunning) {
            byte[] bytes = new byte[1024];
            client.Receive(bytes);    
            string repsonse = Encoding.UTF8.GetString(bytes);
            Console.WriteLine(data);
        }
    }
}