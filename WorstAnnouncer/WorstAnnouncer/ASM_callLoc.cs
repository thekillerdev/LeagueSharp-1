using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace WorstAnnouncer
{
    class ASM_callLoc
    {
        private byte[] esp = { (byte)103, (byte)100, (byte)97, (byte)104, (byte)101, (byte)98, (byte)105, (byte)102, (byte)99 }; // ; stack pointer
        private string[] dword = { "Top SS", "Mid SS", "Bot SS", "Top Incoming", "Mid Incoming", "Bot Incoming", "RE", "/all GL HF", "/all GG WP" }; // ; 32b, int 2=>4 (EBX?)
        private int _max_ecx = 9; // ; stdout
        private float dq = 0; // ; declare quad word.
        private string[] eax = { "WorstPing", "1.0.3.0" }; // ; 16b, int 2<=4 (EBX?)

        public ASM_callLoc()
        {
            //                              |
            // esp[?], dword[?] ; explained V
            /*
            esp[0] = (byte)103; // ; "ss top"
            esp[1] = (byte)100; // ; "ss mid"
            esp[2] = (byte)97; // ; "ss bot"
            esp[3] = (byte)104; // ; "inc top"
            esp[4] = (byte)101; // ; "inc mid"
            esp[5] = (byte)98; // ; "inc bot"
            esp[6] = (byte)105; // ; "RE"
            esp[7] = (byte)102; // ; "GL HF"
            esp[8] = (byte)99; // ; "GG WP"
            
            dword[0] = "Top SS"; // ; "ss top"
            dword[1] = "Mid SS"; // ; "ss mid"
            dword[2] = "Bot SS"; // ; "ss bot"
            dword[3] = "Top Incoming"; // ; "inc top"
            dword[4] = "Mid Incoming"; // ; "inc mid"
            dword[5] = "Bot Incoming"; // ; "inc bot"
            dword[6] = "RE"; // ; "RE"
            dword[7] = "/all GL HF"; // ; "GL HF"
            dword[8] = "/all GG WP"; // ; "GG WP"
            */

            /* EXAMPLE HOW TO CHANGE: (remove '//' from the start of the line) */
            // dword[8] = "/all GG WP GETREKT";
            // esp[8] = (byte)107;

            Game.OnWndProc += loc_408D9A;
            CustomEvents.Game.OnGameLoad += loc_407E4B;
        }

        private void loc_408D9A(LeagueSharp.WndEventArgs args)
        {
            if(args.Msg == 0x100)
            { // 256 ^
                if (dq + 500 < Environment.TickCount)
                {
                    for (int eax = 0; eax < _max_ecx; ++eax)
                        if (args.WParam == esp[eax])
                            Game.Say(dword[eax]);
                    dq = Environment.TickCount;
                }
            }
        }

        private void loc_407E4B(EventArgs args)
        {
            Game.PrintChat("WorstAnnouncer {0}, by {1} loaded.", eax[0], eax[1]);
        }
    }
}
