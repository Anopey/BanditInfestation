using System;
using System.Collections.Generic;

namespace CustomSpawns.PartySpeed
{
    public class PartySpeedContext
    {
        private Dictionary<string, float> _partyIDToExtraSpeed = new Dictionary<string, float>();
        private Dictionary<string, float> _partyIDToBaseSpeed = new Dictionary<string, float>();
        private Dictionary<string, float> _partyIDToMinimumSpeed = new Dictionary<string, float>();
        private Dictionary<string, float> _partyIDToMaximumSpeed = new Dictionary<string, float>();

        public void RegisterPartyExtraBonusSpeed(string partyBaseID, float extraSpeed)
        {
            if (_partyIDToExtraSpeed.ContainsKey(partyBaseID) || !ConfigLoader.Instance.Config.ModifyPartySpeeds)
                return;
            _partyIDToExtraSpeed.Add(partyBaseID, extraSpeed);
        }

        public void RegisterPartyMinimumSpeed(string partyBaseID, float minimumSpeed)
        {
            if (_partyIDToMinimumSpeed.ContainsKey(partyBaseID) || !ConfigLoader.Instance.Config.ModifyPartySpeeds)
                return;
            _partyIDToMinimumSpeed.Add(partyBaseID, minimumSpeed);
        }

        public void RegisterPartyMaximumSpeed(string partyBaseID, float maximumSpeed)
        {
            if (_partyIDToMaximumSpeed.ContainsKey(partyBaseID) || !ConfigLoader.Instance.Config.ModifyPartySpeeds)
                return;
            _partyIDToMaximumSpeed.Add(partyBaseID, maximumSpeed);
        }

        public void RegisterPartyBaseSpeed(string partyBaseID, float maximumSpeed)
        {
            if (_partyIDToBaseSpeed.ContainsKey(partyBaseID) || !ConfigLoader.Instance.Config.ModifyPartySpeeds)
                return;
            _partyIDToBaseSpeed.Add(partyBaseID, maximumSpeed);
        }

        public bool IsPartySpeedBonusAllowedByUser() => ConfigLoader.Instance.Config.ModifyPartySpeeds;
        
        public bool IsBasePartySpeedOverriden(string partyBaseID) => _partyIDToBaseSpeed.ContainsKey(partyBaseID);
        
        public bool IsPartyEligibleForExtraSpeed(string partyBaseID) => _partyIDToExtraSpeed.ContainsKey(partyBaseID);
        
        public bool IsPartyMinimumSpeedOverriden(string partyBaseID) => _partyIDToMinimumSpeed.ContainsKey(partyBaseID);
        
        public bool IsPartyMaximumSpeedOverriden(string partyBaseID) => _partyIDToMaximumSpeed.ContainsKey(partyBaseID);
        
        public float GetBaseSpeed(string partyBaseID)
        {
            if (!_partyIDToBaseSpeed.ContainsKey(partyBaseID))
                throw new ArgumentException("Invalid partyId ! Party should have been registered beforehand.");
            float baseSpeed;
            _partyIDToBaseSpeed.TryGetValue(partyBaseID, out baseSpeed);
            return baseSpeed;
        }
        
        public float GetSpeedWithExtraBonus(string partyBaseID)
        {
            if (!_partyIDToExtraSpeed.ContainsKey(partyBaseID))
                throw new ArgumentException("Invalid partyId ! Party should have been registered beforehand.");
            float speedWithExtraBonus;
            _partyIDToExtraSpeed.TryGetValue(partyBaseID, out speedWithExtraBonus);
            return speedWithExtraBonus;
        }
        
        public float GetMinimumSpeed(string partyBaseID)
        {
            if (!_partyIDToMinimumSpeed.ContainsKey(partyBaseID))
                throw new ArgumentException("Invalid partyId ! Party should have been registered beforehand.");
            float minimumSpeed;
            _partyIDToMinimumSpeed.TryGetValue(partyBaseID, out minimumSpeed);
            return minimumSpeed;
        }
        
        public float GetMaximumSpeed(string partyBaseID)
        {
            if (!_partyIDToMaximumSpeed.ContainsKey(partyBaseID))
                throw new ArgumentException("Invalid partyId ! Party should have been registered beforehand.");
            float maximumSpeed;
            _partyIDToMaximumSpeed.TryGetValue(partyBaseID, out maximumSpeed);
            return maximumSpeed;
        }
    }
}