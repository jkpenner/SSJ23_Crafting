using System;
using System.Collections.Generic;
using UnityEngine;

namespace SSJ23_Crafting
{
    [Serializable]
    public class Stat
    {
        [SerializeField] float baseValue;

        private List<StatMod> mods;

        public float BaseValue => baseValue;
        public float Value { get; private set; }

        public Stat(float baseValue)
        {
            this.baseValue = baseValue;
            mods = new List<StatMod>();
            Recalculate();
        }

        public void AddMod(StatMod mod)
        {
            if (mods.Contains(mod))
            {
                return;
            }

            mods.Add(mod);
            Recalculate();
        } 

        public void RemoveMod(StatMod mod)
        {
            if (!mods.Remove(mod))
            {
                return;
            }

            Recalculate();
        }

        public void Recalculate()
        {
            float flatMods = 0f;

            foreach(var mod in mods)
            {
                switch (mod.Type)
                {
                    case StatModType.Flat:
                        flatMods += mod.Amount;
                        break;
                }
            }

            Value = baseValue + flatMods;
        }
    }

    public enum StatModType
    {
        Flat,
    }

    [SerializeField]
    public class StatMod
    {
        [SerializeField] StatModType type;
        [SerializeField] float amount;

        public StatModType Type => type;
        public float Amount => amount;

        public StatMod(StatModType type, float amount)
        {
            this.type = type;
            this.amount = amount;
        }

        public static StatMod Flat(float amount)
        {
            return new StatMod(StatModType.Flat, amount);
        }
    }
}