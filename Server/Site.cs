//+--------------------------------------------------------------------------
//
// NightDriver.Net - (c) 2023 Dave Plummer.  All Rights Reserved.
//
// File:        SiteController.cs
//
// Description:
//
// The object that maintains a connection with the LEDStrip and that can send
// data to it and get statistics back from it.
//
// History:     Dec-18-2023        Davepl      Cleanup
//
//---------------------------------------------------------------------------

using System.Diagnostics;
using System.Security.Policy;
using Newtonsoft.Json;


namespace NightDriver
{
    // ScheduledEffect
    //
    // An LED effect with scheduling; adds a start and stop time to an effect

    public class ScheduledEffect
    {
        public const DayOfWeek WeekEnds = DayOfWeek.Saturday | DayOfWeek.Sunday;
        public const DayOfWeek WeekDays = DayOfWeek.Monday | DayOfWeek.Tuesday | DayOfWeek.Wednesday | DayOfWeek.Thursday | DayOfWeek.Friday;
        public const DayOfWeek AllDays = WeekDays | WeekEnds;

        public LEDEffect Effect { get; private set;  }

        public DayOfWeek DaysOfWeek { get; private set; }
        public uint StartHour { get; private set; }
        public uint EndHour { get; private set; }
        public uint StartMinute { get; private set; }
        public uint EndMinute { get; private set; }

        public ScheduledEffect(DayOfWeek daysOfWeek, uint startHour, uint endHour, LEDEffect effect, uint startMinute = 0, uint endMinute = 60)
        {
            DaysOfWeek  = daysOfWeek;
            Effect      = effect;
            StartHour   = startHour;
            EndHour     = endHour;
            StartMinute = startMinute;
            EndMinute   = endMinute;
        }

        [JsonIgnore]
        public bool ShouldEffectRunNow
        {
            get
            {
                if (DaysOfWeek.HasFlag(DateTime.Now.DayOfWeek))
                    if (DateTime.Now.Hour > StartHour || DateTime.Now.Hour == StartHour && DateTime.Now.Minute >= StartMinute)
                        if (DateTime.Now.Hour < EndHour || DateTime.Now.Hour == EndHour && DateTime.Now.Minute <= EndMinute)
                            return true;

                return false;
            }
        }

        [JsonIgnore]
        public uint MinutesRunning
        {
            get
            {
                uint c = 0;
                if (DateTime.Now.Hour > StartHour)
                    c += ((uint)DateTime.Now.Hour - StartHour) * 60;
                if (DateTime.Now.Minute >= StartMinute)
                    c += ((uint)DateTime.Now.Minute - StartMinute);
                return c;
            }
        }

    }

    // Site
    //
    // A "site" is a location where one or more LED strip controllers and the effects that will run on them.  It
    // implements the "GraphicsBase" interface so that the effects can draw upon the "site" as a whole,
    // and it is later divied up to the various controllers.  So if you have 4000 LEDs, you might have
    // four strips with 1000 LEDs each, for example.  Combined with a list of effects, they consitute a site.
    //
    // Put more simply, a site is a collection of strips, and each strip normally begins at its own offset
    // in the larger site.  So if you have 4000 LEDs, you might have four strips with 1000 LEDs each, for example.
    // Combined with a list of effects, they consitute a site.

    [Serializable]
    public class Site : GraphicsBase
    {
        public System.Threading.Thread _Thread;
        public virtual String Name { get; private set; }
        public List<LightStrip> LightStrips { get; set; } = new List<LightStrip>();
        public List<ScheduledEffect> LEDEffects { get; set; } = new List<ScheduledEffect>();
        public int iCurrentEffect = -1;
        public bool Enabled { get; set; } = true;

        [JsonIgnore]
        public CRGB[] LEDs { get; private set; }

        public const int PIXELS_PER_METER144 = 144;

        public Site(String name, uint width, uint height, bool enabled)
        {
            Width     = width;
            Height    = height;
            LEDs      = InitializePixels<CRGB>((int)LEDCount);
            Name      = name;
            StartTime = DateTime.Now;
            Enabled   = enabled;
        }

        public int FramesPerSecond
        {
            get; set;
        } = 22;

        protected CancellationToken _token;
        protected DateTime StartTime;

        protected int SecondsPerEffect
        {
            get
            {
                return 60;
            }
        }

        public uint SpareTime
        {
            get;
            private set;
        } = 1000;

        public int _iEffectOffset = 0;

        public void NextEffect() 
        {
            _iEffectOffset++;
        }

        public void PreviousEffect()
        {
            _iEffectOffset--;
        }

        // If we were certain that every pixel would get touched, and hence created, we wouldn't need to init them, but to
        // be safe, we init them all to a default pixel value (like magenta)

        protected static T[] InitializePixels<T>(int length) where T : new()
        {
            T[] array = new T[length];
            for (int i = 0; i < length; ++i)
            {
                array[i] = new T();
            }

            return array;
        }

        void WorkerDrawAndSendLoop()
        {
            DateTime lastSpareTimeReset = DateTime.UtcNow;
            DateTime timeLastFrame = DateTime.UtcNow -TimeSpan.FromSeconds((FramesPerSecond == 0 ? 0 : 1.0 / FramesPerSecond));

            while (!_token.IsCancellationRequested)
            {
                // If this strip is disabled, we don't render or send data, but we still sleep for a bit to avoid spinning
                if (Enabled == false)
                {
                    Thread.Sleep(100);
                    continue;
                }

                DateTime timeNext = timeLastFrame + TimeSpan.FromSeconds(1.0 / (FramesPerSecond > 0 ? FramesPerSecond : 30));
                DrawAndEnqueueAll(timeNext);
                timeLastFrame = timeNext;

                TimeSpan delay = timeNext - DateTime.UtcNow;
                if (delay.TotalMilliseconds > 0)
                {
                    Thread.Sleep((int) delay.TotalMilliseconds);
                }
                else
                {
                    ConsoleApp.Stats.WriteLine(this.GetType().Name + " dropped Frame by " + delay.TotalMilliseconds);
                    Thread.Sleep(1);
                }

                double spare = delay.TotalMilliseconds <= 0 ? 0 : delay.TotalMilliseconds;
                SpareTime = Math.Min(SpareTime, (uint) spare);

                ConsoleApp.Stats.SpareMilisecondsPerFrame = (uint)delay.TotalMilliseconds;

                if ((DateTime.UtcNow - lastSpareTimeReset).TotalSeconds > 1)
                {
                    SpareTime = 1000;
                    lastSpareTimeReset = DateTime.UtcNow;
                }
            }
            Debug.WriteLine("Leaving WorkerDrawAndSendLooop");
        }

        public void StartWorkerThread(CancellationToken token)
        {
            foreach (var strip in LightStrips)
                strip.StripSite = this; 

            _token = token;
            _Thread = new Thread(WorkerDrawAndSendLoop);
            _Thread.IsBackground = true;
            _Thread.Priority = ThreadPriority.BelowNormal;
            _Thread.Start();
        }

        [JsonIgnore]
        public string CurrentEffectName
        {
            get;
            private set;
        } = "[None]";

        public void DrawAndEnqueueAll(DateTime timestamp)
        {
            DateTime timeStart2 = DateTime.UtcNow;

            var enabledEffects = LEDEffects.Where(effect => effect.ShouldEffectRunNow == true);
            var effectCount = enabledEffects.Count();
            if (effectCount > 0)
            {
                // Currently all effects expire and change over at the same time, since they all share the same interval.  Each effect can be
                // offset within its effect list by N steps positive or negative, and that's how the Effect+ and Effect- buttons work.

                int iEffect = (int)((DateTime.Now - StartTime).TotalSeconds / SecondsPerEffect) + _iEffectOffset;
                iEffect %= effectCount;

                if (iEffect != iCurrentEffect)
                {
                    if (iCurrentEffect >= 0)
                        LEDEffects.ElementAt(iCurrentEffect).Effect.OnStop();
                    LEDEffects.ElementAt(iEffect).Effect.OnStart();
                    iCurrentEffect = iEffect;
                }

                var effect = enabledEffects.ElementAt(iEffect);
                ConsoleApp.Stats.WriteLine("Effect: " + effect.Effect.EffectName + " " + iEffect + " of " + effectCount);

                // We lock the CRGB buffer so that when the UI follows the same approach, we don't
                // get half-rendered frames in the LEDVisualizer

                lock (LEDs)
                    effect.Effect.DrawFrame(this);

                CurrentEffectName = effect.Effect.GetType().Name;
                if ((DateTime.UtcNow - timeStart2).TotalSeconds > 0.25)
                    ConsoleApp.Stats.WriteLine("MAIN3 DELAY");
            }
            else
            {
                CurrentEffectName = "[None Running]";
            }

            if ((DateTime.UtcNow - timeStart2).TotalSeconds > 0.25)
                ConsoleApp.Stats.WriteLine("MAIN2 DELAY");

            foreach (var controller in LightStrips)
            {
                if (controller.ReadyForData)
                    controller.CompressAndEnqueueData(LEDs, timestamp);
                else
                    controller.Response.Reset();
            }
        }

        protected uint GetPixelIndex(uint x, uint y)
        {
            return (y * Width) + x;
        }

        protected void SetPixel(uint x, uint y, CRGB color)
        {

            LEDs[GetPixelIndex(x, Height - 1 - y)] = color;
        }

        protected void SetPixel(uint x, CRGB color)
        {
            LEDs[x] = color;
        }

        protected CRGB GetPixel(uint x)
        {
            if (x < 0 || x >= Width)
                return CRGB.Black;

            return LEDs[GetPixelIndex(x, 0)];
        }

        public override CRGB GetPixel(uint x, uint y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
                return CRGB.Black;

            return LEDs[GetPixelIndex(x, y)];
        }

        public override void DrawPixels(double fPos, double count, CRGB color)
        {
            double availFirstPixel = 1 - (fPos - (uint)(fPos));
            double amtFirstPixel = Math.Min(availFirstPixel, count);
            count = Math.Min(count, DotCount-fPos);
            if (fPos >= 0 && fPos < DotCount)
                BlendPixel((uint)fPos, color.fadeToBlackBy(1.0 - amtFirstPixel));

            fPos += amtFirstPixel;
            //fPos %= DotCount;
            count -= amtFirstPixel;

            while (count >= 1.0)
            {
                if (fPos >= 0 && fPos < DotCount)
                {
                    BlendPixel((uint)fPos, color);
                    count -= 1.0;
                }
                fPos += 1.0;
            }

            if (count > 0.0)
            {
                if (fPos >= 0 && fPos < DotCount)
                    BlendPixel((uint)fPos, color.fadeToBlackBy(1.0 - count));
            }
        }

        public override void DrawPixel(uint x, CRGB color)
        {
            SetPixel(x, color);
        }

        public override void DrawPixel(uint x, uint y, CRGB color)
        {
            SetPixel(x, y, color);
        }

        public override void BlendPixel(uint x, CRGB color)
        {
            CRGB c1 = GetPixel(x);
            SetPixel(x, c1 + color);
        }
    };

    // EffectsDatabase
    //
    // A static database of some predefined effects

    public static class EffectsDatabase
    {
        //        public static LEDEffect FireWindow => new BlueFire(5*144, true)
        //        {
        //           _Cooling = 100
        //        };

        public static LEDEffect FireWindow => new FireEffect(100, true, 2, 1, null) 
        {
           _Cooling = 3500,
           _Reversed = true,
           _Sparks = 1,
           _SparkHeight = 3
        };

        public static LEDEffect MarqueeEffect => new PaletteEffect(Palette.Rainbow)
        {
            _Density = Palette.SmoothRainbow.OriginalSize / 144.0 / 2.5 * Site.PIXELS_PER_METER144,
            _EveryNthDot = 15,
            _DotSize = 3,
            _LEDColorPerSecond = 0,
            _LEDScrollSpeed = 8,
            _Brightness = 1.0,
            _Mirrored = true
        };
        public static LEDEffect MarqueeEffect2 => new PaletteEffect(Palette.Rainbow)
        {
            _Density = .5 * Site.PIXELS_PER_METER144,
            _EveryNthDot = 3,
            _DotSize = 1,
            _LEDColorPerSecond = 0,
            _LEDScrollSpeed = 0.5,
            _Brightness = 1.0,
            _Mirrored = true
        };
        public static LEDEffect ColorCycleTube => new PaletteEffect(Palette.Rainbow)
        {
            _Density = 0,
            _EveryNthDot = 1,
            _DotSize = 1,
            _LEDColorPerSecond = 3,
            _LEDScrollSpeed = 0,
        };

        public static LEDEffect WhitePointLights => new SimpleColorFillEffect(new CRGB(246, 200, 160));

        public static LEDEffect QuietBlueStars => new StarEffect<ColorStar>
        {
            Blend = true,
            NewStarProbability = 1.0,
            StarPreignitonTime = 2.5,
            StarIgnition = 0.0,
            StarHoldTime = 2.0,
            StarFadeTime = 1.0,
            StarSize = 1,
            MaxSpeed = 0,
            BaseColorSpeed = 0.1,
            ColorSpeed = 0.0,
            RandomStartColor = false,
            RandomStarColorSpeed = false,
        };

        public static LEDEffect QuietColorStars => new StarEffect<PaletteStar>
        {
            Blend = true,
            NewStarProbability = 1.00,
            StarPreignitonTime = 4.0,
            StarIgnition = 0.0,
            StarHoldTime = 1.0,
            StarFadeTime = 3.0,
            StarSize = 1,
            MaxSpeed = 0,
            ColorSpeed = 0,
            Palette = new Palette(CRGB.ChristmasLights),
            RampedColor = false

        };

        public static LEDEffect ClassicTwinkle => new StarEffect<PaletteStar>
        {
            Blend = false,
            NewStarProbability = 1.0,
            StarPreignitonTime = 0,
            StarIgnition = 0.0,
            StarHoldTime = 1.75,
            StarFadeTime = 0.0,
            StarSize = 1,
            MaxSpeed = 0,
            ColorSpeed = 0,
            Palette = new Palette(CRGB.ChristmasLights),
            RampedColor = false,
            RandomStartColor = true
        };

        public static LEDEffect FrostyBlueStars => new StarEffect<PaletteStar>
        {
            Blend = true,
            NewStarProbability = .5,
            StarPreignitonTime = 0.05,
            StarIgnition = 0.1,
            StarHoldTime = 3.0,
            StarFadeTime = 1.0,
            StarSize = 1,
            MaxSpeed = 0,
            ColorSpeed = 1,
            Palette = new Palette(CRGB.makeGradient(new CRGB(0, 0, 64), new CRGB(0, 64, 255)))
        };

        public static LEDEffect TwinkleBlueStars => new StarEffect<PaletteStar>
        {
            Blend = true,
            NewStarProbability = 5,
            StarPreignitonTime = 0.1,
            StarIgnition = 0.05,
            StarHoldTime = 2.0,
            StarFadeTime = 1.0,
            StarSize = 1,
            MaxSpeed = 5,
            ColorSpeed = 0,
            Palette = new Palette(CRGB.BlueStars)
        };

        public static LEDEffect SparseChristmasLights => new StarEffect<PaletteStar>
        {
            Blend = false,
            NewStarProbability = 0.20,
            StarPreignitonTime = 0.00,
            StarIgnition = 0.0,
            StarHoldTime = 5.0,
            StarFadeTime = 0.0,
            StarSize = 1,
            MaxSpeed = 2,
            ColorSpeed = .05,
            Palette = new Palette(CRGB.ChristmasLights)
        };

        public static LEDEffect SparseChristmasLights2 => new StarEffect<AlignedPaletteStar>
        {
            Blend = false,
            NewStarProbability = 0.20,
            StarPreignitonTime = 0.00,
            StarIgnition = 0.0,
            StarHoldTime = 3.0,
            StarFadeTime = 0.0,
            StarSize = 6,
            MaxSpeed = 0,
            ColorSpeed = 0,
            Alignment = 24,
            RampedColor = true,
            Palette = new Palette(CRGB.ChristmasLights)
        };

        public static LEDEffect TwinkleChristmasLights => new StarEffect<AlignedPaletteStar>
        {
            Blend = false,
            NewStarProbability = 2.0,
            StarPreignitonTime = 0.00,
            StarIgnition = 0.0,
            StarHoldTime = 0.5,
            StarFadeTime = 0.0,
            StarSize = 4,
            MaxSpeed = 0,
            ColorSpeed = 0,
            Alignment = 24,
            RampedColor = true,
            Palette = new Palette(CRGB.ChristmasLights)
        };
        public static LEDEffect ChristmasLights => new PaletteEffect(new Palette(CRGB.ChristmasLights) { Blend = false } )
        {
            _Density = 3,
            _EveryNthDot = 18,
            _DotSize = 2,
            _LEDColorPerSecond = 0,
            _LEDScrollSpeed = 30,
            _RampedColor = true,
        };
        public static LEDEffect VintageChristmasLights => new PaletteEffect(new Palette(CRGB.VintageChristmasLights))
        {
            _Density = 1 * Site.PIXELS_PER_METER144,
            _EveryNthDot = 5,
            _DotSize = 2,
            _LEDColorPerSecond = 0,
            _LEDScrollSpeed = 20,
            _RampedColor = true
        };

        public static LEDEffect FastChristmasLights => new PaletteEffect(new Palette(CRGB.ChristmasLights))
        {
            _Density = 1 * Site.PIXELS_PER_METER144,
            _EveryNthDot = 28,
            _DotSize = 10,
            _LEDColorPerSecond = 0,
            _LEDScrollSpeed = 80,
            _RampedColor = true
        };

        public static LEDEffect ChristmasLightsFast => new PaletteEffect(new Palette(CRGB.ChristmasLights))
        {
            _Density = 16 * Site.PIXELS_PER_METER144,
            _EveryNthDot = 32,
            _DotSize = 1,
            _LEDColorPerSecond = 0,
            _LEDScrollSpeed = 1,
            _RampedColor = false
        };


        public static LEDEffect LavaStars => new StarEffect<PaletteStar>
        {
            Blend = true,
            NewStarProbability = 5,
            StarPreignitonTime = 0.05,
            StarIgnition = 1.0,
            StarHoldTime = 1.0,
            StarFadeTime = 1.0,
            StarSize = 10,
            MaxSpeed = 50,
            Palette = new Palette(CRGB.HotStars)
        };

        public static LEDEffect RainbowMiniLites => new PaletteEffect(Palette.SmoothRainbow ) 
        {
            _Density = 1,
            _EveryNthDot = 10,
            _DotSize = 1,
            _LEDColorPerSecond = 20,
            _LEDScrollSpeed = 0
        };

        public static LEDEffect RainbowStrip => new PaletteEffect(Palette.SmoothRainbow)
        {
            _Density = .3,
            _EveryNthDot = 1,
            _DotSize = 1,
            _LEDColorPerSecond = 10,
            _LEDScrollSpeed = 0,
        };

        public static LEDEffect MiniLites => new PaletteEffect(new Palette(new CRGB[]
        {
            CRGB.Blue,
            CRGB.Cyan,
            CRGB.Green,
            CRGB.Blue,
            CRGB.Purple,
            CRGB.Pink,
            CRGB.Blue
        }))
        {
            _Density = .1 * Site.PIXELS_PER_METER144,
            _EveryNthDot = 14,
            _DotSize = 1,
            _LEDColorPerSecond = 3,
            _LEDScrollSpeed = 0,
        };

        public static LEDEffect RainbowColorLites => new PaletteEffect(Palette.Rainbow)
        {
            _Density = 0.15  * Site.PIXELS_PER_METER144,
            _EveryNthDot = 8,
            _DotSize = 3,
            _LEDColorPerSecond = 5,
            _LEDScrollSpeed = 0,
        };

        public static LEDEffect CupboardRainbowSweep => new PaletteEffect(Palette.Rainbow)
        {
            _Density = .5 / 16  * Site.PIXELS_PER_METER144,
            _EveryNthDot = 10,
            _DotSize = 10,
            _LEDColorPerSecond = 1,
            _LEDScrollSpeed = 0,
        };

        public static LEDEffect ColorFadeMiniLites => new PaletteEffect(Palette.Rainbow)
        {
            _Density = 0,
            _EveryNthDot = 14,
            _DotSize = 1,
            _LEDColorPerSecond = 1,
            _LEDScrollSpeed = 0,
        };

        public static LEDEffect RidersEffect => new PaletteEffect(new Palette(CRGB.Football_Regina))
        {
            _Density = 1 * Site.PIXELS_PER_METER144,
            _EveryNthDot = 2,
            _DotSize = 1,
            _LEDColorPerSecond = 2,
            _LEDScrollSpeed = 0,
        };

        public static LEDEffect RidersEffect2 => new PaletteEffect(new Palette(CRGB.Football_Regina2))
        {
            _Density = 1 * Site.PIXELS_PER_METER144,
            _EveryNthDot = 2,
            _DotSize = 1,
            _LEDColorPerSecond = 2,
            _LEDScrollSpeed = 0,
        };

        public static LEDEffect Football_Effect_Seattle => new PaletteEffect(new Palette(CRGB.Football_Seattle))
        {
            _Density = 16 * Site.PIXELS_PER_METER144,
            _EveryNthDot = 1,
            _DotSize = 1,
            _LEDColorPerSecond = 5,
            _LEDScrollSpeed = 0,
        };

        public static LEDEffect Football_Effect_SeattleA => new PaletteEffect(new Palette(CRGB.Football_Seattle))
        {
            _Density = 1 / CRGB.Football_Seattle.Length *  Site.PIXELS_PER_METER144,
            _EveryNthDot = 1,
            _DotSize = 1,
            _LEDColorPerSecond = 2,
            _LEDScrollSpeed = 0,
        };

        public static LEDEffect Football_Effect_Seattle2 => new PaletteEffect(new Palette(CRGB.Football_Seattle))
        {
            _Density = 8 * Site.PIXELS_PER_METER144,
            _EveryNthDot = 10,
            _DotSize = 5,
            _LEDColorPerSecond = 0,
            _LEDScrollSpeed = 20,
        };

        public static LEDEffect C9 => new PaletteEffect(new Palette(CRGB.VintageChristmasLights))
        {
            _Density = 1 * Site.PIXELS_PER_METER144,
            _EveryNthDot = 16,
            _DotSize = 1,
            _LEDColorPerSecond = 0,
            _LEDScrollSpeed = 0,
        };

        public static LEDEffect SeawawksTwinkleStarEffect => new StarEffect<PaletteStar>
        {
            Palette = new Palette(CRGB.Football_Seattle),
            Blend = true,
            NewStarProbability = 3,
            StarPreignitonTime = 0.05,
            StarIgnition = .5,
            StarHoldTime = 1.0,
            StarFadeTime = .5,
            StarSize = 2,
            MaxSpeed = 0,
            ColorSpeed = 0,
            RandomStartColor = false,
            BaseColorSpeed = 0.25,
            RandomStarColorSpeed = false
        };

        public static LEDEffect RainbowTwinkleStarEffect => new StarEffect<PaletteStar>
        {
            Palette = Palette.Rainbow,
            Blend = true,
            NewStarProbability = 3,
            StarPreignitonTime = 0.05,
            StarIgnition = .5,
            StarHoldTime = .0,
            StarFadeTime = 1.5,
            StarSize = 1,
            MaxSpeed = 0,
            ColorSpeed = 10,
            RandomStartColor = false,
            BaseColorSpeed = 0.005,
            RandomStarColorSpeed = false
        };

        public static LEDEffect ChristmasTwinkleStarEffect
        {
            get
            {
                return new StarEffect<PaletteStar>
                {
                    Palette = new Palette(CRGB.ChristmasLights),
                    Blend = true,
                    NewStarProbability = 20,
                    StarPreignitonTime = 0.05,
                    StarIgnition = .05,
                    StarHoldTime = 0.5,
                    StarFadeTime = .25,
                    StarSize = 1,
                    MaxSpeed = 2,
                    ColorSpeed = 0,
                    RandomStartColor = true,
                    BaseColorSpeed = 0.0,
                    RandomStarColorSpeed = false
                };
            }
        }

        public static LEDEffect OneDirectionStars
        {
            get
            {
                return new StarEffect<PaletteStar>
                {
                    Palette = new Palette(CRGB.Rainbow),
                    Blend = false,
                    NewStarProbability = 0.15,
                    StarPreignitonTime = 0.0,
                    StarIgnition = 0,
                    StarHoldTime = 2.0,
                    StarFadeTime = 1.0,
                    StarSize = 1,
                    MaxSpeed = 50,
                    ColorSpeed = 0,
                    Direction = 1,
                    RandomStartColor = true,
                    BaseColorSpeed = 0.0,
                    RandomStarColorSpeed = false
                };
            }
        }

        public static LEDEffect BasicColorTwinkleStarEffect
        {
            get
            {
                return new StarEffect<ColorStar>
                {
                    Blend = true,
                    NewStarProbability = 1,
                    StarPreignitonTime = 0.05,
                    StarIgnition = .5,
                    StarHoldTime = 5.0,
                    StarFadeTime = .5,
                    StarSize = 1,
                    MaxSpeed = 2,
                    ColorSpeed = -2,
                    RandomStartColor = false,
                    BaseColorSpeed = 0.5,
                    RandomStarColorSpeed = false
                };
            }
        }

        public static LEDEffect SubtleColorTwinkleStarEffect
        {
            get
            {
                return new StarEffect<ColorStar>
                {
                    Blend = true,
                    NewStarProbability = 3,
                    StarPreignitonTime = 0.5,
                    StarIgnition = 0,
                    StarHoldTime = 1.0,
                    StarFadeTime = .5,
                    StarSize = 1,
                    MaxSpeed = 2,
                    ColorSpeed = -2,
                    RandomStartColor = false,
                    BaseColorSpeed = 0.2,
                    RandomStarColorSpeed = false
                };
            }
        }

        public static LEDEffect ToyFireTruck
        {
            get
            {
                return new StarEffect<PaletteStar>
                {
                    Palette = new Palette(new CRGB[] { CRGB.Red, CRGB.Red }),
                    Blend = true,
                    NewStarProbability = 25,
                    StarPreignitonTime = 0.05,
                    StarIgnition = .5,
                    StarHoldTime = 0.0,
                    StarFadeTime = .5,
                    StarSize = 1,
                    MaxSpeed = 5,
                    ColorSpeed = -2,
                    RandomStartColor = false,
                    BaseColorSpeed = 50,
                    RandomStarColorSpeed = false,
                    Direction = -1
                };
            }
        }

        public static LEDEffect CharlieBrownTree
        {
            get
            {
                return new StarEffect<ColorStar>
                {
                    Blend = true,
                    NewStarProbability = 5, // 25,
                    StarPreignitonTime = 0.05,
                    StarIgnition = .5,
                    StarHoldTime = 0.0,
                    StarFadeTime = .5,
                    StarSize = 1,
                    MaxSpeed = 20,
                    ColorSpeed = -2,
                    RandomStartColor = false,
                    BaseColorSpeed = 5,
                    RandomStarColorSpeed = false,
                    Direction = 1
                };
            }
        }

        public static LEDEffect Mirror
        {
            get
            {
                return new StarEffect<ColorStar>
                {
                    Blend = true,
                    NewStarProbability = 25,
                    StarPreignitonTime = 0.05,
                    StarIgnition = .5,
                    StarHoldTime = 0.0,
                    StarFadeTime = .5,
                    StarSize = 1,
                    MaxSpeed = 50,
                    ColorSpeed = -2,
                    RandomStartColor = false,
                    BaseColorSpeed = 5,
                    RandomStarColorSpeed = false,
                    Direction = 1
                };
            }
        }

        public static LEDEffect RainbowMarquee => new PaletteEffect(new Palette(CRGB.Rainbow))
        {
            _Density = .05,
            _EveryNthDot = 12,
            _DotSize = 3,
            _LEDColorPerSecond = -10,
            _LEDScrollSpeed = 10,
            _Mirrored = true,

        };

        public static LEDEffect ColorTunnel => new PaletteEffect(new Palette(CRGB.Rainbow))
        {
            _Density = .36 * Site.PIXELS_PER_METER144,
            _EveryNthDot = 3,
            _DotSize = 1,
            _LEDColorPerSecond = 1,
            _LEDScrollSpeed = 0,
        };

        public static LEDEffect BigMiniLites => new PaletteEffect(new Palette(CRGB.Rainbow))
        {
            _Density = 4.0 * Site.PIXELS_PER_METER144,
            _LEDColorPerSecond = 1,
            _LEDScrollSpeed = 0,
        };

        public static LEDEffect Mirror3
        {
            get
            {
                return new StarEffect<ColorStar>
                {
                    Blend = true,
                    NewStarProbability = 15, // 25,
                    StarPreignitonTime = 0.05,
                    StarIgnition = .0,
                    StarHoldTime = 0.0,
                    StarFadeTime = .5,
                    StarSize = 1,
                    MaxSpeed = 0,
                    ColorSpeed = 0,
                    RandomStartColor = false,
                    BaseColorSpeed = 10,
                    RandomStarColorSpeed = false,
                };
            }
        }
    }

    // Cabana
    //
    // Location definitio for the lights on the eaves of the Cabana

    public class Cabana : Site
    { 
        const bool compressData = true;
        const int CABANA_START = 0;
        const int CABANA_1 = CABANA_START;
        const int CABANA_1_LENGTH = (5 * 144 - 1) + (3 * 144);
        const int CABANA_2 = CABANA_START + CABANA_1_LENGTH;
        const int CABANA_2_LENGTH = 5 * 144 + 55;
        const int CABANA_3 = CABANA_START + CABANA_2_LENGTH + CABANA_1_LENGTH;
        const int CABANA_3_LENGTH = 6 * 144 + 62;
        const int CABANA_4 = CABANA_START + CABANA_3_LENGTH + CABANA_2_LENGTH + CABANA_1_LENGTH;
        const int CABANA_4_LENGTH = 8 * 144 - 23;
        const int CABANA_LENGTH = CABANA_1_LENGTH + CABANA_2_LENGTH + CABANA_3_LENGTH + CABANA_4_LENGTH;
        
        public ScheduledEffect[] _GameDayLEDEffects =
        {
            new ScheduledEffect(ScheduledEffect.AllDays,  9, 22,  EffectsDatabase.Football_Effect_Seattle),
        };

        public Cabana() : base("Cabana", CABANA_LENGTH, 1, false)
        {
            LightStrips = new List<LightStrip>()
            {
                new LightStrip("192.168.8.36", "CBWEST1", compressData, CABANA_1_LENGTH, 1, CABANA_1, false)  { FramesPerBuffer = 312, BatchSize = 10 },
                new LightStrip("192.168.8.5", "CBEAST1", compressData, CABANA_2_LENGTH, 1, CABANA_2, true)    { FramesPerBuffer = 312, BatchSize = 10 },
                new LightStrip("192.168.8.37", "CBEAST2", compressData, CABANA_3_LENGTH, 1, CABANA_3, false)  { FramesPerBuffer = 312, BatchSize = 10 },
                new LightStrip("192.168.8.31", "CBEAST3", compressData, CABANA_4_LENGTH, 1, CABANA_4, false)  { FramesPerBuffer = 312, BatchSize = 10 },
            };

            LEDEffects = new List<ScheduledEffect>()
            {
                // Uncomment to test a single effect

                new ScheduledEffect(ScheduledEffect.AllDays,  22, 23, new SimpleColorFillEffect(CRGB.RandomSaturatedColor.fadeToBlackBy(0.50f), 2)),
                new ScheduledEffect(ScheduledEffect.AllDays,  23, 24, new SimpleColorFillEffect(CRGB.RandomSaturatedColor.fadeToBlackBy(0.60f), 2)),
                new ScheduledEffect(ScheduledEffect.AllDays,  0,  1,  new SimpleColorFillEffect(CRGB.RandomSaturatedColor.fadeToBlackBy(0.70f), 4)),
                new ScheduledEffect(ScheduledEffect.AllDays,  1,  2,  new SimpleColorFillEffect(CRGB.RandomSaturatedColor.fadeToBlackBy(0.80f), 4)),
                new ScheduledEffect(ScheduledEffect.AllDays,  2,  3,  new SimpleColorFillEffect(CRGB.RandomSaturatedColor.fadeToBlackBy(0.90f), 4)),
                new ScheduledEffect(ScheduledEffect.AllDays,  3,  4,  new SimpleColorFillEffect(CRGB.RandomSaturatedColor.fadeToBlackBy(0.80f), 4)),
                new ScheduledEffect(ScheduledEffect.AllDays,  4,  5,  new SimpleColorFillEffect(CRGB.RandomSaturatedColor.fadeToBlackBy(0.70f), 3)),
                new ScheduledEffect(ScheduledEffect.AllDays,  5,  6,  new SimpleColorFillEffect(CRGB.RandomSaturatedColor.fadeToBlackBy(0.60f), 2)),
                new ScheduledEffect(ScheduledEffect.AllDays,  6,  7,  new SimpleColorFillEffect(CRGB.RandomSaturatedColor.fadeToBlackBy(0.50f), 2)),
                new ScheduledEffect(ScheduledEffect.AllDays,  7,  8,  new SimpleColorFillEffect(CRGB.RandomSaturatedColor.fadeToBlackBy(0.40f), 2)),


                new ScheduledEffect(ScheduledEffect.AllDays,  8, 22, EffectsDatabase.SubtleColorTwinkleStarEffect ),

                new ScheduledEffect(ScheduledEffect.AllDays,  8, 22, EffectsDatabase.OneDirectionStars ),
                new ScheduledEffect(ScheduledEffect.AllDays,  8, 22, EffectsDatabase.ColorFadeMiniLites ),
                new ScheduledEffect(ScheduledEffect.AllDays,  8, 22, EffectsDatabase.ColorCycleTube ),
                new ScheduledEffect(ScheduledEffect.AllDays,  8, 22, EffectsDatabase.RainbowMiniLites ),
                new ScheduledEffect(ScheduledEffect.AllDays,  8, 22, EffectsDatabase.QuietColorStars),

                new ScheduledEffect(ScheduledEffect.AllDays,  8, 22, EffectsDatabase.QuietColorStars),
            };
        }
    };



    // Bench
    //
    // Location definition for the test rig on the workbench

    public class Bench : Site
    {
        const bool compressData = true;
        const int BENCH_START   = 0;
        const int BENCH_LENGTH = 8 * 144;

        public Bench() : base("Bench", BENCH_LENGTH, 1, false)
        {
            LightStrips = new List<LightStrip>()
            {
                new LightStrip("192.168.8.77", "BENCH", compressData, BENCH_LENGTH, 1, BENCH_START, true, 0, false) { FramesPerBuffer = 24, BatchSize = 1  }  // 216
            };
            LEDEffects = new List<ScheduledEffect>()
            {
                new ScheduledEffect(ScheduledEffect.AllDays,  0, 24, EffectsDatabase.ClassicTwinkle),
            };
        }
    };

    // ChristmasPresents - Front Door Pillar Left
    //
    // Location definition for the test rig on the workbench

    public class CeilingStrip : Site
    {
        const int START   = 0;
        const int LENGTH = 5*144 + 38;

        public CeilingStrip() : base("Ceiling Strip", LENGTH, 1, false)
        {
            LightStrips = new List<LightStrip>()
            {
                new LightStrip("192.168.8.206", "Ceiling A", true, LENGTH, 1, START, true, 0, false) { FramesPerBuffer = 500, BatchSize = 20, CompressData = true },  // 216
            };
            LEDEffects = new List<ScheduledEffect>()
            {
                new ScheduledEffect(ScheduledEffect.AllDays,  0, 24, new FireEffect(LENGTH, true) { _Cooling = 100, _speed = 2 }),
                new ScheduledEffect(ScheduledEffect.AllDays,  0, 24, EffectsDatabase.MarqueeEffect ),
                new ScheduledEffect(ScheduledEffect.AllDays,  0, 24, EffectsDatabase.SubtleColorTwinkleStarEffect ),
                new ScheduledEffect(ScheduledEffect.AllDays,  0, 24, EffectsDatabase.ClassicTwinkle ),
            };
       }
    };

    public class Banner : Site
    {
        const int START = 0;
        const int WIDTH = 64 * 8;
        const int HEIGHT = 32;

        public Banner() : base("Banner", WIDTH, HEIGHT, true)
        {
            LightStrips = new List<LightStrip>()
            {
                new LightStrip("192.168.1.98", "Banner", true, WIDTH, HEIGHT, START, true, 0, false) { FramesPerBuffer = 500, BatchSize = 25, CompressData = true },  // 216
            };
            LEDEffects = new List<ScheduledEffect>()
            {
                new ScheduledEffect(ScheduledEffect.AllDays,  0, 24, new TextBitmapEffect("Dave's Garage", CRGB.Black, CRGB.White)),
                //new ScheduledEffect(ScheduledEffect.AllDays,  0, 24, new VideoPlayerEffect(@"c:\AMD\coding.mp4")),
                //new ScheduledEffect(ScheduledEffect.AllDays,  0, 24, new VideoPlayerEffect(@"c:\AMD\space.mp4")),
                //new ScheduledEffect(ScheduledEffect.AllDays,  0, 24, new VideoPlayerEffect(@"c:\AMD\fire3.mp4")),
                //new ScheduledEffect(ScheduledEffect.AllDays,  0, 24, new VideoPlayerEffect(@"c:\AMD\greenbar.mp4")),
                //new ScheduledEffect(ScheduledEffect.AllDays,  0, 24, new VideoPlayerEffect(@"c:\AMD\sunsets.mp4")),
                //new ScheduledEffect(ScheduledEffect.AllDays,  0, 24, EffectsDatabase.OneDirectionStars ),
                //new ScheduledEffect(ScheduledEffect.AllDays,  0, 24, EffectsDatabase.ClassicTwinkle ),
            };
        }
    };

    public class Tree : Site
    {
        const bool compressData = true;
        const int TREE_START = 0;
        const int TREE_LENGTH = 1*144;

        public Tree() : base("Tree", TREE_LENGTH, 1, false)
        {
            LightStrips = new List<LightStrip>()
            { 
                new LightStrip("192.168.8.77", "Bonsai Tree", compressData, TREE_LENGTH, 1, TREE_START, false, 0, false) { FramesPerBuffer = 312, BatchSize = 10  },
            };
            LEDEffects = new List<ScheduledEffect>()
            {
                new ScheduledEffect(ScheduledEffect.AllDays,  6, 24,
                        new PaletteEffect( new Palette(new CRGB [] { CRGB.Red, CRGB.Green, CRGB.Blue }))
                            { _LEDColorPerSecond = 20,
                            _LEDScrollSpeed = 3,
                            _EveryNthDot = 4,
                            _DotSize = 1,
                            _Mirrored = false,
                            _Brightness = 1.0,
                            _Density = 8}),

                new ScheduledEffect(ScheduledEffect.AllDays,  6, 24,
                        new PaletteEffect( new Palette(new CRGB [] { CRGB.Red, CRGB.Green, CRGB.Blue }))
                            { _LEDColorPerSecond = 20,
                            _LEDScrollSpeed = 0,
                            _EveryNthDot = TREE_LENGTH,
                            _DotSize = TREE_LENGTH,
                            _Mirrored = false,
                            _Brightness = 1.0,
                            _Density = 0}),


                new ScheduledEffect(ScheduledEffect.AllDays,  6, 24, new SimpleColorFillEffect(CRGB.Green, 1)),

                new ScheduledEffect(ScheduledEffect.AllDays,  6, 24,
                        new PaletteEffect( new Palette(new CRGB [] { CRGB.Green }))
                            { _LEDColorPerSecond = 20,
                            _LEDScrollSpeed = 3,
                            _EveryNthDot = 4,
                            _DotSize = 1,
                            _Mirrored = false,
                            _Brightness = 1.0,
                            _Density = 0}),

                    new ScheduledEffect(ScheduledEffect.AllDays,  6, 24,
                        new PaletteEffect( new Palette(new CRGB [] { CRGB.Red, CRGB.Green, CRGB.Blue }))
                            { _LEDColorPerSecond = 20,
                            _LEDScrollSpeed = 3,
                            _EveryNthDot = 4,
                            _DotSize = 1,
                            _Mirrored = false,
                            _Brightness = 1.0,
                            _Density = 8}),
            };
        }
    };

    public class TV : Site
    {
        const bool compressData = true;
        const int TV_START = 0;
        const int TV_LENGTH = 144 * 5;

        public TV() : base("TV", TV_LENGTH, 1, false)
        {
            LightStrips = new List<LightStrip>()
            {
                new LightStrip("192.168.8.26", "TV",        compressData, TV_LENGTH,         1, TV_START, false) { FramesPerBuffer = 500, BatchSize = 10  }
            };
            LEDEffects = new List<ScheduledEffect>()
            {
                new ScheduledEffect(ScheduledEffect.AllDays, 0, 24, new FireEffect(4 * 144, true) { _Cooling = 60 } )
            };
        }
    }

    // ShopCupboards
    //
    // Location definition for the up-lights on top of the shop cupboards

    public class ShopCupboards : Site
    { 
        const bool compressData = true;

        const int CUPBOARD_START = 0;
        const int CUPBOARD_1_START = CUPBOARD_START;
        const int CUPBOARD_1_LENGTH = 300 + 200;
        const int CUPBOARD_2_START = CUPBOARD_1_START + CUPBOARD_1_LENGTH;
        const int CUPBOARD_2_LENGTH = 300 + 300;                                   // 90 cut from one 
        const int CUPBOARD_3_START = CUPBOARD_2_START + CUPBOARD_2_LENGTH;
        const int CUPBOARD_3_LENGTH = 144;
        const int CUPBOARD_4_START = CUPBOARD_2_START + CUPBOARD_2_LENGTH + CUPBOARD_3_LENGTH;
        const int CUPBOARD_4_LENGTH = 144;          // Actuall 82, but 
        const int CUPBOARD_LENGTH = CUPBOARD_1_LENGTH + CUPBOARD_2_LENGTH + CUPBOARD_3_LENGTH + CUPBOARD_4_LENGTH;

        public ShopCupboards() : base("Shop Cupboards", CUPBOARD_LENGTH, 1, false)
        {
            LightStrips = new List<LightStrip>()
            {
                new LightStrip("192.168.8.12", "CUPBOARD1", compressData, CUPBOARD_1_LENGTH, 1, CUPBOARD_1_START, false) { FramesPerBuffer = 500, BatchSize = 10 },
                new LightStrip("192.168.8.29", "CUPBOARD2", compressData, CUPBOARD_2_LENGTH, 1, CUPBOARD_2_START, false) { FramesPerBuffer = 500, BatchSize = 10 },
                new LightStrip("192.168.8.30", "CUPBOARD3", compressData, CUPBOARD_3_LENGTH, 1, CUPBOARD_3_START, false) { FramesPerBuffer = 500, BatchSize = 10 },  // WHOOPS
                new LightStrip("192.168.8.15", "CUPBOARD4", compressData, CUPBOARD_4_LENGTH, 1, CUPBOARD_4_START, false) { FramesPerBuffer = 500, BatchSize = 10 },
            };

            LEDEffects = new List<ScheduledEffect>()
            {
                new ScheduledEffect(ScheduledEffect.AllDays, 0, 24, new PaletteEffect(Palette.SmoothRainbow)
                {
                    _EveryNthDot = 1,
                    _DotSize = 1,
                    _Density = 0.025/32 * PIXELS_PER_METER144,
                    _LEDColorPerSecond = 3
                })
            };
        }
    };

    // ShopSouthWindows
    //
    // Location definition for the lights int the 3-window south shop bay window

    
    public class ShopSouthWindows1 : Site
    {
        const bool compressData = true;

        const int WINDOW_START = 0;
        const int WINDOW_1_START = 0;
        const int WINDOW_1_LENGTH = 100;

        const int WINDOW_LENGTH = WINDOW_1_LENGTH;

        public ShopSouthWindows1() : base("Shop South Windows 1", WINDOW_LENGTH, 1, false)
        {
            LightStrips = new List<LightStrip>()
            {
                new LightStrip("192.168.8.8", "WINDOW1", compressData, WINDOW_1_LENGTH, 1, WINDOW_1_START, false) { FramesPerBuffer = 21, BatchSize = 1 } ,
            };

            LEDEffects = new List<ScheduledEffect>()
            {
                new ScheduledEffect(ScheduledEffect.AllDays, 0, 24, new SimpleColorFillEffect(new CRGB(255, 112, 0), 1)),
            };
        }
    }

    public class ShopSouthWindows2 : Site
    {
        const bool compressData = true;

        const int WINDOW_START = 0;
        const int WINDOW_1_START = 0;
        const int WINDOW_1_LENGTH = 100;

        const int WINDOW_LENGTH = WINDOW_1_LENGTH;

        public ShopSouthWindows2() : base("Shop South Windows 2", WINDOW_LENGTH, 1, false)
        {
            LightStrips = new List<LightStrip>()
            {
                new LightStrip("192.168.8.9", "WINDOW2", compressData, WINDOW_1_LENGTH, 1, WINDOW_1_START, false) { FramesPerBuffer = 21, BatchSize = 1 } ,
            };
            LEDEffects = new List<ScheduledEffect>()
            {
                new ScheduledEffect(ScheduledEffect.AllDays, 0, 24, new SimpleColorFillEffect(CRGB.Blue, 1)),
            };
        }
    }

    public class ShopSouthWindows3 : Site
    {
        const bool compressData = true;

        const int WINDOW_START = 0;
        const int WINDOW_1_START = 0;
        const int WINDOW_1_LENGTH = 100;

        const int WINDOW_LENGTH = WINDOW_1_LENGTH;

        public ShopSouthWindows3() : base("Shop South Windows 3", WINDOW_LENGTH, 1, false)
        {
            LightStrips = new List<LightStrip>()
            {
                new LightStrip("192.168.8.10", "WINDOW3", compressData, WINDOW_1_LENGTH, 1, WINDOW_1_START, false) { FramesPerBuffer = 21, BatchSize = 1 } ,
            };
            LEDEffects = new List<ScheduledEffect>()
            {
                new ScheduledEffect(ScheduledEffect.AllDays, 0, 24, new SimpleColorFillEffect(CRGB.Green, 1)),
            };
        }
    }
}
