/// <summary>
/// Interfaz que permite interactuar con el objeto como un tipo Persona
/// </summary>
public interface IPerson {
    public int Amount {get; set;}
    public void Accept(IPersonVisitor visitor);
}