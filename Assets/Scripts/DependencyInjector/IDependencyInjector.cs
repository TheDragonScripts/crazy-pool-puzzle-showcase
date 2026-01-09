namespace SDI
{
    public interface IDependencyInjector
    {
        void Register(params object[] dependencies);
        void Register<T>(T dependency);
        void Inject<T>(T target);
        void Inject(params object[] targets);
    }
}