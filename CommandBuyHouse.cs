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
    public class CommandBuyHouse : IRocketCommand
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
                        "buyhouse"
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
                return "buyhouse";
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
            if(Plugin.Instance.Configuration.Instance.BoughtHouses.Find(c => (c.house == house.position)) != null)
            {
                UnturnedChat.Say(player, "This house is already bought!", Color.red);
                return;
            }
            if(Uconomy.Instance.Database.GetBalance(player.CSteamID.ToString()) < Plugin.Instance.getCost(Plugin.Instance.getHouse(player.Position)))
            {
                UnturnedChat.Say(player, "You can't afford this house! It cost " + Plugin.Instance.getCost(Plugin.Instance.getHouse(player.Position)) + "!", Color.red); return;
            }
            decimal cost = 0;
            Plugin.Instance.buyhHouse(Plugin.Instance.getHouse(player.Position), (ulong)player.CSteamID, out cost);
            UnturnedChat.Say(player, "You succesfully bought this house for " + cost + "!", Color.yellow);
            Uconomy.Instance.Database.IncreaseBalance(player.CSteamID.ToString(), -cost);
        }
    }
}