public class RecursosVisitor : IPersonVisitor
{
    Recursos recursos;
    public RecursosVisitor(Aldea aldea) {
        recursos = new Recursos();
        aldea.granjero.Accept(this);
        aldea.minero.Accept(this);
    }
    public void DoForGranjero(Granjero granjero)
    {
        recursos.Comida = granjero.Cosecha;
    }

    public void DoForMinero(Minero minero)
    {
        recursos.Oro = minero.OroConseguido;
    }
    public Recursos getRecursos() => recursos;
}