using fr34kyn01535.Uconomy;
using Rocket.API;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Reflection;
using Rocket.Unturned.Permissions;

namespace Edsparr.Houseplugin
{
    public class CommandCheckHouse : IRocketCommand
    {
        #region Declarations

        public bool AllowFromConsole
        {
            get
            {
                return false;
            }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>()
                    {
                        "checkhouse"
                    };
            }
        }

        public AllowedCaller AllowedCaller
        {
            get
            {
                return AllowedCaller.Player;
            }
        }

        public bool RunFromConsole
        {
            get
            {
                return false;
            }
        }

        public string Name
        {
            get
            {
                return "checkhouse";
            }
        }

        public string Syntax
        {
            get
            {
                return "apartment (create/select)";
            }
        }

        public string Help
        {
            get

            {
                return "You assign the sell barricade.";
            }
        }

        public List<string> Aliases
        {
            get
            {
                return new List<string>();
            }
        }

        #endregion

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            var bfound = Plugin.Instance.Configuration.Instance.BoughtHouses.Find(c => (c.house == Plugin.Instance.getHouse(player.Position).position));
            var afound = Plugin.Instance.Configuration.Instance.Houses.Find(c => (c.id == Plugin.Instance.getHouseLevelObject(player.Position).id));
            string final = "";
            if(bfound == null && afound != null)
            {
                final = "This house is not calimed and cost " + afound.cost + "!";
            } else if(bfound != null)
            {
                final = "This house has been claimed by " + Edsparr.DisplaynameSaver.Plugin.Instance.GetDisplayName((CSteamID)bfound.owner) + " and payed the rent at " + bfound.boughtAt + " of " + afound.cost + "!";
            }
            else
            {
                UnturnedChat.Say(player, "House not found!", Color.red);
                return;
            }
            UnturnedChat.Say(player, final, Color.yellow);
            }
    }
}