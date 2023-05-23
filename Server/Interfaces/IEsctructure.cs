/// <summary>
/// Interfaz para convertir al objeto en un tipo esctructura
/// </summary>
public interface IEsctructure {
    public void Accept(IEsctructureVisitor visitor);
}