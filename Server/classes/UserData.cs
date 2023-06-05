public class UserData {
    public int Id {get;}
    public string Nickname {get;}
    public UserData(int id, in string nickname) {
        Id = id;
        Nickname = nickname;
    }
    public string ToJson() => $" [ \"UserId\" : \"{Id}\", \"Nickname\" : \"{Nickname}\" ] ";
}