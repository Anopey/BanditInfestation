using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.TwoDimension;

namespace CustomSpawns.Economics
{
    public static class PartyEconomicUtils
    {

        public static void PartyReplenishFood(MobileParty mobileParty)
        {
            if (mobileParty.IsPartyTradeActive && mobileParty.Food < Mathf.Abs(mobileParty.FoodChange * 2))
            {
                mobileParty.PartyTradeGold = (int)((double)mobileParty.PartyTradeGold * 0.95 + (double)(50f * (float)mobileParty.Party.MemberRoster.TotalManCount * 0.05f));
                if (mobileParty.Food < 0 || (MBRandom.RandomFloat < 0.1f && mobileParty.MapEvent != null))
                {
                    foreach (ItemObject itemObject in Items.All)
                    {
                        if (itemObject.IsFood)
                        {
                            int num = 12;
                            int num2 = MBRandom.RoundRandomized((float)mobileParty.MemberRoster.TotalManCount * (1f / (float)itemObject.Value) * (float)num * MBRandom.RandomFloat * MBRandom.RandomFloat * MBRandom.RandomFloat * MBRandom.RandomFloat);
                            if (num2 > 0)
                            {
                                mobileParty.ItemRoster.AddToCounts(itemObject, num2);
                            }
                        }
                    }
                }
            }
        }

    }
}
