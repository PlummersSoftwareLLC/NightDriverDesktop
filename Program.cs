//+--------------------------------------------------------------------------
//
// NightDriver.Net - (c) 2019 Dave Plummer.  All Rights Reserved.
//
// File:        Program.cs
//
// Description:
//
//   A WinForms app that hosts a server that can be used to control LED strips
//   via a network connection. 
//
// History:     Dec-23-2023        Davepl      Created
//
//---------------------------------------------------------------------------

using NightDriver;

namespace NightDriver
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }

    public static class StringExtensions
    {
        public static bool IsInRange(this string str, int low, int high, out int number)
        {
            number = 0;
            return int.TryParse(str, out number) && number >= low && number <= high;
        }
    }
}