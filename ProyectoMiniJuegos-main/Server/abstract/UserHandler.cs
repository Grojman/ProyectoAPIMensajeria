public abstract class UserHandler {
    protected IDataBase dataBaseConection;
    protected LinkedList<User> conectedUsers;
    public char Separator {get;}
    protected UserHandler(in char separator, in IDataBase dataBaseConection) {
        conectedUsers = new LinkedList<User>();
        this.dataBaseConection = dataBaseConection;
        Separator = separator;
    }
    public abstract void HandleMessage(in string[] message);
    public abstract bool UserIsAlreadyConected(in string Id);
    public abstract bool UserIsAlreadyConected(in TcpClient socket);
    public abstract void SendMsg(string Id, string message);
    public abstract void SendMsg(Stream stream, string message);
    public abstract void SendMsg(TcpClient socket, string message);
}