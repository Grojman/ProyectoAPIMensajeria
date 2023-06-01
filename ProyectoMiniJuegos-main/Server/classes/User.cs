/// <summary>
/// Data objet to relate User's Id with a socket conection
/// </summary>
public class User {
    public readonly string Id;
    public TcpClient Client {get;}
    public NetworkStream Stream {get {return Client.GetStream();}}
    public User(in string id, in TcpClient client) {
        Id = id;
        Client = client;
    }
    public override bool Equals(object? obj) {
        User? u = obj as User;
        return u!.Id.Equals(Id) && u!.Client.Equals(Client);
    }
}