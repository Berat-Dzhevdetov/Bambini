namespace Bambini.Services.WindowsHelper
{
    using System;
    using Microsoft.Win32;

    public class WindowsHelper : IWindowsHelper
    {
        #region Constructors
        public WindowsHelper(string asd)
        {
            DefaultBrowser = GetSystemDefaultBrowser();
        }
        #endregion

        #region Properties
        public string DefaultBrowser { get; init; }
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
                name = regKey.GetValue(null).ToString().ToLower().Replace("" + (char)34, "");

                //check to see if the value ends with .exe (this way we can remove any command line arguments)
                if (!name.EndsWith("exe"))
                {
                    //get rid of all command line arguments (anything after the .exe must go)
                    name = name.Substring(0, name.LastIndexOf(".exe") + 4);

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
