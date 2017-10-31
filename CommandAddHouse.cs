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
    public class CommandAddHouse : IRocketCommand
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
                        "addhouse"
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
                return "addhouse";
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
            if(command.Length < 1)
            {
                UnturnedChat.Say(player, "Invalid syntax: /addhouse <price>", Color.red);
                return;
            }
            decimal cost = 0;
            bool IsNumber = decimal.TryParse(command[0], out cost);
            var house = Plugin.Instance.getHouseFromObjects(player.Position);
            if (!IsNumber)
            {
                UnturnedChat.Say(player, command[0] + " is not a number!", Color.red);
                return;
            }
            if(house == null)
            {
                UnturnedChat.Say(player, "Couden't manage to find a level object where you are standing!", Color.red);
                return;
            }
            var incase = (Plugin.Instance.Configuration.Instance.Houses.Find(c => (c.id == house.asset.id)));
            if(incase != null)
            {
                incase.cost = cost;
            }
            else
            {
                
                Plugin.Instance.Configuration.Instance.Houses.Add(new HouseItem(house.asset.id, cost));
            }
            Plugin.Instance.Configuration.Save();
            UnturnedChat.Say(player, "You succesfully added this house to the houselist!", Color.red);
        }
    }
}