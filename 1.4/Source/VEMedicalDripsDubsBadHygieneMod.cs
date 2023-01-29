using HarmonyLib;
using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;

namespace VEMedicalDripsDubsBadHygiene
{
    public class CompProperties_FillNeeds : CompProperties
    {
        public float thirstFillPerDay;
        public float hygieneFillPerDay;
        public CompProperties_FillNeeds()
        {
            this.compClass = typeof(CompFillNeeds);
        }
    }
    public class CompFillNeeds : ThingComp
    {
        public CompFacility compFacility;
        public CompProperties_FillNeeds Props => props as CompProperties_FillNeeds;
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            this.compFacility = parent.GetComp<CompFacility>();
        }
        public override void CompTick()
        {
            base.CompTick();
            if (compFacility.CanBeActive)
            {
                foreach (var linkedBuilding in compFacility.LinkedBuildings)
                {
                    if (linkedBuilding is Building_Bed bed)
                    {
                        foreach (var occupant in bed.CurOccupants)
                        {
                            FillThirstNeed(occupant, GetFillValue(Props.thirstFillPerDay));
                            FillHygieneNeed(occupant, GetFillValue(Props.hygieneFillPerDay));
                        }
                    }
                }
            }
        }

        public float GetFillValue(float fillRatePerDay)
        {
            switch (parent.def.tickerType)
            {
                case TickerType.Normal: return fillRatePerDay / (float)GenDate.TicksPerDay;
                case TickerType.Rare: return fillRatePerDay / (float)GenTicks.TickRareInterval / (float)GenDate.TicksPerDay;
                case TickerType.Long: return fillRatePerDay / (float)GenTicks.TickLongInterval / (float)GenDate.TicksPerDay;
            }
            return 0;
        }

        public static void FillThirstNeed(Pawn pawn, float value)
        {
            FillNeed(pawn.needs?.TryGetNeed<DubsBadHygiene.Need_Thirst>(), value);
        }

        public static void FillHygieneNeed(Pawn pawn, float value)
        {
            FillNeed(pawn.needs?.TryGetNeed<DubsBadHygiene.Need_Hygiene>(), value);
        }

        private static void FillNeed(Need need, float value)
        {
            if (need != null)
            {
                if (need.MaxLevel > need.CurLevel)
                {
                    need.CurLevel += value;
                }
            }
        }
    }
}
