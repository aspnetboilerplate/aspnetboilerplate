using Abp;

namespace AbpQuartzDemo.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var bootstrapper = AbpBootstrapper.Create<AbpQuartzDemoConsoleAppModule>();
            bootstrapper.Initialize();
        }
    }
}