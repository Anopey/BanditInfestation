using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.Localization;

namespace CustomSpawns.HarmonyPatches
{
    [HarmonyPatch(typeof(DefaultPartySpeedCalculatingModel), "CalculateFinalSpeed")]
    public class PartySpeedModelPatch
    {
        private static readonly TextObject _baseSpeedExplanationText = new TextObject("Custom Spawns Base Speed Modification");

        private static readonly TextObject _extraSpeedExplanationText = new TextObject("Custom Spawns Extra Speed Modification");

        static bool Prefix(MobileParty mobileParty, ref ExplainedNumber finalSpeed)
        {
            if (!Main.PartySpeedContext.IsPartySpeedBonusAllowedByUser())
            {
                return true;
            }

            string partyId = CampaignUtils.IsolateMobilePartyStringID(mobileParty); //TODO if this is non-trivial make it more efficient
            if (Main.PartySpeedContext.IsPartyEligibleForExtraSpeed(partyId))
            {
                float extraSpeed = Main.PartySpeedContext.GetSpeedWithExtraBonus(partyId);
                finalSpeed.Add(extraSpeed, _extraSpeedExplanationText);
            }
            else if (Main.PartySpeedContext.IsBasePartySpeedOverriden(partyId))
            {
                float overridenBaseSpeed = Main.PartySpeedContext.GetBaseSpeed(partyId);
                finalSpeed = new ExplainedNumber(overridenBaseSpeed);
                return true;
            }

            return true;
        }

        static void Postfix(MobileParty mobileParty, ref ExplainedNumber __result)
        {
            if (!Main.PartySpeedContext.IsPartySpeedBonusAllowedByUser())
            {
                return;
            }

            string partyId = CampaignUtils.IsolateMobilePartyStringID(mobileParty); //TODO if this is non-trivial make it more efficient

            if (Main.PartySpeedContext.IsPartyMinimumSpeedOverriden(partyId)) //minimum adjustment
                __result.LimitMin(Main.PartySpeedContext.GetMinimumSpeed(partyId));

            if (Main.PartySpeedContext.IsPartyMaximumSpeedOverriden(partyId))//maximum adjustment
                __result.LimitMax(Main.PartySpeedContext.GetMaximumSpeed(partyId));
        }
    }
}