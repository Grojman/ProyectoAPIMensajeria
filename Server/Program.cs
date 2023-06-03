global using System.Net.Sockets;
global using System.Net;
global using System.Text;
global using System.Text.RegularExpressions;
global using System.Data.SQLite;
global using System.Security.Cryptography;
class Program {
    public static void Main(string[] args) {
        var ch = new ConectionHandler("127.0.0.1",  14000, new StandarUserHandler('/', new SQLiteHandler("Chat.db", "GROUPS", "USERS", "MESSAGES")));
    }
}