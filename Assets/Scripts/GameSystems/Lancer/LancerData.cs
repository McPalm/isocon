using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Lancer
{
    [System.Serializable]
    public class LancerData
    {
        public int CurrentHP;
        public int MaxHP;
        public int OverShield;
        public int CurrentHeat;
        public int MaxHeat;
        public int StructureDamage;
        public int MaxStructure;
        public int StressDamage;
        public int MaxStress;
        public int Armor;
        public int Evasion;
        public int EDefence;
        public int Sensors;
        public int TechAttack;
        public int RepairCapacity;
        public int SaveTarget;
        public int Speed;
        public int OverCharge;

        

        static public LancerData Everest() => new LancerData()
        {
            CurrentHP = 10,
            MaxHP = 10,
            MaxHeat = 6,
            MaxStructure = 4,
            MaxStress = 4,
            Evasion = 8,
            EDefence = 8,
            Speed = 4,
            Sensors = 10,
            SaveTarget = 10,
            RepairCapacity = 5,
        };
    }
}
