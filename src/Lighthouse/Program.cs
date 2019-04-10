// Copyright 2014-2015 Aaron Stannard, Petabridge LLC
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.

using System.Configuration;
using Topshelf;

namespace Lighthouse
{
    class Program
    {
        static int Main(string[] args)
        {
            return (int) HostFactory.Run(x =>
            {
                string version = typeof(LighthouseService).Assembly.GetName().Version.ToString();
                x.Service<LighthouseService>(s =>
                {
                    s.ConstructUsing(ss => new LighthouseService());
                    s.WhenStarted(ss => ss.Start());
                    s.WhenStopped(ss => ss.StopAsync().Wait());
                });

                var lighthouseServiceName = ConfigurationManager.AppSettings["IsolatedServiceLighthouseName"] ?? "lighthouse";

                x.SetServiceName("Ilab_" + lighthouseServiceName + "_" + version + " (Instance: " + ConfigurationManager.AppSettings.Get("Environment") + ")");
                x.SetDisplayName("Ilab_"+ lighthouseServiceName + "_" + version + " (Instance: " + ConfigurationManager.AppSettings.Get("Environment") + ")");
                x.SetDescription("Ilab_"+ lighthouseServiceName + "_" + version + " (Instance: " + ConfigurationManager.AppSettings.Get("Environment") + ")");

                x.RunAsNetworkService();
                x.StartAutomatically();
                x.UseNLog();
                x.EnableServiceRecovery(r => r.RestartService(1));
            });
        }
    }
}
