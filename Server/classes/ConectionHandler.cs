public class ConectionHandler {
    TcpListener server;
    private bool running;
    public ConectionHandler(IPAddress ip, int port) {
        server = new TcpListener(ip, port);
        server.Start();
        running = true;
        Console.WriteLine($"Opened server at [{ip}:{port}]");
        Thread mainThread = new Thread(Handle);
        mainThread.Start();
    }
    public void Start() => server.Start();
    public void Stop() => server.Stop();
    private void Handle() {
        while(running) {
            TcpClient client = server.AcceptTcpClient();
            Thread thread = new Thread(() => ThreadHandler(client, client.GetStream()));
            thread.Start();
        }
    }
    private void ThreadHandler(TcpClient client, NetworkStream stream) {
        //TEST
        byte[] test = Encoding.UTF8.GetBytes("Hola");
        stream.Write(test, 0, test.Length);
        Console.WriteLine("Usuario conectado");
        bool threadIsRunning = true;
        while(threadIsRunning) {
            while(!stream.DataAvailable) {
                byte[] bytes = new byte[client.Available];
                stream.Read(bytes, 0, client.Available);
                string data = Encoding.UTF8.GetString(bytes);
                Console.WriteLine(data);
            }
        }
    }
}