namespace ScrumPowerTools
{
    public interface IPackageServiceProvider
    {
        T GetService<T>() where T : class;
    }
}
