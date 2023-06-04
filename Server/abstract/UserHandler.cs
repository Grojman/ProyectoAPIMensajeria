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
    public abstract void SendMsg(string Id, string message);
    public abstract void SendMsg(Stream stream, string message);
    public abstract void SendMsg(TcpClient socket, string message);
    public abstract void AddUser(in string Id, in TcpClient socket);
    public abstract string BuildJson(MessageStatus Status, int Sender, string Nickname, int Destination, string Message);
    public enum MessageStatus {LogIn, FailedLogIn, LogOut, NewMessage}
}