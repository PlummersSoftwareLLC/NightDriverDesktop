//+--------------------------------------------------------------------------
//
// NightDriver - (c) 2018 Dave Plummer.  All Rights Reserved.
//
// File:        NightDriver - Exterior LED Wireless Control
//
// Description:
//
//   Draws remotely on LED strips via WiFi
//
// History:     Oct-12-2018     davepl      Created
//              Jun-06-2018     davepl      C# rewrite from C++
//
//---------------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace NightDriver
{
    public class ConsoleApp
    {
        CancellationToken _token;
        public static Statistics Stats = new Statistics();

        // REVIEW - Best was I can find at the moment to conirm whether a console is present.
        //          If not, we might be under Docker, etc, so don't try to use the console

        private static bool _AlreadyFailedConsole = false;

        internal static bool SystemHasConsole
        {
            get
            {
                try
                {
                    if (_AlreadyFailedConsole)
                        return false;

                    if (Console.WindowHeight > 0 && Console.WindowWidth > 0)
                        return true;
                }
                catch (Exception)
                {
                    _AlreadyFailedConsole = true;
                    return false;
                }
                return false;
            }
        }

        protected static void myCancelKeyPressHandler(object sender, ConsoleCancelEventArgs args)
        {
            Stats.WriteLine($"  Key pressed: {args.SpecialKey}");
            Stats.WriteLine($"  Cancel property: {args.Cancel}");

            // Set the Cancel property to true to prevent the process from terminating.
            Console.WriteLine("Setting the bShouldExit property to true...");

            args.Cancel = false;
        }

        internal void Start(CancellationToken token)
        {

        }
    }
}
