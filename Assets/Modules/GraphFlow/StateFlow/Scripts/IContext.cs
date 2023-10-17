
namespace StateFlow {

    public interface IContext : InvokeFlow.IContext {
        StateGraphExecutor executor { get; set; }
    }

}