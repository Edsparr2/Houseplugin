using Rocket.API.Collections;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using Steamworks;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using SDG.Unturned;
using fr34kyn01535.Uconomy;
using System.Text;
using Rocket.Unturned.Events;
using Rocket.API;
using Rocket.Unturned.Enumerations;
using Rocket.Unturned.Permissions;

namespace Edsparr.Houseplugin
{
    public class OwnerItem
    {
        public ulong owner;
        public DateTime boughtAt;
        public Vector3 house;

        public OwnerItem(ulong owner, DateTime boughtAt, Vector3 house)
        {
            this.owner = owner;
            this.boughtAt = boughtAt;
            this.house = house;
        }

        public OwnerItem() { }
    }

}







