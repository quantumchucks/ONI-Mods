using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using UnityEngine;

namespace NotificationsPause
{
    public static class NotificationsPause
    {
        [HarmonyPatch(typeof(Notification), "IsReady")]
        public static class Notification_IsReady_Patch
        {
            public static void Postfix(ref Notification __instance)
            {
                if (__instance.Type == NotificationType.Bad || __instance.Type == NotificationType.DuplicantThreatening)
                {
                    if (!(__instance.titleText=="Combat!"))
                        SpeedControlScreen.Instance.Pause();
                }
            }
        }
    }
}
