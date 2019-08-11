using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using Newtonsoft;
using Newtonsoft.Json;
using Harmony;
using UnityEngine;

namespace NotificationsPause
{
    public static class NotificationsPause
    {
        private static SettingsFile settings;
        private static bool tryReadOnce = true;
        private static float lastPause = 0f;
        private class SettingsFile
        {
            public string fileversion;
            public float cooldown;
            public SortedDictionary<string, bool> PauseOnNotification;
        }

        [HarmonyPatch(typeof(Notification), "IsReady")]
        public static class Notification_IsReady_Patch
        {
            private static void readSettings()
            {
                FileInfo sfile = new FileInfo(Assembly.GetExecutingAssembly().Location);
                DirectoryInfo dirInfo = sfile.Directory;
                string settingpath = dirInfo.FullName + "/settings.json";

                FileInfo filechecker = new FileInfo(settingpath);
                if (!filechecker.Exists)
                    return;

                StreamReader sr = new StreamReader(settingpath);
                string settstr=sr.ReadToEnd();
                try {
                    settings = Newtonsoft.Json.JsonConvert.DeserializeObject<SettingsFile>(settstr);
                } catch (JsonReaderException exc)
                {
                    sr.Close();
                    sr.Dispose();
                    Debug.Log("Critical Notification Pauser: Error reading Json");
                }
                sr.Close();
                sr.Dispose();
            }

            public static void Postfix(ref Notification __instance)
            {
                if (tryReadOnce && settings == null)
                {
                    //First notification, read file and stuff
                    readSettings();
                    tryReadOnce = false;
                }

                
                if ((!(SpeedControlScreen.Instance.IsPaused)) && settings != null && settings.PauseOnNotification != null && settings.PauseOnNotification.ContainsKey(__instance.titleText))
                {
                    if (Time.time - lastPause > settings.cooldown)
                    {
                        //If the title is set in the settings, use that
                        if (settings.PauseOnNotification[__instance.titleText])
                        {
                            SpeedControlScreen.Instance.Pause();
                            lastPause = Time.time;
                        }
                    }
                }
                else {
                    //... otherwise use default behaviour
                    if (__instance.Type == NotificationType.Bad || __instance.Type == NotificationType.DuplicantThreatening)
                    {
                        if (!((__instance.titleText == "Combat!") || (__instance.titleText == "Missing Research Station") || (__instance.titleText == "No Researchers assigned") || (__instance.titleText == "Yellow Alert") || (__instance.titleText == "Red Alert")))
                        {
                            if (Time.time - lastPause > 1.0)
                            {
                                SpeedControlScreen.Instance.Pause();
                                lastPause = Time.time;
                            }
                        }
                    }
                }
            }
        }
    }
}
