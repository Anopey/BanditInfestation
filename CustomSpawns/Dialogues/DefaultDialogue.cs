﻿using System.Collections.Generic;
using CustomSpawns.Data;
using CustomSpawns.Dialogues.DialogueAlgebra;

namespace CustomSpawns.Dialogues
{
    public class DefaultDialogue
    {
        private readonly DialogueConditionInterpretor _dialogueConditionInterpretor;
        private readonly DialogueConsequenceInterpretor _dialogueConsequenceInterpretor;

        public DefaultDialogue(DialogueConditionInterpretor dialogueConditionInterpretor, DialogueConsequenceInterpretor dialogueConsequenceInterpretor)
        {
            _dialogueConditionInterpretor = dialogueConditionInterpretor;
            _dialogueConsequenceInterpretor = dialogueConsequenceInterpretor;
            DefaultDialogueData = new();
            DefaultDialogueData.Add(HostileAttackDialogue());
            DefaultDialogueData.Add(HostileDefendDialogue());
            DefaultDialogueData.Add(FriendlyDialogue());
        }
        
        public List<DialogueData> DefaultDialogueData { get; }

        private DialogueData HostileAttackDialogue()
        {
            DialogueCondition dialogueCondition =
                _dialogueConditionInterpretor.ParseCondition("IsCustomSpawnParty AND IsHostile AND IsAttacking");
            DialogueConsequence consequence = _dialogueConsequenceInterpretor.ParseConsequence("Battle");
            DialogueData dialogueData = new();
            dialogueData.Dialogue_ID = "CS_Default_Dialogue_1";
            dialogueData.InjectedToTaleworlds = false;
            dialogueData.IsPlayerDialogue = false;
            dialogueData.DialogueText = "That's a nice head on your shoulders!";
            dialogueData.Consequence = consequence;
            dialogueData.Condition = dialogueCondition;
            dialogueData.Children = new();
            return dialogueData;
        }
        
        private DialogueData HostileDefendDialogue()
        {
            DialogueCondition dialogueCondition =
                _dialogueConditionInterpretor.ParseCondition("IsCustomSpawnParty AND IsHostile AND IsDefending");
            DialogueConsequence consequence = _dialogueConsequenceInterpretor.ParseConsequence("Battle");
            DialogueData dialogueData = new();
            dialogueData.Dialogue_ID = "CS_Default_Dialogue_2";
            dialogueData.InjectedToTaleworlds = false;
            dialogueData.IsPlayerDialogue = false;
            dialogueData.DialogueText = "I will drink from your skull!";
            dialogueData.Consequence = consequence;
            dialogueData.Condition = dialogueCondition;
            dialogueData.Children = new();
            return dialogueData;
        }
        
        private DialogueData FriendlyDialogue()
        {
            DialogueCondition dialogueCondition =
                _dialogueConditionInterpretor.ParseCondition("IsCustomSpawnParty AND IsFriendly");
            DialogueData dialogueData = new();
            dialogueData.Dialogue_ID = "CS_Default_Dialogue_3";
            dialogueData.InjectedToTaleworlds = false;
            dialogueData.IsPlayerDialogue = false;
            dialogueData.DialogueText = "It's almost harvesting season!";
            dialogueData.Condition = dialogueCondition;
            dialogueData.Children = new();
            dialogueData.Children.Add(AttackFriendlyDialogue());
            dialogueData.Children.Add(LeaveFriendlyDialogue());
            return dialogueData;
        }
        
        private DialogueData LeaveFriendlyDialogue()
        {
            DialogueConsequence consequence = _dialogueConsequenceInterpretor.ParseConsequence("Leave");
            DialogueData dialogueData = new();
            dialogueData.Dialogue_ID = "CS_Default_Dialogue_4";
            dialogueData.InjectedToTaleworlds = false;
            dialogueData.IsPlayerDialogue = true;
            dialogueData.DialogueText = "Away with you vile beggar!";
            dialogueData.Consequence = consequence;
            dialogueData.Children = new();
            return dialogueData;
        }
        
        private DialogueData AttackFriendlyDialogue()
        {
            DialogueConsequence consequence = _dialogueConsequenceInterpretor.ParseConsequence("Battle");
            DialogueData dialogueData = new();
            dialogueData.Dialogue_ID = "CS_Default_Dialogue_5";
            dialogueData.InjectedToTaleworlds = false;
            dialogueData.IsPlayerDialogue = true;
            dialogueData.DialogueText = "I will tear you limb from limb!";
            dialogueData.Consequence = consequence;
            dialogueData.Children = new();
            return dialogueData;
        }
    }
}