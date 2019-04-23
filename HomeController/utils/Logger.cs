using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.Diagnostics;

namespace HomeController.utils
{
    public class Logger
    {
        public const string RCUProxy = "RCUProxy";
        public const string LCU = "LCU";
        public const string Test = "Test";

        static  Logger()
        {
            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                var path = localFolder.Path;
                Debug.WriteLine("Path is " + path);
            }
            catch 
            {
            }
        }

        public static async void Logg(string text)
        {
            Logg(null, text, "rpi.txt");
        }
        public static async void Logg(string category, string text, string fileName)
        {
            #region Unused Code
            //using (StreamWriter outputFile = new StreamWriter(new FileStream(@"c:\temp\rpi.txt", FileMode.Append)))
            //{
            //    var now = DateTime.Now;
            //    outputFile.WriteLine(now.ToString(Definition.StandardDateTimeFormat) + ": " + text);
            //}

            // Code from https://docs.microsoft.com/en-us/uwp/api/windows.storage.storagefolder.getfolderfrompathasync
            // Get the path to the app's Assets folder.

            // Get the folder object that corresponds to this absolute path in the file system.
            //StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(path);

            //string root = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
            //string path = root + @"\Assets";

            // To read: https://stackoverflow.com/questions/35747635/perform-synchronous-operation-on-ui-thread

            // Code from: https://www.redgreencode.com/time-tortoise-reading-writing-text-files-uwp-apps/
            //await Task.Run(() => File.AppendAllText(Path.Combine(path, "rpi.txt"), now.ToString(Definition.StandardDateTimeFormat) + ": " + text));

            // https://stackoverflow.com/questions/33193988/unauthorizedaccessexception-writing-to-file-using-cortanas-universal-windows-se
            // Ger "because it is being used by another process" efter några skrivningar
            //await Task.Run(() =>
            //{
            //    using (TextWriter writer = File.CreateText(Path.Combine(path, "rpi.txt")))
            //    {
            //        writer.Write(text);

            //    }
            //});
            #endregion

            if (category != null)
            {
                text = category + "; " + text;
            }
            // This is not a very nice way to write to a log file since we need to make several attempts.
            // The exception is about the text file itself: "because it is being used by another process"
            // There is something fundamental wrong about it but at least it works so it will have to do right now.
            var now = DateTime.Now;

            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            var path = localFolder.Path;

//path = @"c:\temp\";   

            int count = 1;
            string countString = "";
            while (count < 10)
            {
                try
                {
                    // C:\Users\makl\AppData\Local\Packages\9c6bbe75-87fc-407f-9ad3-a5035f3268a0_n0repxk218c66\LocalState
                    //await Task.Run(() => File.AppendAllText(Path.Combine(path, "rpi.txt"),
                    //now.ToString(Definition.StandardDateTimeFormat) + countString + ": " + text + "\r\n"));

                    string stringToWrite = now.ToString(Definition.StandardDateTimeFormat + ".ff") + countString +
                                           ": " + text + "\r\n";
                    File.AppendAllText(Path.Combine(path, fileName), stringToWrite);
                    Debug.WriteLine(stringToWrite);
                    break;
                }
                catch (IOException ex)
                {
                    int a = 0;
                    Task.Delay(1).Wait();
                }

                count++;
                countString = " [" + count + "]";
            }
        }

        public static void Logg(string category, string s)
        {
            Logg(category, s, "rpi.txt");
        }
    }
}
