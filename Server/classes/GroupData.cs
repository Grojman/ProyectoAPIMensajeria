public class GroupData {
    public int Id {get;}
    public string Name {get;}
    public List<UserData> Users {get;}
    public GroupData(int id, in string name, List<UserData> users) {
        Id = id;
        Name = name;
        Users = users;
    }
    public string ToJson() => $"\"GroupId\" : \"{Id}\", \"GroupName\" : \"{Name}\", \"Users\" : [ {string.Join(", ", Users)} ] ";
}