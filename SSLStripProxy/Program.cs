using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SSLStripProxy
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Logger.MainForm = new MainForm();
            new ClientListener(80).StartListening();
            Application.Run(Logger.MainForm);
            //Logger.SaveSets();
            
        }
    }
}
