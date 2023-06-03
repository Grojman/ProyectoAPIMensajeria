/// <summary>
/// Interface to interact with the database from wich we are taking user's data & messages
/// </summary>
public interface IDataBase {
    public string[] FindUsersFromConversation(string conversationId, string userId);
    public bool UserExists(string Nickname);
    public string[] GetMessages(string conversationId, int amount, char separator);
    public string[] GetConversations(string userId);
    public void SaveMessage(string sender, string destination, string message);
    public void SaveGroup(string[] users);
    public void SaveUser(string Nickname, string Password);
    public bool CheckCredentials(string Id, string password);
    public string FromNicknameToId(string Nickname);
    public class DataBaseConectionException : Exception {
        public DataBaseConectionException(string message) : base(message){}
    } 
}