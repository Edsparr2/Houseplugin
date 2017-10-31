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
    public class Plugin : RocketPlugin<Configuration>
    {

        public Vector3 pos;

        public static string version = "1.0";
        public static Plugin Instance;
        private DateTime n = DateTime.Now;

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList(){
                {"nothing","No transeltions right now if requested they might get added."},
                };
            }
        }

        protected override void Load()
        {
            Plugin.Instance = this;
            U.Events.OnPlayerConnected += OnConnected;
        }

        private void OnConnected(UnturnedPlayer player)
        {
            if(Configuration.Instance.Data.Find(c => (c.Player == (ulong)player.CSteamID)) == null)
            {
                Configuration.Instance.Data.Add(new DisplaynameSaver.PlayerInfo((ulong)player.CSteamID, player.CharacterName));
            }
            else
            {
                Configuration.Instance.Data.Find(c => (c.Player == (ulong)player.CSteamID)).Displayname = player.CharacterName;
            }
            Configuration.Save();
        }

        public void FixedUpdate()
        {
            if (!Level.isLoaded) return;
            if(DateTime.Now > n.AddSeconds(1))
            {
                n = DateTime.Now;
                foreach (var item in Configuration.Instance.BoughtHouses)
                {
                    if (DateTime.Now > item.boughtAt.AddHours(Configuration.Instance.FeeTime))
                    {
                        if (getCost(getHouse(item.house)) > Uconomy.Instance.Database.GetBalance(item.owner.ToString())) { clearHouse(getHouse(item.house)); continue; }
                        Uconomy.Instance.Database.IncreaseBalance(item.owner.ToString(), -getCost(getHouse(item.house)));
                    }
                }
                List<BarricadeData> barricades = new List<BarricadeData>();
                List<StructureData> structures = new List<StructureData>();
                foreach(var region in BarricadeManager.regions)
                {
                    foreach(var data in region.barricades)
                    {
                        if (data.barricade.isDead) continue;
                        OwnerItem house = null;
                        bool IsInH = IsInHouse(getTransform(data.point), out house);
                        if (!IsInH || house != null && IsInH && data.group != house.steamGroup && data.owner != house.owner && data.group != 0)
                        {
                            barricades.Add(data);
                        }
                    }
                }

                foreach (var region in StructureManager.regions)
                {
                    foreach (var data in region.structures)
                    {
                        if (data.structure.isDead) continue;
                        OwnerItem house = null;
                        bool IsInH = IsInHouse(getTransform(data.point), out house);
                        if (!IsInH || house != null && data.group != house.steamGroup && data.owner != house.owner && data.group != 0)
                        {
                            structures.Add(data);
                        }
                    }
                }
                foreach(var s in structures)
                {
                    UnturnedPlayer placer = UnturnedPlayer.FromCSteamID((CSteamID)s.owner);
                    if (placer != null)
                    {
                        try
                        {
                            UnturnedChat.Say(placer, "You're not allowed to place structures outside your house!", Color.red);
                        }catch { }
                        placer.GiveItem(s.structure.id, 1);
                    }
                    var rocket = new RocketPlayer(s.owner.ToString());
                    if (!rocket.HasPermission("houseplugin.bypass"))
                        DeleteStructure(getTransform(s.point));
                }
                foreach(var b in barricades)
                {
                    UnturnedPlayer placer = UnturnedPlayer.FromCSteamID((CSteamID)b.owner);
                    if (placer != null)
                    {
                        try
                        {
                            UnturnedChat.Say(placer, "You're not allowed to place barricades outside your house!", Color.red);
                        }catch { }
                        placer.GiveItem(b.barricade.id, 1);
                    }
                    var rocket = new RocketPlayer(b.owner.ToString());
                    if(!rocket.HasPermission("houseplugin.bypass"))
                    DeleteBarriacade(getTransform(b.point));
                }
            }
        }
        public void removeHouse(Transform house)
        {
            var found = Configuration.Instance.BoughtHouses.Find(c => (c.house == house.position));
            if (found == null) return;
            clearHouse(getTransform(found.house));
            Configuration.Instance.BoughtHouses.Remove(Configuration.Instance.BoughtHouses.Find(c => (c.house == found.house)));
            Configuration.Save();
        }
        public bool IsInHouse(Transform house, out OwnerItem houseee)
        {
            houseee = null;
            if (house == null) return false;
            foreach (var item in Configuration.Instance.Houses)
            {
                var housee = getHouseLevelObject(house.position);
                if (housee != null && Configuration.Instance.BoughtHouses.Find(c => (c.house == getHouse(housee.transform.position).position)) != null) {    houseee = Configuration.Instance.BoughtHouses.Find(c => (c.house == getHouse(housee.transform.position).position)); return true; }
            }
            return false;
        }
        public bool buyhHouse(Transform house, ulong owner, ulong steamgroup, out decimal cost)
        {
            cost = 0;
            if (Configuration.Instance.BoughtHouses.Find(c => (c.house == house.position)) != null) return false;
            Configuration.Instance.BoughtHouses.Add(new OwnerItem(owner, DateTime.Now, house.position, steamgroup));
            cost = Configuration.Instance.Houses.Find(c => (c.id == getHouseLevelObject(house.position).asset.id)).cost;
            Configuration.Save();
            return true;
        }
        public bool clearHouse(Transform house)
        {
            var found = Configuration.Instance.BoughtHouses.Find(c => (c.house == house.position));
            if (found == null) return false;
            List<Transform> barricades = new List<Transform>();
            List<Transform> structures = new List<Transform>();
            foreach (var region in BarricadeManager.regions)
            {
                foreach(var data in region.barricades)
                {
                    if (!data.barricade.isDead && getHouse(found.house).GetComponent<Collider>().bounds.Contains(data.point))
                    {
                        barricades.Add(getTransform(data.point));
                    }
                }
            }

            foreach (var region in StructureManager.regions)
            {
                foreach (var data in region.structures)
                {
                    if (!data.structure.isDead && getHouse(found.house).GetComponent<Collider>().bounds.Contains(data.point))
                    {
                        structures.Add(getTransform(data.point));
                    }
                }
            }
            foreach(var s in structures)
            {
                DeleteStructure(s);
            }
            foreach(var b in barricades)
            {
                DeleteBarriacade(b);
            }
            return true;
        }
        public decimal getCost(Transform house)
        {
            var item = getHouseLevelObject(house.position);
            return Configuration.Instance.Houses.Find(c => (c.id == item.asset.id)).cost;
        }
        public Transform getHouse(Vector3 pos)
        {
            foreach (var ob in LevelObjects.objects)
            {
                foreach (var item in ob)
                {

                    if (item.transform != null && item.transform.GetComponent<Collider>() != null && item.transform.GetComponent<Collider>().bounds.Contains(pos) && Configuration.Instance.Houses.Find(c => (c.id == item.asset.id)) != null)
                    {
                        return item.transform;
                    }
                }
            }
            return null;
        }
        public LevelObject getHouseFromObjects(Vector3 pos)
        {
            foreach (var ob in LevelObjects.objects)
            {
                foreach (var item in ob)
                {

                    if (item.transform != null && item.transform.GetComponent<Collider>() != null && item.transform.GetComponent<Collider>().bounds.Contains(pos))
                    {
                        return item;
                    }
                }
            }
            return null;
        }
        public LevelObject getHouseLevelObject(Vector3 pos)
        {
            foreach (var ob in LevelObjects.objects)
            {
                foreach (var item in ob)
                {

                    if (item.transform != null && item.transform.GetComponent<Collider>() != null && item.transform.GetComponent<Collider>().bounds.Contains(pos) && Configuration.Instance.Houses.Find(c => (c.id == item.asset.id)) != null)
                    {
                        return item;
                    }
                }
            }
            return null;
        }
        public Transform getTransform(Vector3 vectorPos)
        {
            var hits = Physics.OverlapSphere(vectorPos, 0.1f);
            foreach (var hit in hits)
            {
                if (hit.transform.position == vectorPos)
                    return hit.transform;
            }
            return null;
        }

        protected override void Unload()
        {
        }

        public void DeleteBarriacade(Transform transform)
        {
            byte x;
            byte y;
            ushort index;
            ushort plant;
            BarricadeRegion region;

            if (BarricadeManager.tryGetInfo(transform, out x, out y, out plant, out index, out region))
            {
                region.barricades.RemoveAt((int)index);
                if ((int)plant == (int)ushort.MaxValue)
                    BarricadeManager.instance.channel.send("tellTakeBarricade", ESteamCall.ALL, x, y, BarricadeManager.BARRICADE_REGIONS, ESteamPacket.UPDATE_RELIABLE_BUFFER, (object)x, (object)y, (object)plant, (object)index);
                else
                {
                    BarricadeManager.instance.channel.send("tellTakeBarricade", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER, new object[4]
                    {
                (object) x,
                (object) y,
                (object) plant,
                (object) index
                    });
                }
            }
        }
        public void DeleteStructure(Transform transform)
        {
            byte x;
            byte y;
            StructureDrop index;
            ushort plant;
            StructureRegion region;

            if (StructureManager.tryGetInfo(transform, out x, out y, out plant, out region, out index))
            {
                region.structures.RemoveAt((int)plant);
                if ((int)plant == (int)ushort.MaxValue)
                    StructureManager.instance.channel.send("tellTakeStructure", ESteamCall.ALL, x, y, StructureManager.STRUCTURE_REGIONS, ESteamPacket.UPDATE_RELIABLE_BUFFER, (object)x, (object)y, (object)plant, (object)index);
                else
                {
                    StructureManager.instance.channel.send("tellTakeStructure", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER, new object[4]
                    {
                (object) x,
                (object) y,
                (object) plant,
                (object) index
                    });
                }
            }
        }




    }

}







