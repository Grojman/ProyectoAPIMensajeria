public abstract class UserHandler {
    public IDataBase dataBaseConection;
    protected List<User> conectedUsers;
    public char Separator {get;}
    protected UserHandler(in char separator, in IDataBase dataBaseConection) {
        conectedUsers = new List<User>();
        this.dataBaseConection = dataBaseConection;
        Separator = separator;
    }
    public abstract void HandleMessage(in string[] message);
    public abstract bool UserIsAlreadyConected(in string Id);
    public abstract bool UserIsAlreadyConected(in TcpClient socket);
    public abstract void SendMsg(string Id, string message, MessageStatus m);
    public abstract void SendMsg(Stream stream, string message, MessageStatus m);
    public abstract void SendMsg(TcpClient socket, string message, MessageStatus m);
    public abstract void AddUser(in string Id, in TcpClient socket);
    public enum MessageStatus {LogIn, FailedLogIn, LogOut, NewChat, NewMessage}
}