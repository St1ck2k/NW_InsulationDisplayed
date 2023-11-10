using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Strings;

using Noggog;
using Mutagen.Bethesda.Plugins.Records;
using System.Runtime.Serialization;


namespace NW_InsulationDisplayed
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<IFallout4Mod, IFallout4ModGetter>(RunPatch)
                .SetTypicalOpen(GameRelease.Fallout4, "NW_InsulationDisplayed.esp")
                .Run(args);
        }
        
        public static void RunPatch(IPatcherState<IFallout4Mod, IFallout4ModGetter> state)
        {
            var insulation = " | Insulation:";

            var suffixes = new List<string> { "0", "5", "10", "15","20", "25", "30", "35", "40", "45", "50", "55", "60", "65", "70", "75", "80", "85", "90", "95", "100" }; // Add more suffixes as needed

            foreach (var formList in state.LoadOrder.PriorityOrder.FormList().WinningContextOverrides())
            {
                if (formList.Record.EditorID == null) continue;

                foreach (var suffix in suffixes)
                {
                    if (formList.Record.EditorID.Equals($"Winter_ArmorExposure_{suffix}"))
                    {
                        foreach (var entry in formList.Record.Items)
                        {
                            if (entry is null) continue;

                            if (entry.TryResolve(state.LinkCache, out var record))
                            {
                                if (record is null) continue;

                                if (record.IsDeleted) continue;

                                var armorRecord = state.PatchMod.Armors.GetOrAddAsOverride(record);

                                if (armorRecord.Name is null) continue;

                                if (armorRecord.Name.TryLookup(Language.English, out var armorName))
                                {
                                    if (!armorName.Contains(insulation + $" {suffix}"))
                                    {
                                        var newName = armorName.Replace(armorName, armorName + insulation + $" {suffix}");

                                        armorRecord.Name = newName;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }            
    }
}
