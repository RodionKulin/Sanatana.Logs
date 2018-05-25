using Sanatana.Logs.FileWriter;
using Sanatana.Logs.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExampleWebApi
{
    public static class LogsConfig
    {
        public static void ConfigureWebLogs()
        {
            WebLogsRegister register = new WebLogsRegister();

            register.RegisterLogWriter(new FileDiagnosticLogWriter(register.LogSettingsFactory()));
            register.RegisterLogWriter(new FileErrorLogWriter());
            register.RegisterLogWriter(new FileUsageLogWriter());
            register.RegisterLogWriter(new FilePerfLogWriter());

            WebLogsHub.Configure(register);
        }
    }
}