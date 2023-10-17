
namespace CalcExpr {
    public interface IIndexFunction {
        bool call(int index, uint[] argv, out uint ret);
    }
}