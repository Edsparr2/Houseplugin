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
    public class CommandDisbandHouse : IRocketCommand
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
                        "disbandhouse"
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
                return "disbandhouse";
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
            Transform house = Plugin.Instance.getHouse(player.Position);
            if(house == null)
            {
                UnturnedChat.Say(player, "Couden't manage to find a house where you're at!", Color.red);
                return;
            }
            var found = (Plugin.Instance.Configuration.Instance.BoughtHouses.Find(c => (c.house == house.position)));
            if(found == null)
            {
                UnturnedChat.Say(player, "Noone has bought this house!", Color.red);
                return;
            }
            Plugin.Instance.removeHouse(Plugin.Instance.getHouse(found.house));
            UnturnedChat.Say(player, "You succesfully disbanded this players house!", Color.yellow);
            return;
        }
    }
}