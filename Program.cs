﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Targets;

using NLog;
using NLog.Config;
using NLog.Web;
using Logzio.DotNet.NLog;

namespace crds_core_service_template
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ReadEnvironmentVariables();

            var logger = SetUpLogging();

            try
            {
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                //NLog: catch setup errors
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .UseStartup<Startup>()
                   .ConfigureLogging(logging =>
                   {    
                        logging.ClearProviders();
                        logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                   })
                   .UseNLog();  // NLog: setup NLog for Dependency injection

        static void ReadEnvironmentVariables()
        {
            //TODO: Autogenerate a file, whether its .env or .json is TBD

            try
            {
                DotNetEnv.Env.Load("../.env");
            }
            catch (Exception)
            {
                // no .env file present but since not required, just write
                Console.WriteLine("no .env file found, reading environment variables from machine");
            }

            // TODO: Read these values out of a file instead of hard coding here
            List<string> requiredEnvVariables = new List<string>() {
                "MP_OAUTH_BASE_URL",
                "OKTA_OAUTH_BASE_URL",
                "LOGZ_IO_KEY"
            };

            List<string> missingEnvVariables = new List<string>();
            foreach (string envVariable in requiredEnvVariables)
            {
                //TODO: Load this from a singleton of SettingsService
                if (Environment.GetEnvironmentVariable(envVariable) == null)
                {
                    missingEnvVariables.Add(envVariable);
                }
            }
            /*
            if (missingEnvVariables.Count > 0)
            {
                throw new Exception("Must Define Environment Variables: " + String.Join(",", missingEnvVariables.ToArray()));
            }
            */
        }

        static NLog.Logger SetUpLogging()
        {

            //TODO: Load this from a singleton of SettingsService
            var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == EnvironmentName.Development;

            var loggingConfig = new LoggingConfiguration();

            //Set up to log to stdout
            if (isDevelopment)
            {
                var consoleTarget = new ColoredConsoleTarget("console")
                {
                    Layout = @"${date:format=HH\:mm\:ss} ${level} ${message} ${exception:format=ToString}"
                };
                loggingConfig.AddTarget("console", consoleTarget);

                //Log everything in development
                loggingConfig.AddRuleForAllLevels(consoleTarget, "*");
            }
            else // Log to Logzio
            {
                var logzioTarget = new LogzioTarget
                {
                    //TODO: Load this from a singleton of SettingsService
                    Token = Environment.GetEnvironmentVariable("LOGZ_IO_KEY"),
                };
                logzioTarget.ContextProperties.Add(new TargetPropertyWithContext("host", "${machinename}"));
                logzioTarget.ContextProperties.Add(new TargetPropertyWithContext("application", Environment.GetEnvironmentVariable("APP_NAME")));
                logzioTarget.ContextProperties.Add(new TargetPropertyWithContext("environment", Environment.GetEnvironmentVariable("CRDS_ENV")));
                loggingConfig.AddTarget("logzio", logzioTarget);

                //Log only error and above for all built in logs
                loggingConfig.AddRule(NLog.LogLevel.Error, NLog.LogLevel.Fatal, logzioTarget, "*");

                //Log everything debug and above for custom logs
                loggingConfig.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, logzioTarget, "Crossroads.*");

                // Also log to console so we have the info
                var consoleTarget = new ColoredConsoleTarget("console")
                {
                    Layout = @"${date:format=HH\:mm\:ss} ${level} ${message} ${exception:format=ToString}"
                };
                loggingConfig.AddTarget("console", consoleTarget);

                //Log only warn and above for all built in logs
                loggingConfig.AddRule(NLog.LogLevel.Error, NLog.LogLevel.Fatal, consoleTarget, "*");

                //Log everything debug and above for custom logs
                loggingConfig.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, consoleTarget, "Crossroads.*");
            }

            LogManager.Configuration = loggingConfig;

            return NLogBuilder.ConfigureNLog(loggingConfig).GetCurrentClassLogger();
        }
    }
}
