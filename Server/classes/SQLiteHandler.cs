/// <summary>
/// An object that allows to comunicate to a DataBase throught SQlite. Note that this is a implementation example, using a DataBase made by myself. You might want to refactor this class from scrath, if you want to use it with your own DB.
/// TODO: REFACTOR THIS WHOLE FUCKING CLASS.
/// </summary>
public class SQLiteHandler : IDataBase
{
    private SQLiteConnection sql;
    private SQLiteCommand query;
    private string ConversationTable {get;}
    private string UsersTable {get;}
    private string MessagesTable {get;}    

    public SQLiteHandler(in string dbPath,in string conversationTable, in string usersTable, in string messageTable) {
        ConversationTable = conversationTable;
        UsersTable = usersTable;
        MessagesTable = messageTable;
        try {
            sql = new SQLiteConnection($"Data Source={dbPath}");
        } catch {
            throw new IDataBase.DataBaseConectionException($"Se ha producido un error al intentar conectarse a la base de datos. Ruta: {dbPath}");
        }
    }
    public string[] FindUsersFromConversation(string conversationId, string userId)
    {
        var users = new List<string>();
        sql.Open();
        query = new SQLiteCommand($"SELECT UserId FROM GROUPS WHERE GroupId = @{conversationId} AND UserId != @{userId}", sql);
        query.Parameters.Add($"@{conversationId}", System.Data.DbType.String);
        query.Parameters[$"@{conversationId}"].Value = conversationId;
        query.Parameters.Add($"@{userId}", System.Data.DbType.String);
        query.Parameters[$"@{userId}"].Value = userId;
        var reader = query.ExecuteReader();
        while(reader.Read()) {
            users.Append(reader.GetInt64(0).ToString());
        }
        sql.Close();
        return users.ToArray();
    }

    public string[] GetConversations(string userId)
    {
        //LAS CONVERASCIONES ENTRE DOS PERSONAS SE GUARDAN IGUAL QUE LAS CONVERSACIONES DE GRUPO
        var conversations = new List<string>();
        sql.Open();
        query = new SQLiteCommand($"SELECT GroupId FROM GROUPS WHERE UserId = @userId", sql);
        query.Parameters.Add($"@userId", System.Data.DbType.Int64);
        query.Parameters[$"@userId"].Value = userId;
        var reader = query.ExecuteReader();
        while(reader.Read()) {
            conversations.Add($"{reader.GetInt64(0)}");
        }
        sql.Close();
        return conversations.ToArray();
    }

    public string[] GetMessages(string conversationId, int amount, char separator)
    {
        var messages = new List<string>();
        sql.Open();
        query = new SQLiteCommand($"SELECT * FROM MESSAGES WHERE DestinationId = @{conversationId} ORDER BY Date LIMIT @{amount}", sql);
        query.Parameters.Add($"@{conversationId}", System.Data.DbType.String);
        query.Parameters[$"@{conversationId}"].Value = conversationId;
        query.Parameters.Add($"@{amount}", System.Data.DbType.Int32);
        query.Parameters[$"@{amount}"].Value = amount;
        var reader = query.ExecuteReader();
        while(reader.Read()) {
            messages.Add($"{reader.GetInt64(0)}{separator}{reader.GetInt64(1)}{separator}{reader.GetString(2)}{separator}{reader.GetString(3)}");
        }
        sql.Close();
        return messages.ToArray();
    }

    public void SaveGroup(string[] users) {
        sql.Open();
        query = new SQLiteCommand("SELECT GroupId FROM GROUPS ORDER BY GroupId DESC LIMIT 1", sql);
        Int64 groupId = (Int64)query.ExecuteScalar();
        groupId++;

        for(int i = 0; i < users.Length; i++) {
            query.CommandText = "INSERT INTO GROUPS('GroupId', 'UserId') VALUES(@id, @users)";
            query.Parameters.AddWithValue("@users", users[i]);
            query.Parameters.AddWithValue("@id", groupId);
            query.ExecuteNonQuery();
        }
        sql.Close();

    }

    public void SaveMessage(string sender, string destination, string message)
    {
        sql.Open();
        query = new SQLiteCommand($"INSERT INTO MESSAGES('SenderId', 'DestinationId', 'Message', 'Date') VALUES(@sender, @destination, @message, DATETIME('now'))", sql);
        query.Parameters.Add($"@sender", System.Data.DbType.Int64);
        query.Parameters[$"@sender"].Value = sender;
        query.Parameters.Add($"@destination", System.Data.DbType.Int64);
        query.Parameters[$"@destination"].Value = destination;
        query.Parameters.Add($"@message", System.Data.DbType.String);
        query.Parameters[$"@message"].Value = message;
        query.ExecuteNonQuery();
        sql.Close();
    }
    public void SaveUser(string Nickname, string Password) {
        sql.Open();
        query = new SQLiteCommand($"INSERT INTO USERS('Nickname', 'Password') VALUES(@{Nickname}, @Password)", sql);
        query.Parameters.Add($"@{Nickname}", System.Data.DbType.String);
        query.Parameters[$"@{Nickname}"].Value = Nickname;
        query.Parameters.Add($"@Password", System.Data.DbType.String);
        query.Parameters[$"@Password"].Value = Password;
        query.ExecuteNonQuery();
        sql.Close();
    }

    public bool UserExists(string Nickname) {
        sql.Open();
        query = new SQLiteCommand($"SELECT EXISTS(SELECT 1 FROM USERS WHERE Nickname = @{Nickname})", sql);
        query.Parameters.Add($"@{Nickname}", System.Data.DbType.String);
        query.Parameters[$"@{Nickname}"].Value = Nickname;
        Int64 scalar = (Int64)query.ExecuteScalar();
        sql.Close();
        return scalar == 1;
    }
    public bool CheckCredentials(string Id, string password) {
        sql.Open();
        query = new SQLiteCommand($"SELECT EXISTS (SELECT 1 FROM USERS WHERE Id = @Id AND Password = @password)", sql);
        query.Parameters.Add($"@Id", System.Data.DbType.Int32);
        query.Parameters[$"@Id"].Value = Id;
        query.Parameters.Add($"@password", System.Data.DbType.String);
        query.Parameters[$"@password"].Value = password;
        Int64 scalar = (Int64)query.ExecuteScalar();
        sql.Close();
        return scalar == 1;
    }
    public string FromNicknameToId(string Nickname) {
        sql.Open();
        query = new SQLiteCommand($"SELECT Id FROM USERS WHERE Nickname = @{Nickname}", sql);
        query.Parameters.Add($"@{Nickname}", System.Data.DbType.String);
        query.Parameters[$"@{Nickname}"].Value = Nickname;
        string result = $"{query.ExecuteScalar()}";
        sql.Close();
        return result;
    }
}