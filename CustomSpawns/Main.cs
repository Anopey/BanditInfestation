using System;
using CustomSpawns.Data;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using CustomSpawns.PartySpeed;
using CustomSpawns.RewardSystem;
//using CustomSpawns.MCMv3;
using CustomSpawns.UtilityBehaviours;
using HarmonyLib;

namespace CustomSpawns
{
    public class Main : MBSubModuleBase
    {
        public static readonly string version = "v1.4.8";
        public static readonly bool isAPIMode = false;
        public static PartySpeedContext PartySpeedContext;

        private static bool removalMode = false;

        #region Taleworlds Sub Mod Callbacks

        protected override void OnSubModuleLoad()
        {
            ModIntegration.SubModManager.LoadAllValidDependentMods();
            if (ConfigLoader.Instance.Config.IsRemovalMode)
            {
                removalMode = true;
                return;
            }
            removalMode = false;
            PartySpeedContext = new PartySpeedContext();
        }

        public override void OnCampaignStart(Game game, object starterObject)
        {
            base.OnCampaignStart(game, starterObject);
            if (!(game.GameType is Campaign))
                return;
            try
            {
                AddBehaviours(starterObject as CampaignGameStarter);
            }
            catch (Exception e)
            {
                ErrorHandler.HandleException(e, "Error while adding campaign behaviours");
            }
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            //if it is new campaign then the player has to go through the menus etc.
            if (!(game.GameType is Campaign) || ((Campaign)game.GameType).CampaignGameLoadingType == Campaign.GameLoadingType.NewCampaign) 
                return;
            AddBehaviours(gameStarterObject as CampaignGameStarter);
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot() //assure player :) also myself lol
        {
            UX.ShowMessage("CustomSpawns " + version + " is now enabled. Enjoy! :)", Color.ConvertStringToColor("#001FFFFF"));
            AI.AIManager.FlushRegisteredBehaviours(); //forget old behaviours to allocate space. 
            foreach (var subMod in ModIntegration.SubModManager.dependentModsArray)
            {
                UX.ShowMessage( subMod.SubModuleName + " is now integrated into the CustomSpawns API.", Color.ConvertStringToColor("#001FFFFF"));
            }
            //ConfigLoader.Instance.Config.GetInstance();
        }

        #endregion

        private void ClearLastInstances()
        {
            Data.DiplomacyDataManager.ClearInstance(this);
            Data.SpawnDataManager.ClearInstance(this);
            Data.NameSignifierData.ClearInstance(this);
        }

        private void AddBehaviours(CampaignGameStarter starter)
        {
            if (!removalMode)
            {

                OnSaveStartRunBehaviour.InitializeSave(starter);
                OnSaveStartRunBehaviour.Singleton.RegisterFunctionToRunOnSaveStart(OnSaveStart);

                starter.AddBehavior(new Spawn.SpawnBehaviour());
                starter.AddBehavior(new AI.HourlyPatrolAroundSpawnBehaviour());
                starter.AddBehavior(new AI.AttackClosestIfIdleForADayBehaviour());
                starter.AddBehavior(new AI.PatrolAroundClosestLestInterruptedAndSwitchBehaviour());
                starter.AddBehavior(new Diplomacy.ForcedWarPeaceBehaviour());
                starter.AddBehavior(new Diplomacy.ForceNoKingdomBehaviour());
                starter.AddBehavior(new PrisonerRecruitment.PrisonerRecruitmentBehaviour());
                starter.AddBehavior(new Dialogues.CustomSpawnsDialogueBehavior());
                starter.AddBehavior(new SpawnRewardBehavior());



                //campaign behaviours
                starter.AddBehavior(CampaignData.DevestationMetricData.Singleton);
                starter.AddBehavior(CampaignData.DailyLogger.Singleton);
                starter.AddBehavior(CampaignData.CampaignTest.Singleton);

                //these come last! assuming those that are added last are also run last.
                starter.AddBehavior(MobilePartyTrackingBehaviour.Singleton);
            }
            else
            {
                starter.AddBehavior(new Utils.RemoverBehaviour());
            }
        }

        private void OnSaveStart()
        {
            //restore lost AI behaviours!
            try
            {
                var partyIDToData = SpawnDataManager.Instance.PartyIDToData;
                foreach (MobileParty mb in MobileParty.All)
                {
                    string id = CampaignUtils.IsolateMobilePartyStringID(mb);
                    if(id != "" && partyIDToData.ContainsKey(id))
                    {
                        var spawnData = partyIDToData[id];
                        Spawn.Spawner.HandleAIChecks(mb, spawnData, mb.HomeSettlement);
                    }

                }
            }catch(Exception e)
            {
                ErrorHandler.HandleException(e, " reconstruction of save custom spawns mobile party data");
            }
        }

        public override void OnGameInitializationFinished(Game game) {
            base.OnGameInitializationFinished(game);
            try
            {
                Harmony harmony = new Harmony("com.Questry.CustomSpawns");
                harmony.PatchAll();
            }
            catch (Exception e)
            {
                ErrorHandler.HandleException(e, "HARMONY PATCHES");
            }
            
            try
            {
                // Spawn Data Init (Read from XML)
                ClearLastInstances();
                SpawnDataManager.Init();
            }
            catch (Exception e)
            {
                ErrorHandler.HandleException(e, "Could not create an instance of SpawnDataManager. Might have encountered an " +
                                                "issue while parsing the XML file or invalid parameters/values have been found");
            }
        }

    }
}
