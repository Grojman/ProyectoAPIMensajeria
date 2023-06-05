/// <summary>
/// Interface to interact with the database from wich we are taking user's data & messages
/// </summary>
public interface IDataBase {
    //ALL OF THE string[][] should be replaced with classes
    public List<UserData> FindUsersFromConversation(string conversationId, string userId);
    public bool UserExists(string Nickname);
    public List<MessageData> GetMessages(string conversationId, int amount, char separator);
    public List<GroupData> GetConversations(string userId);
    public void SaveMessage(string sender, string destination, string message);
    public void SaveGroup(string[] users);
    public void SaveUser(string Nickname, string Password);
    public bool CheckCredentials(string Id, string password);
    public string FromNicknameToId(string Nickname);
    public class DataBaseConectionException : Exception {
        public DataBaseConectionException(string message) : base(message){}
    } 
}