using System.Diagnostics;
using CSF;

namespace XProject
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

            Respond("Succesfully copied the content to your clipboard.");
        }
    }
}
