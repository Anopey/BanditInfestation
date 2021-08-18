using System;
using System.Collections.Generic;
using System.Linq;
using CustomSpawns.RewardSystem.Models;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace CustomSpawns.RewardSystem
{
    public class SpawnRewardBehavior : CampaignBehaviorBase
    {
        private const string PlayerPartyId = "player_party";

        public SpawnRewardBehavior() : base()
        {
            XmlRewardData.GetInstance();
        }
        
        public override void RegisterEvents()
        {
            CampaignEvents.OnPlayerBattleEndEvent.AddNonSerializedListener(this, new Action<MapEvent>(
                mapEvent =>
                {
                    if (mapEvent.GetLeaderParty(BattleSideEnum.Attacker).Id == PlayerPartyId &&
                        mapEvent.WinningSide == BattleSideEnum.Attacker)
                    {
                        CalculateReward(mapEvent.DefenderSide.Parties, mapEvent.AttackerSide.Parties.FirstOrDefault(p => p.Party.Id == PlayerPartyId));
                    }
                    else if (mapEvent.GetLeaderParty(BattleSideEnum.Defender).Id == PlayerPartyId &&
                             mapEvent.WinningSide == BattleSideEnum.Defender)
                    {
                        CalculateReward(mapEvent.AttackerSide.Parties, mapEvent.DefenderSide.Parties.FirstOrDefault(p => p.Party.Id == PlayerPartyId));
                    }
                })
            );
        }

        public override void SyncData(IDataStore dataStore){}

        private void CalculateReward(MBReadOnlyList<MapEventParty> defeatedParties, MapEventParty mapEventPlayerParty)
        {
            foreach (var party in defeatedParties)
            {
                var moneyAmount = 0;
                var renownAmount = 0;
                var influenceAmount = 0;
                var partyRewards = XmlRewardData.GetInstance().PartyRewards;
                var partyReward = partyRewards.FirstOrDefault(el => party.Party.Id.Contains(el.PartyId));
                if (partyReward != null)
                {
                    foreach (var reward in partyReward.Rewards)
                    {
                        switch (reward.Type)
                        {
                            case RewardType.Influence:
                                if (reward.RenownInfluenceMoneyAmount != null)
                                {
                                    influenceAmount = Convert.ToInt32(reward.RenownInfluenceMoneyAmount);
                                    mapEventPlayerParty.GainedInfluence += Convert.ToSingle(reward.RenownInfluenceMoneyAmount);
                                }
                                break;
                            case RewardType.Money:
                                if (reward.RenownInfluenceMoneyAmount != null)
                                {
                                    moneyAmount = Convert.ToInt32(reward.RenownInfluenceMoneyAmount);
                                    mapEventPlayerParty.PlunderedGold += Convert.ToInt32(reward.RenownInfluenceMoneyAmount);
                                }
                                break;
                            case RewardType.Item:
                                if (reward.ItemId != null)
                                {
                                    var itemToAdd = Items.All.FirstOrDefault(obj => obj.StringId == reward.ItemId);
                                    if (reward.Chance != null)
                                    {
                                        if (IsItemGiven(Convert.ToDecimal(reward.Chance)))
                                        {
                                            mapEventPlayerParty.RosterToReceiveLootItems.Add(new ItemRosterElement(itemToAdd, 1));
                                        }
                                    }
                                }
                                break;
                            case RewardType.Renown:
                                if (reward.RenownInfluenceMoneyAmount != null)
                                {
                                    renownAmount = Convert.ToInt32(reward.RenownInfluenceMoneyAmount);
                                    mapEventPlayerParty.GainedRenown += Convert.ToSingle(reward.RenownInfluenceMoneyAmount);
                                }
                                break;
                        }
                    }
                    InformationManager.DisplayMessage(
                        new InformationMessage(
                            $"{mapEventPlayerParty.Party.Leader.Name} defeated {party.Party.Name} gaining {moneyAmount} denars, {renownAmount} renown and {influenceAmount} influence", 
                            Colors.Green
                            )
                        );
                }
            }
        }

        private bool IsItemGiven(decimal chance)
        {
            var random = new Random();
            return random.Next(101) <= chance * 100;
        }
    }
}