using Rocket.API;

using System.Collections.Generic;

using System.Xml.Serialization;

using System;
using UnityEngine;

namespace Edsparr.Houseplugin
{

    public class Configuration : IRocketPluginConfiguration
    {
        public double FeeTime;
        public List<HouseItem> Houses = new List<HouseItem>();
        public List<OwnerItem> BoughtHouses = new List<OwnerItem>();
        public List<Edsparr.DisplaynameSaver.PlayerInfo> Data = new List<Edsparr.DisplaynameSaver.PlayerInfo>();
        public void LoadDefaults()
        {
            FeeTime = 24 * 7;
            Houses.Add(new HouseItem(1, 10000));
            Data = null;

        }
    }
}