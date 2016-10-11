using System;
using System.Diagnostics;

namespace Syn.VA.Plugins.Skype
{
    public static class SkypeCommandLauncher
    {
        public static void StartCall(string userId)
        {
            try
            {
                //var tempFile = SynUtility.File.GetTempFileName(".cmd");
                //var commandString = $"skype \"/callto:{userId}\"";
                //File.WriteAllText(tempFile, commandString);
                Process.Start("skype", $"\"/callto:{userId}\"");
            }
            catch (Exception exception)
            {
                VirtualAssistant.Instance.Logger.Error(exception);
            }
        }
    }
}