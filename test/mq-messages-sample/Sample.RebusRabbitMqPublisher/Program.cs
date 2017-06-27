using System;
using Topshelf;

namespace Sample.RebusRabbitMqPublisher
{
    class Program
    {
        static int Main()
        {

            return (int)HostFactory.Run(x =>
            {
                x.UseAssemblyInfoForServiceInfo();

                x.Service<SampleRebusRabbitMqPublisherBootstrap>(s =>
                {
                    s.ConstructUsing(() => new SampleRebusRabbitMqPublisherBootstrap());
                    s.WhenStarted(v => v.Start());
                    s.WhenStopped(v => v.Stop());
                    s.BeforeStartingService(_ => { Console.WriteLine("Processor is starting"); });
                    s.BeforeStoppingService(_ => { Console.WriteLine("Processor is stopping"); });
                });

                // x.StartAutomatically();

                x.SetStartTimeout(TimeSpan.FromSeconds(10));
                x.SetStopTimeout(TimeSpan.FromSeconds(10));

                x.EnableServiceRecovery(r =>
                {
                    r.RestartService(1);
                    //r.RunProgram(7, "ping google.com");
                    r.RestartComputer(5, "message");

                    r.OnCrashOnly();
                    r.SetResetPeriod(2);
                });

                //x.AddCommandLineSwitch("throwonstart", v => throwOnStart = v);
                //x.AddCommandLineSwitch("throwonstop", v => throwOnStop = v);
                //x.AddCommandLineSwitch("throwunhandled", v => throwUnhandled = v);

                x.OnException((exception) =>
                {
                });
            });
        }
    }
}
