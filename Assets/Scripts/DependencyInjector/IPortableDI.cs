namespace SDI
{
    public interface IPortableDI
    {
        void Inject<T>(T obj);
        void SetPrimaryInjector(IDependencyInjector injector);
    }
}