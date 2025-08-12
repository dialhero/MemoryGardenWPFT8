using System.Media;

namespace MemoryGardenWPF.Services
{
    public static class SoundService
    {
        public static bool Enabled { get; set; } = false;
        public static void Flip()  { if (Enabled) SystemSounds.Asterisk.Play(); }
        public static void Match() { if (Enabled) SystemSounds.Exclamation.Play(); }
        public static void Miss()  { if (Enabled) SystemSounds.Hand.Play(); }
        public static void Tick()  { if (Enabled) SystemSounds.Beep.Play(); }
        public static void Win()   { if (Enabled) SystemSounds.Asterisk.Play(); }
    }
}
