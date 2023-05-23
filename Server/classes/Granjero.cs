public class Granjero : IPerson {
    public int Cansancio;
    public int Cosecha {get {
        Cansancio += 10;
        return Cansancio > 50 ? 0 : Amount / Cansancio;
    }}
    public int Amount { get; set; }

    public void Accept(IPersonVisitor visitor) => visitor.DoForGranjero(this);
}