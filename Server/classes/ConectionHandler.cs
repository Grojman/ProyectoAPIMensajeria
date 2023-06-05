class ConectionHandler {
    readonly string ip;
    readonly int port;
    TcpListener server;
    bool running;
    private UserHandler userHandler;
    public ConectionHandler(in string ip, in int port, UserHandler userHandler) {
        this.userHandler = userHandler;
        this.ip = ip;
        this.port = port;

        server = new TcpListener(IPAddress.Parse(ip), port);
        server.Start();
        Console.WriteLine("Server has started on {0}:{1}, Waiting for a connection…", ip, port);

        Start();

        Thread mainThread = new Thread(HandleConection);
        mainThread.Start();
    }

    public void Start() => running = true;
    public void Stop() => running = false;
    
    private void HandleConection() {
        while (true) { //EVITA QUE DESAPAREZCA EL HILO SI EL HANDLER SE DETIENE
            while(running) {
            TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("Un equipo se ha conectado");
            NetworkStream stream = client.GetStream();
            Thread clientThread = new Thread(() => HandleSocketThread(client, stream));
            clientThread.Start();
        }
        }
    }

    private void HandleSocketThread(TcpClient client, NetworkStream stream) {
        while (true) {
            while (!stream.DataAvailable);
            while (client.Available < 3); // match against "get"
            string? message = HandleMessages(client, stream);
            if (message is not null) {
                string[] deglosedMessage = message.Split(userHandler.Separator);
                if(userHandler.UserIsAlreadyConected(client)) userHandler.HandleMessage(deglosedMessage);
                else HandleMessage(client, deglosedMessage);
            }
        }
    }
    private void HandleMessage(TcpClient socket, string[] message) {
        //HAY QUE PODER MANDAR MENSAJES DE VUELTA EN ALGÚN PUTO LADO JAFKLDJSLAÑFJDLASKÑFJKDLÑASJFLKDÑASJFÑA
        /*
            Possible messages:
            1. Wants to log in
            2. Wants to register


            Even tho there is only 2 options, I rather use a switch block.
        */
        switch (message[0]) {
            case "log-in":
                LogIn(userHandler.dataBaseConection.FromNicknameToId(message[1]), message[2], socket);
                break;
            case "sign-in":
                SignIn(message[1], message[2], socket);
                break;
            default:
                Console.WriteLine($"Mensaje con cabecera desconocida: {string.Join("", message)}");
                break;
        }
    }
    private void LogIn(string Id, string password, TcpClient socket) {
        //COMPROBAR SI EL USUARIO NO ESTÁ CONECTADO
        //COMPROBAR SI LAS CREDENCIALES DEL USUARIO SON CORRECTAS
        if (!(userHandler.UserIsAlreadyConected(Id)) && userHandler.dataBaseConection.CheckCredentials(Id, password.Encrypt())) {
        //SI TODO LO ANTERIOR ES CORRECTO:
            //GUARDAR NUEVO USUARIO EN LA LISTA DE USUARIOS CONECTADOS, Y AVISAR AL USUARIO DE QUE SE HA CONECTADO
            userHandler.AddUser(Id, socket);
            //ENVIARLE JUNTO CON EL AVISO EL ID DE SU USUARIO, PARA QUE NO TENER QUE UTILIZAR EL NICKNAME
            userHandler.SendMsg(socket.GetStream(), $"\"Id\" : \"{Id}\"", UserHandler.MessageStatus.LogIn);
        } else {
        //EN CASO CONTRARIO:
            //AVISAR DEL ERROR
            userHandler.SendMsg(socket.GetStream(), "\"ErrorMessage\" : \"El inicio de sesión ha fallado. Por favor, inténtalo de nuevo\"", UserHandler.MessageStatus.FailedLogIn); 
        }  
    }

    private void SignIn(string Nickname, string Password, TcpClient socket) {
        //COMPROBAR QUE EL NOMBRE NO EXISTE TODAVÍA
        if (!userHandler.dataBaseConection.UserExists(Nickname)) {
        //SI ES CORRECTO:
            //CREAR USUARIO EN LA BASE DE DATOS
            userHandler.dataBaseConection.SaveUser(Nickname, Password.Encrypt());
            //AÑADIRLO COMO USUARIO CONECTADO
            userHandler.AddUser(userHandler.dataBaseConection.FromNicknameToId(Nickname), socket);
            //AVISAR AL CLIENTE DE QUE SE HA REGISTRADO CORRECTAMENTE
            userHandler.SendMsg(socket, $"\"Id\" : \"{userHandler.dataBaseConection.FromNicknameToId(Nickname)}\"", UserHandler.MessageStatus.LogIn);
        } else {
        //EN CASO CONTRARIO:
            //AVISAR DEL ERROR AL CLIENTE
            userHandler.SendMsg(socket.GetStream(), $"\"ErrorMessage\" : \"{Nickname} ya está escogido. Por favor, prueba con otro nombre.\"", UserHandler.MessageStatus.FailedLogIn);
        }
    }
    

    //NO TENGO NI IDEA DE CÓMO FUNCIONA, ESTÁ COPIADO DE UNA PÁGINA WEB. DEVUELVE NULO SI SE HA PRODUCIDO EL HANDSHAKING O LA MÁSCARA NO ESTÁ PUESTA
    private string? HandleMessages(TcpClient client, Stream stream) {
        byte[] bytes = new byte[client.Available];
            stream.Read(bytes, 0, client.Available);
            string s = Encoding.UTF8.GetString(bytes);

            if (Regex.IsMatch(s, "^GET", RegexOptions.IgnoreCase)) {
                // Console.WriteLine("=====Handshaking from client=====\n{0}", s);

                // 1. Obtain the value of the "Sec-WebSocket-Key" request header without any leading or trailing whitespace
                // 2. Concatenate it with "258EAFA5-E914-47DA-95CA-C5AB0DC85B11" (a special GUID specified by RFC 6455)
                // 3. Compute SHA-1 and Base64 hash of the new value
                // 4. Write the hash back as the value of "Sec-WebSocket-Accept" response header in an HTTP response
                string swk = Regex.Match(s, "Sec-WebSocket-Key: (.*)").Groups[1].Value.Trim();
                string swka = swk + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
                byte[] swkaSha1 = System.Security.Cryptography.SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(swka));
                string swkaSha1Base64 = Convert.ToBase64String(swkaSha1);

                // HTTP/1.1 defines the sequence CR LF as the end-of-line marker
                byte[] response = Encoding.UTF8.GetBytes(
                    "HTTP/1.1 101 Switching Protocols\r\n" +
                    "Connection: Upgrade\r\n" +
                    "Upgrade: websocket\r\n" +
                    "Sec-WebSocket-Accept: " + swkaSha1Base64 + "\r\n\r\n");

                stream.Write(response, 0, response.Length);
                return null;
            } else {
                bool fin = (bytes[0] & 0b10000000) != 0,
                    mask = (bytes[1] & 0b10000000) != 0; // must be true, "All messages from the client to the server have this bit set"
                int opcode = bytes[0] & 0b00001111, // expecting 1 - text message
                    offset = 2;
                ulong msglen = (ulong)bytes[1] & 0b01111111;

                if (msglen == 126) {
                    // bytes are reversed because websocket will print them in Big-Endian, whereas
                    // BitConverter will want them arranged in little-endian on windows
                    msglen = BitConverter.ToUInt16(new byte[] { bytes[3], bytes[2] }, 0);
                    offset = 4;
                } else if (msglen == 127) {
                    // To test the below code, we need to manually buffer larger messages — since the NIC's autobuffering
                    // may be too latency-friendly for this code to run (that is, we may have only some of the bytes in this
                    // websocket frame available through client.Available).
                    msglen = BitConverter.ToUInt64(new byte[] { bytes[9], bytes[8], bytes[7], bytes[6], bytes[5], bytes[4], bytes[3], bytes[2] },0);
                    offset = 10;
                }

                if (msglen == 0) {
                    // Console.WriteLine("msglen == 0");
                } else if (mask) {
                    byte[] decoded = new byte[msglen];
                    byte[] masks = new byte[4] { bytes[offset], bytes[offset + 1], bytes[offset + 2], bytes[offset + 3] };
                    offset += 4;

                    for (ulong i = 0; i < msglen; ++i)
                        decoded[i] = (byte)(bytes[offset + (int)i] ^ masks[i % 4]);

                    return Encoding.UTF8.GetString(decoded);
                } else 
                    Console.WriteLine("mask bit not set");
                    return null;
            }
    }
}