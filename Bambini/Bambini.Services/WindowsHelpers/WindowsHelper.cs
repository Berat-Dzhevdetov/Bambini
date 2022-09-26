namespace Bambini.Services.WindowsHelpers
{
    using Microsoft.Win32;
    using System;
    using System.Diagnostics;

    internal class WindowsHelper : IWindowsHelper
    {
        #region Constructors
        public WindowsHelper()
        {
            DefaultBrowser = GetSystemDefaultBrowser();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Returns the default browser exe file.
        /// </summary>
        public string DefaultBrowser { get; init; }
        #endregion

        #region Public methods
        public void EnsureFile(string path)
        {
            if (File.Exists(path))
            {
                File.Create($"{path}");
            }
        }

        /// <summary>
        /// Executes a command on the machine
        /// </summary>
        /// <param name="filename">Filename (should the be exe)</param>
        /// <param name="arguments">Arguments that are needed to execute the file</param>
        /// <returns>Status of what happened during the procces</returns>
        /// <exception cref="ArgumentNullException">Could be thrown if the filename is null</exception>
        public bool ExecuteCommand(string filename, string arguments)
        {
            var process = new Process();

            var startInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                ErrorDialog = true
            };

            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException("Excepted parameter filename to have a value");
            }

            startInfo.FileName = filename;

            if (!string.IsNullOrEmpty(arguments))
            {
                startInfo.Arguments = arguments;
            }

            process.StartInfo = startInfo;

            try
            {
                process.Start();
                // We need to wait the process to exit to get the exit code
                process.WaitForExit();
            }
            catch (Exception e)
            {
                throw new Exception("OS error while executing: " + e.Message, e);
            }

            return process.ExitCode == 0;
        }
        #endregion

        #region Private Methods
        private string GetSystemDefaultBrowser()
        {
            string name = string.Empty;
            RegistryKey regKey = null;

            try
            {
                //set the registry key we want to open
                regKey = Registry.ClassesRoot.OpenSubKey("HTTP\\shell\\open\\command", false);

                //get rid of the enclosing quotes
                name = regKey?.GetValue(null)?.ToString()?.ToLower()?.Replace("" + (char)34, "");

                //check to see if the value ends with .exe (this way we can remove any command line arguments)
                if (!name.EndsWith("exe"))
                {
                    //get rid of all command line arguments (anything after the .exe must go)
                    name = name[..(name.LastIndexOf(".exe") + 4)];

                    if (name.Contains("internet explorer\\iexplore.exe"))
                    {
                        //Instead of opening Internet Explorer opens Microsoft Edge
                        name = @"C:\\Program Files (x86)\\Microsoft\\Edge\\Application\\msedge.exe";
                    }
                }
            }
            catch (Exception ex)
            {
                name = string.Format("ERROR: An exception of type: {0} occurred in method: {1} in the following module: {2}", ex.GetType(), ex.TargetSite, this.GetType());
            }
            finally
            {
                //check and see if the key is still open, if so
                //then close it
                if (regKey != null)
                    regKey.Close();
            }

            //return the value
            return name;
        }
        #endregion
    }
}
