using Rocket.API.Collections;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Edsparr.DisplaynameSaver
{
    public class Plugin
    {
        public static string version = "1.0";
        public static Plugin Instance = new Plugin();

        public string GetDisplayName(CSteamID id)
        {
            var result = Edsparr.Houseplugin.Plugin.Instance.Configuration.Instance.Data.Find(c => (c.Player == (ulong)id));
            if (result == null) return "USername not found";
            return result.Displayname;
        }






        /////////////////






    }

}

namespace Edsparr.DisplaynameSaver
{
    public class PlayerInfo
    {
        public ulong Player = 0;
        public string Displayname;

        public PlayerInfo(ulong Player, string Displayname)
        {
            this.Player = Player;
            this.Displayname = Displayname;

        }

        public PlayerInfo() { /* Either some default shit or nothing at all */ }

    }
}



