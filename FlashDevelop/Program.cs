using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using PluginCore.Utilities;
using PluginCore.Managers;
using PluginCore.Helpers;
using FlashDevelop;

namespace FlashDevelop
{
    static class Program
    {
        [STAThread]
        static void Main(String[] arguments)
        {
            if (SingleInstanceApp.AlreadyExists)
            {
                if (!MultiInstanceMode) SingleInstanceApp.NotifyExistingInstance(arguments);
                else RunFlashDevelopWithErrorHandling(arguments, false);
            }
            else RunFlashDevelopWithErrorHandling(arguments, true);
        }

        /// <summary>
        /// Run FlashDevelop and catch any unhandled exceptions.
        /// </summary>
        static void RunFlashDevelopWithErrorHandling(String[] arguments, Boolean isFirst)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainForm.IsFirst = isFirst;
            MainForm.Arguments = arguments;
            MainForm mainForm = new MainForm();
            SingleInstanceApp.NewInstanceMessage += delegate(Object sender, Object message)
            {
                MainForm.Arguments = message as String[];
                mainForm.ProcessParameters(message as String[]);
            };
            try
            {
                SingleInstanceApp.Initialize();
                Application.Run(mainForm);
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an unexpected problem while running FlashDevelop: " + ex.Message, "Error");
            }
            finally
            {
                SingleInstanceApp.Close();
            }
        }

        /// <summary>
        /// Checks if we should run in multi instance mode.
        /// </summary>
        public static Boolean MultiInstanceMode
        {
            get 
            {
                String file = Path.Combine(PathHelper.AppDir, ".multi");
                return File.Exists(file);
            }
        }

    }
    
}
