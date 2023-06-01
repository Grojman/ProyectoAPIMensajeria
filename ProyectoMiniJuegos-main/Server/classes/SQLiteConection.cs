/// <summary>
/// An object that allows to comunicate to a DataBase throught SQlite. Note that this is a implementation example, using a DataBase made by myself. You might want to refactor this class from scrath, if you want to use it with your own DB.
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
            sql = new SQLiteConnection(dbPath);
        } catch {
            throw new IDataBase.DataBaseConectionException($"Se ha producido un error al intentar conectarse a la base de datos. Ruta: {dbPath}");
        }
    }

    //PARAMETRIZAR ESTAS FUNCIONES PARA EVITAR CODIGO REPETIDO

    public string[] FindUsersFromConversation(string conversationId, string userId)
    {
        var users = new List<string>();
        sql.Open();
        query = new SQLiteCommand($"SELECT UserId FROM GROUPS WHERE GroupId = @{conversationId} AND UserId != @{userId}");
        query.Parameters.Add($"@{conversationId}", System.Data.DbType.String);
        query.Parameters[$"@{conversationId}"].Value = conversationId;
        query.Parameters.Add($"@{userId}", System.Data.DbType.String);
        query.Parameters[$"@{userId}"].Value = userId;
        var reader = query.ExecuteReader();
        while(reader.Read()) {
            users.Append(reader.GetString(0));
        }
        sql.Close();
        return users.ToArray();
    }

    public string[] GetConversations(string userId)
    {
        //LAS CONVERASCIONES ENTRE DOS PERSONAS SE GUARDAN IGUAL QUE LAS CONVERSACIONES DE GRUPO
        var conversations = new List<string>();
        sql.Open();
        query = new SQLiteCommand($"SELECT GroupId FROM GROUPS WHERE UserId = @{userId}");
        query.Parameters.Add($"@{userId}", System.Data.DbType.String);
        query.Parameters[$"@{userId}"].Value = userId;
        var reader = query.ExecuteReader();
        while(reader.Read()) {
            conversations.Append(reader.GetString(0));
        }
        //ESTO SE HACE EN CASO DE QUE EN GROUPS NO SE GUARDE LAS CONVERSACIONES DE DOS PERSONAS
        // query = new SQLiteCommand($"SELECT DISTINCT DestinationId FROM MESSAGES WHERE SenderId = @{userId}");
        // query.Parameters.Add($"@{userId}", System.Data.DbType.String);
        // query.Parameters[$"@{userId}"].Value = userId;
        // var reader = query.ExecuteReader();
        // while(reader.Read()) {
        //     var conv = readoer.GetString(0);
        //     if (!conversations.Contains(conv))
        //         conversations.Append(conv);
        // }
        
        sql.Close();
        return conversations.ToArray();
    }

    public string[] GetMessages(string conversationId, int amount)
    {
        var messages = new List<string>();
        sql.Open();
        query = new SQLiteCommand($"SELECT * FROM MESSAGES WHERE DestinationId = @{conversationId} ORDER BY Date LIMIT @{amount}");
        query.Parameters.Add($"@{conversationId}", System.Data.DbType.String);
        query.Parameters[$"@{conversationId}"].Value = conversationId;
        query.Parameters.Add($"@{amount}", System.Data.DbType.Int32);
        query.Parameters[$"@{amount}"].Value = amount;
        sql.Close();
        return messages.ToArray();
    }

    public void SaveGroup(string[] users)
    {
        throw new NotImplementedException();
    }

    public void SaveMessage(string sender, string destination, string message)
    {
        throw new NotImplementedException();
    }

    public bool UserExists(string Id)
    {
        throw new NotImplementedException();
    }
}