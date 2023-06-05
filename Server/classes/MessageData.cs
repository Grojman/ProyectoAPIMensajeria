public class MessageData {
    public int Sender {get;}
    public int Destination {get;}
    public string Message {get;}
    public string Date {get;}
    public MessageData(int sender, int destination, in string message, in string date) {
        Sender = sender;
        Destination = destination;
        Message = message;
        Date = date;
    }
    public string ToJson() => $"\"Sender\" : \"{Sender}\", \"Destination\" : \"{Destination}\", \"Message\" : \"{Message}\", \"Date\" : \"{Date}\"";
}