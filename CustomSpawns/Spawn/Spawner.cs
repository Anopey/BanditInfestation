using System;
using System.Collections.Generic;
using System.Linq;
//using CustomSpawns.MCMv3;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.CampaignSystem;
using Helpers;

namespace CustomSpawns.Spawn
{
    public static class Spawner
    {


        public static MobileParty SpawnParty(Settlement spawnedSettlement, Clan clan, PartyTemplateObject templateObject, Track.PartyTypeEnum partyType,  TextObject partyName = null)
        {
            try
            {

                if(templateObject == null)
                {
                    ErrorHandler.ShowPureErrorMessage(
                        "Party Template with ID " + templateObject.StringId + " possibly does not exist. It was tried to be assigned to "
                        + templateObject.StringId);
                    return null;
                }

                //get name and show message.
                TextObject textObject = partyName ?? clan.Name;
                ModDebug.ShowMessage("CustomSpawns: Spawning " + textObject.ToString() + " at " + spawnedSettlement.GatePosition + " in settlement " + spawnedSettlement.Name.ToString(), DebugMessageType.Spawn);

                //create.
                MobileParty mobileParty = CreatePartyInstance(spawnedSettlement, clan, templateObject, partyType, partyName);

                if(mobileParty == null)
                {
                    return null; //must have had some issue. or maybe it was just the wind.
                }

                mobileParty.InitializeMobileParty(ConstructTroopRoster(templateObject, mobileParty.Party),
                    new TroopRoster(mobileParty.Party), spawnedSettlement.GatePosition, 0);

                //initialize
                InitParty(mobileParty, textObject, clan, spawnedSettlement);

                return mobileParty;
            }
            catch (Exception e) {
                ErrorHandler.ShowPureErrorMessage("Possible invalid spawn data. Spawning of party terminated.");
                ErrorHandler.HandleException(e, "party spawning");
                return null;
            }

        }

        private static MobileParty CreatePartyInstance(Settlement spawnedSettlement, Clan clan, PartyTemplateObject templateObject,
            Track.PartyTypeEnum partyType, TextObject partyName = null)
        {
            if (clan.IsBanditFaction)
            {
                return BanditPartyComponent.CreateBanditParty(templateObject.StringId + "_" + 1, clan,
                    spawnedSettlement.Hideout, false);
            }
            
            return MobileParty.CreateParty(templateObject.StringId + "_" + 1);
        }

        private static void InitParty(MobileParty party, TextObject name, Clan faction, Settlement homeSettlement)
        {

            party.SetCustomName(name);
            if (faction.Leader == null)
            {
                party.Party.Owner = faction.Heroes.ToList().Count == 0? null : faction.Heroes.First();
            }
            else
            {
                if (faction.Leader.HomeSettlement == null)
                {
                    faction.UpdateHomeSettlement(homeSettlement);
                }

                party.Party.Owner = faction.Leader;
            }
            party.Party.Visuals.SetMapIconAsDirty();

            party.ActualClan = faction;
            party.HomeSettlement = homeSettlement;
            TaleWorldsCode.BanditsCampaignBehaviour.CreatePartyTrade(party);
            foreach (ItemObject itemObject in Items.All)
            {
                if (itemObject.IsFood)
                {
                    int num = TaleWorldsCode.BanditsCampaignBehaviour.IsLooterFaction(party.MapFaction) ? 8 : 16;
                    int num2 = MBRandom.RoundRandomized((float)party.MemberRoster.TotalManCount * (1f / (float)itemObject.Value) * (float)num * MBRandom.RandomFloat * MBRandom.RandomFloat * MBRandom.RandomFloat * MBRandom.RandomFloat);
                    if (num2 > 0)
                    {
                        party.ItemRoster.AddToCounts(itemObject, num2);
                    }
                }
            }
        }

        private static TroopRoster ConstructTroopRoster(PartyTemplateObject pt, PartyBase ownerParty, int troopNumberLimit = -1) //TODO implement troop number limit.
        {
            TroopRoster returned = new TroopRoster(ownerParty);
            float gameProcess = MiscHelper.GetGameProcess();
            float num = 0.25f + 0.75f * gameProcess;
            int num2 = MBRandom.RandomInt(2);
            float num3 = (num2 == 0) ? MBRandom.RandomFloat : (MBRandom.RandomFloat * MBRandom.RandomFloat * MBRandom.RandomFloat * 4f);
            float num4 = (num2 == 0) ? (num3 * 0.8f + 0.2f) : (1f + num3);
            float randomFloat = MBRandom.RandomFloat;
            float randomFloat2 = MBRandom.RandomFloat;
            float randomFloat3 = MBRandom.RandomFloat;
            for (int i = 0; i < pt.Stacks.Count; i++)
            {
                float f = (pt.Stacks.Count > 0) ? ((float)pt.Stacks[i].MinValue + num * num4 * randomFloat * (float)(pt.Stacks[i].MaxValue - pt.Stacks[i].MinValue)) : 0f;
                returned.AddToCounts(pt.Stacks[i].Character, MBRandom.RoundRandomized(f), false);
            }
            return returned;
        }

        public static void HandleAIChecks(MobileParty mb, Data.SpawnData data, Settlement spawnedSettlement) //TODO handle sub parties being reconstructed!
        {
            try
            {
                bool invalid = false;
                Dictionary<string, bool> aiRegistrations = new Dictionary<string, bool>();
                if (data.PatrolAroundSpawn)
                {
                    bool success = AI.AIManager.HourlyPatrolAroundSpawn.RegisterParty(mb, spawnedSettlement);
                    aiRegistrations.Add("Patrol around spawn behaviour: ", success);
                    invalid = invalid ? true : !success;
                }
                if (data.AttackClosestIfIdleForADay)
                {
                    bool success = AI.AIManager.AttackClosestIfIdleForADayBehaviour.RegisterParty(mb);
                    aiRegistrations.Add("Attack Closest Settlement If Idle for A Day Behaviour: ", success);
                    invalid = invalid ? true : !success;
                }
                if (data.PatrolAroundClosestLestInterruptedAndSwitch.isValidData)
                {
                    bool success = AI.AIManager.PatrolAroundClosestLestInterruptedAndSwitchBehaviour.RegisterParty(mb, 
                        new AI.PatrolAroundClosestLestInterruptedAndSwitchBehaviour.PatrolAroundClosestLestInterruptedAndSwitchBehaviourData(mb, data.PatrolAroundClosestLestInterruptedAndSwitch));
                    aiRegistrations.Add("Patrol Around Closest Lest Interrupted And Switch Behaviour: ", success);
                    invalid = invalid ? true : !success;
                }
                if (invalid && ConfigLoader.Instance.Config.IsDebugMode)
                {
                    ErrorHandler.ShowPureErrorMessage("Custom Spawns AI XML registration error has occured. The party being registered was: " + mb.StringId +
                        "\n Here is more info about the behaviours being registered: \n" + String.Join("\n", aiRegistrations.Keys));
                }
            }
            catch (Exception e)
            {
                ErrorHandler.HandleException(e);
            }
        }

        public static Settlement GetSpawnSettlement(Data.SpawnData data, Random rand = null, List<Settlement> exceptions = null)
        {
            if(rand == null)
                rand = new Random();

            if (exceptions == null)
                exceptions = new List<Settlement>();

            Clan spawnClan = data.SpawnClan;
            //deal with override of spawn clan.
            if (data.OverridenSpawnClan.Count != 0)
            {
                spawnClan = data.OverridenSpawnClan[rand.Next(0, data.OverridenSpawnClan.Count)];
            }
            //check for one hideout
            Settlement firstHideout = null;
            if (ConfigLoader.Instance.Config.SpawnAtOneHideout)
            {
                foreach (Settlement s in Settlement.All)
                {
                    if (s.IsHideout())
                    {
                        firstHideout = s;
                        break;
                    }
                }
            }

            //deal with town spawn
            Settlement spawnOverride = null;
            if (data.OverridenSpawnSettlements.Count != 0)
            {
                spawnOverride = CampaignUtils.PickRandomSettlementAmong(new List<Settlement>(data.OverridenSpawnSettlements.Where(s => !exceptions.Contains(s))), 
                    data.TrySpawnAtList, rand);
            }

            if (spawnOverride == null && data.OverridenSpawnCultures.Count != 0)
            {
                //spawn at overriden spawn instead!
                spawnOverride = CampaignUtils.PickRandomSettlementOfCulture(data.OverridenSpawnCultures, data.TrySpawnAtList, exceptions);
            }

            if (spawnOverride != null)
                return spawnOverride;

            //get settlement
            Settlement spawnSettlement = ConfigLoader.Instance.Config.SpawnAtOneHideout ? firstHideout : (data.TrySpawnAtList.Count == 0 ? CampaignUtils.GetPreferableHideout(spawnClan) : null);
            return spawnSettlement;
        }

        public static Settlement GetSpawnSettlement(Data.SpawnData data, Func<Settlement , bool> exceptionPredicate, Random rand = null)
        {
            if (rand == null)
                rand = new Random();


            Clan spawnClan = data.SpawnClan;
            //deal with override of spawn clan.
            if (data.OverridenSpawnClan.Count != 0)
            {
                spawnClan = data.OverridenSpawnClan[rand.Next(0, data.OverridenSpawnClan.Count)];
            }
            //check for one hideout
            Settlement firstHideout = null;
            if (ConfigLoader.Instance.Config.SpawnAtOneHideout)
            {
                foreach (Settlement s in Settlement.All)
                {
                    if (s.IsHideout())
                    {
                        firstHideout = s;
                        break;
                    }
                }
            }

            //deal with town spawn
            Settlement spawnOverride = null;
            if (data.OverridenSpawnSettlements.Count != 0)
            {
                spawnOverride = CampaignUtils.PickRandomSettlementAmong(new List<Settlement>(data.OverridenSpawnSettlements.Where(s => !exceptionPredicate(s))),
                    data.TrySpawnAtList, rand);
            }

            if (spawnOverride == null && data.OverridenSpawnCultures.Count != 0)
            {
                //spawn at overriden spawn instead!
                spawnOverride = CampaignUtils.PickRandomSettlementOfCulture(data.OverridenSpawnCultures, exceptionPredicate, data.TrySpawnAtList);
            }

            if (spawnOverride != null)
                return spawnOverride;

            //get settlement
            Settlement spawnSettlement = ConfigLoader.Instance.Config.SpawnAtOneHideout ? firstHideout : (data.TrySpawnAtList.Count == 0 ? CampaignUtils.GetPreferableHideout(spawnClan) : null);
            return spawnSettlement;
        }
    }
}
