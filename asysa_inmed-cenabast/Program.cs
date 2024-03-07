using System;
using System.ServiceProcess;

namespace asysa_inmed_cenabast
{
    internal static class Program
    {
        private static void Main()
        {
            ServiceBase[] servicesToRun;
            servicesToRun = new ServiceBase[]
            {
                new Service1()
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}

