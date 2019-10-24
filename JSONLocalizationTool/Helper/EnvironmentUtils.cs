using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace JSONLocalizationTool.Helper
{
    public static class EnvironmentUtils
    {
        public static string GetAppData()
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            folder += "\\" + getApplicationName();

            Directory.CreateDirectory(folder);

            return folder;
        }

        public static string GetCommonDocuments()
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
            folder += "\\" + getApplicationName();

            Directory.CreateDirectory(folder);

            return folder;
        }

        public static string GetProgramDirectory()
        {
            string executableName = Assembly.GetExecutingAssembly().Location;
            FileInfo executableFileInfo = new FileInfo(executableName);
            string executableDirectoryName = executableFileInfo.DirectoryName;

            return executableDirectoryName + "\\";
        }


        public static string getExecuteexecutableFile()
        {
            string exe = System.IO.Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);
            return exe;
        }

        public static string getApplicationName()
        {
            string exe = getExecuteexecutableFile();
            exe = exe.Replace(".exe", "");
            return exe;
        }

        public static string GetLogDirectory()
        {
            string folder = GetAppData();

            folder += "logs\\";

            Directory.CreateDirectory(folder);

            return folder;
        }

        public static string GetCurrentVersion()
        {
            string result = "";
            try
            {
                result = AssemblyName.GetAssemblyName(EnvironmentUtils.GetProgramDirectory() + EnvironmentUtils.getExecuteexecutableFile()).Version.ToString();
            }
            catch (Exception)
            {
            }

            return result;
        }

        public static string GetWindwosClientVersion()
        {
            int major = Environment.OSVersion.Version.Major;
            int minor = Environment.OSVersion.Version.Minor;
            int build = Environment.OSVersion.Version.Build;


            if (major == 4 && minor == 0 && build == 950)
                return "Win95 Release 1";
            else if (major == 4 && minor == 0 && build == 1111)
                return "Win95 Release 2";
            else if (major == 4 && minor == 3 && (build == 1212 || build == 1213 || build == 1214))
                return "Win95 Release 2.1";
            else if (major == 4 && minor == 10 && build == 1998)
                return "Win98";
            else if (major == 4 && minor == 10 && build == 2222)
                return "Win98 Second Edition";
            else if (major == 4 && minor == 90)
                return "WinMe";
            else if (major == 5 && minor == 0)
                return "Win2000";
            else if (major == 5 && minor == 1 && build == 2600)
                return "WinXP";
            else if (major == 6 && minor == 0)
                return "Vista";
            else if (major == 6 && minor == 1)
                return "Win7";
            else if (major == 6 && minor == 2 && build == 9200)
                return "Win8 | Win8.1";
            else if (major == 6 && minor == 2 && build == 9600)
                return "Win8.1 Update 1";
            else if (major == 10 && minor == 0 && build == 10240)
                return "Win10";
            else
                return "Can not find os version.";
        }

        public static bool IsWindowsVersion8OrHigher()
        {
            int major = Environment.OSVersion.Version.Major;
            int minor = Environment.OSVersion.Version.Minor;
            int build = Environment.OSVersion.Version.Build;

            // Windows 7
            if (major < 6)
                return false;

            // windows 10
            if (major >= 7)
                return true;

            // Windows 8
            if (major >= 6 && minor >= 2 && build >= 9200)
                return true;

            return false;
        }
    }
}
