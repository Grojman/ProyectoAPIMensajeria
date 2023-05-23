public class Minero : IPerson
{
    public int EstadoMinas {get => EstadoMinas; set {
        EstadoMinas = EstadoMinas - value < 0 ? 0 : EstadoMinas - value;
    }}
    public int OroConseguido { get {
        int value = 0;
        for (int i = 0; i < Amount; i++) {
            value += new Random().Next(0, EstadoMinas);
            EstadoMinas -= new Random().Next(0, 2);
        }
        return value;
    }}
    public int Amount { get; set;}

    public void Accept(IPersonVisitor visitor) => visitor.DoForMinero(this);
}