using System;
using System.Collections.Generic;
using Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;

namespace Services
{
    public class SettingsService:ISettingsService
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private Dictionary<string,string> appSettings;

        public AppSettings settings;


        public SettingsService()
        {

            appSettings = new Dictionary<string, string>();

            GetSettingsFromAppsettings();
            GetSettingsFromSecretService();
            GetSettingsFromEnvironmentVariables();

        }



        public string GetValue(string key)
        {

            if (appSettings.TryGetValue(key, out string value))
            {
                return value;
            }
            else
            {
                AlertKeyNotFound(key);
                return null;
            }
        }


        /// <summary>
        /// Gets the settings from appsettings.json
        /// </summary>
        private void GetSettingsFromAppsettings()
        {


            foreach (var setting in settings.Values)
            {
                if (appSettings.TryAdd(setting.Key, setting.Value))
                {

                    //Log info
                }
                else
                {
                    AlertDuplicateKey(setting.Key, "appsettings.json");
                }
            }
        }

        private void GetSettingsFromSecretService()
        {
            //TODO: Get actual settings
            var ssSettings = new Dictionary<string, string>();

            foreach (var setting in ssSettings)
            {
                if (appSettings.TryAdd(setting.Key, setting.Value))
                {
                    //Log info
                }
                else
                {
                    AlertDuplicateKey(setting.Key, "secret service");
                }
            }
        }

        private void GetSettingsFromEnvironmentVariables()
        {
            //TODO: Get actual settings
            var envSettings = new Dictionary<string, string>();

            foreach (var setting in envSettings)
            {
                if (appSettings.TryAdd(setting.Key, setting.Value))
                {
                    //Log info
                }
                else
                {
                    AlertDuplicateKey(setting.Key, "environment variable");
                }
            }
        }



        private void AlertDuplicateKey(string key, string source)
        {
            //TODO: Add logger
        }

            private void AlertKeyNotFound(string key)
        {
            //TODO: Add logger
        }

    }
}
