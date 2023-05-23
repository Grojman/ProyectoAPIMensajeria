/// <summary>
/// Interfaz que para interactuar con tipos Persona
/// </summary>
public interface IPersonVisitor {
    //FUNCIONES PARA INTERACTUAR CON LOS NPC'S Y LAS ESTERUCTURAS
    void DoForGranjero(Granjero granjero);
    void DoForMinero(Minero minero);
}