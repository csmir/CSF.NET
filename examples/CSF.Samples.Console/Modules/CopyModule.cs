﻿using CSF.Core;
using System.Diagnostics;

namespace CSF.Samples
{
    public class CopyModule : ModuleBase
    {
        [RequireOperatingSystem(PlatformID.Win32NT)]
        public void Copy([Remainder] string toCopy)
        {
            Process clipboardExecutable = new()
            {
                StartInfo = new ProcessStartInfo
                {
                    RedirectStandardInput = true,
                    FileName = "clip",
                }
            };
            clipboardExecutable.Start();

            clipboardExecutable.StandardInput.Write(toCopy);
            clipboardExecutable.StandardInput.Close();

            Console.WriteLine("Succesfully copied the content to your clipboard.");
        }
    }
}
