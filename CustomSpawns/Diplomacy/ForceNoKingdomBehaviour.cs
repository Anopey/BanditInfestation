using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.CampaignSystem.Actions;


namespace CustomSpawns.Diplomacy
{
    class ForceNoKingdomBehaviour : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail,Boolean>(this.DailyMakeSureNoKingdom));
        }

        private Data.DiplomacyDataManager dataManager;

        public override void SyncData(IDataStore dataStore)
        {

        }

        private void DailyMakeSureNoKingdom(Clan c, Kingdom k1, Kingdom k2, ChangeKingdomAction.ChangeKingdomActionDetail details, Boolean b)
        {
            if (c == null || DiplomacyUtils.GetHardCodedExceptionClans().Contains(c.StringId) || !c.IsClanTypeMercenary)
                return;
            try
            {
                if (dataManager == null)
                {
                    GetData();
                }
                if (dataManager.Data.ContainsKey(c.StringId))
                {
                    if(dataManager.Data[c.StringId].ForceNoKingdom && c.Kingdom != null)
                    {
                        ChangeKingdomAction.ApplyByLeaveKingdom(c, false);
                        ModDebug.ShowMessage(c.StringId + " has forcefully been removed from parent kingdom", DebugMessageType.Diplomacy);
                    }
                }
            }
            catch (Exception e)
            {
                ErrorHandler.HandleException(e, "daily make sure no kingdom process");
            }
        }


        private void GetData()
        {
            dataManager = Data.DiplomacyDataManager.Instance;
        }
    }
}
