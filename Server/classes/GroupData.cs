public class GroupData {
    public int Id {get;}
    public string Name {get;}
    public List<UserData> Users {get; set;}
    public GroupData(int id, in string name) {
        Id = id;
        Name = name;
    }
    public string ToJson() => $"\"GroupId\" : \"{Id}\", \"GroupName\" : \"{Name}\", \"Users\" : [ {string.Join(", ", Users.Select(n => n.ToJson()))} ] ";
}