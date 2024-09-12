//+--------------------------------------------------------------------------
//
// NightDriver.Net - (c) 2019 Dave Plummer.  All Rights Reserved.
//
// File:        LERServer.cs
//
// Description:
//
//   A server object that can load, start, and stop the LightStrips 
//
// History:     Dec-23-2023        Davepl      Created
//
//---------------------------------------------------------------------------

using Newtonsoft.Json;

namespace NightDriver
{
    internal class LEDServer
    {
        public bool IsRunning
        {
            get;
            private set;
        } = false;

        public Statistics Stats = new Statistics();

        // Main
        //
        // Application main loop - starts worker threads

        private List<Site> SampleLocations = new List<Site>
        {
            new Banner()            { FramesPerSecond = 60 },  
            new Tree()              { FramesPerSecond = 28 },

            new CeilingStrip        { FramesPerSecond = 30 },

            new Cabana()            { FramesPerSecond = 20 },

            new TV()                { FramesPerSecond = 20 },
            new ShopCupboards()     { FramesPerSecond = 20 },

            new ShopSouthWindows1() { FramesPerSecond = 2 },
            new ShopSouthWindows2() { FramesPerSecond = 2 },
            new ShopSouthWindows3() { FramesPerSecond = 2 },
        };

        internal class _Installation
        {
            public List<LightStrip> _AllStrips { get; set; }
            public Dictionary<string, Site> _AllSites { get; set; }
        };

        public List<LightStrip> AllStrips = new List<LightStrip>();
        public Dictionary<string, Site> AllSites = new Dictionary<string, Site>();

        internal bool SaveStrips()
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                string jsonString = JsonConvert.SerializeObject(AllSites, settings);
                File.WriteAllText("AllStrips.json", jsonString); // Write to a file
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during serialization: {ex.Message}");
                return false;
            }
        }

        public bool LoadStrips()
        {
            try
            {
                string jsonString = File.ReadAllText("AllStrips.json");

                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    ObjectCreationHandling = ObjectCreationHandling.Replace
                };

                AllSites = JsonConvert.DeserializeObject<Dictionary<string, Site>>(jsonString, settings);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during deserialization: {ex.Message}");
                return false;
            }

            // Go through each of the Site objects and see if we have a matching Site already
            // in the AllSites dictionary. If not, add it, and if so, add the strips to the
            // existing site.

            foreach (var site in AllSites.Values)
            {
                foreach (var strip in site.LightStrips)
                {
                    strip.StripSite = site;
                    AllStrips.Add(strip);
                }
            }

            return true;
        }

        internal void LoadStripsFromTable()
        {
            AllSites.Clear();
            AllStrips.Clear();

            foreach (var location in SampleLocations)
            {
                AllSites[location.Name] = location;

                foreach (var strip in location.LightStrips)
                {
                    strip.StripSite = location;
                    AllStrips.Add(strip);
                }
            }
        }

        private void StartCommunications()
        {
            foreach (var location in AllSites.Values)
                foreach (var strip in location.LightStrips)
                    strip.Start();
        }

        private void StopCommunications()
        {
            foreach (var location in AllSites.Values)
                foreach (var strip in location.LightStrips)
                    strip.Stop();
        }


        internal void Start(CancellationToken token)
        {
            IsRunning = true;
            foreach (var location in AllSites.Values)
                location.StartWorkerThread(token);
            StartCommunications();
        }

        internal void Stop(CancellationTokenSource cancelSource)
        {
            cancelSource.Cancel();
            StopCommunications();
            IsRunning = false;
        }

        public void RemoveStrip(LightStrip strip)
        {
            strip.Stop();
            var siteContainingStrip = AllSites.Values.FirstOrDefault(site => site.LightStrips.Contains(strip));
            siteContainingStrip.LightStrips.Remove(strip);
            AllStrips.Remove(strip);
        }

    }
}
