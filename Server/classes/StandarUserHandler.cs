/// <summary>
/// Example of UserHandler's Implementation
/// </summary>
public class StandarUserHandler : UserHandler {
    public StandarUserHandler(in char separator, in IDataBase dataBase) : base(separator, dataBase) {}

    public override void HandleMessage(in string[] message)
    {
        /*
           header/senderId/sendernickname/message

            Posible headers:
            
            1. User needs messages to fill website
            2. User sends message to someone
            3. User wants to send message to group
            4. User want's to create a group
            5. User log's out

            Every single message starts with the header and the user id


            if a user wants to chat with someone new, It will be client who will check wheter the conversation has already started or not
            Otherwise, it behaves the same as a normal message
            
            Messages will have the nickname of the person, so client can identify them easily
        */
        switch(message[0]) {
            case "get-msgs":
                SendData(message[1]);
                break;
            case "snd-msg":
                break;
            case "snd-gmsg":
                break;
            case "crt-grp":
                break;
            case "log-out":
                break;
            default:
                break;
        }
    }
    private void SendData(string Id) {
        //OBTENER TODAS LAS CONVERSACIONES
        string[] conversations = dataBaseConection.GetConversations(Id);
        foreach(string c in conversations) {
            string[] messages = dataBaseConection.GetMessages(c, 10);//¿?¿?¿
            foreach(string m in messages)
                SendMsg(Id, m);
        }
            //DE CADA UNA DE LAS CONVERSACIONES, OBTENER LOS ÚLTIMOS N MENSAJES
            //DEVOLVER TODAS LAS CONVERSACIONES AL USUARIO
    }
    private void SendGroupMsg(string conversationId, string userId, string message) {
        //HAY QUE ASEGURARSE DE QUÉ RECIBE EN EL PARÁMETRO MESSAGE


        //PEDIRLE A LA BASE DE DATOS TODOS LOS USUARIOS QUE PARTICIPEN EN LA CONVERSACION
        string[] users = dataBaseConection.FindUsersFromConversation(conversationId, userId);
            //A CADA UNO DE LOS USUARIOS (QUE ESTÉN CONECTADOS), ENVIARLE EL MENSAJE
        foreach(string id in users) {
            if (UserIsAlreadyConected(id))
                SendMsg(id, message);
        }
        //REGISTRAR MENSAJE EN LA CONVERSACION
        dataBaseConection.SaveMessage(userId, conversationId, message.Split(Separator)[3]);
    }
    private void CreateGroup(string[] usersId) {
        //CREAR EL GRUPO EN LA BASE DE DATOS
        dataBaseConection.SaveGroup(usersId);
        //ESPERAR A LA CONFIRMACIÓN
        //ENVIAR LA NUEVA CONVERSACIÓN A TODOS LOS USUARIOS QUE ESTÉN CONECTADOS
    }
    private void LogOut(string Id) {
        //STILL NEEDS TO BE CHECKED
        conectedUsers.Remove(conectedUsers.Where(n => n.Id.Equals(Id)).First());
    }

    public override bool UserIsAlreadyConected(in string Id) => true; //conectedUsers.Select(n => n.Id).Contains(Id);

    public override bool UserIsAlreadyConected(in TcpClient socket) => conectedUsers.Select(n => n.Client).Contains(socket);

    public override void SendMsg(TcpClient socket, string message) => SendMsg(socket.GetStream(), message);
    public override void SendMsg(string Id, string message) => SendMsg(conectedUsers.Where(n => n.Id.Equals(Id)).First().Stream, message);
    public override void AddUser(in string Id, in TcpClient socket) => conectedUsers.Add(new User(Id, socket));
    
    
    //STOLEN CODE
    public override void SendMsg(Stream stream, string msg)
    {
        // NetworkStream stream = client.GetStream();
        Queue<string> que = new Queue<string>(msg.SplitInGroups(125));
        int len = que.Count;

        while (que.Count > 0)
        {
            var header = GetHeader(
                que.Count > 1 ? false : true,
                que.Count == len ? false : true
            );

            byte[] list = Encoding.UTF8.GetBytes(que.Dequeue());
            header = (header << 7) + list.Length;
            stream.Write(IntToByteArray((ushort)header), 0, 2);
            stream.Write(list, 0, list.Length);
        }            
    }


    private int GetHeader(bool finalFrame, bool contFrame)
    {
        int header = finalFrame ? 1 : 0;//fin: 0 = more frames, 1 = final frame
        header = (header << 1) + 0;//rsv1
        header = (header << 1) + 0;//rsv2
        header = (header << 1) + 0;//rsv3
        header = (header << 4) + (contFrame ? 0 : 1);//opcode : 0 = continuation frame, 1 = text
        header = (header << 1) + 0;//mask: server -> client = no mask

        return header;
    }

    private byte[] IntToByteArray(ushort value)
    {
        var ary = BitConverter.GetBytes(value);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(ary);
        }

        return ary;
    }
}