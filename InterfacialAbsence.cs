using HarmonyLib;
using NeosModLoader;
using FrooxEngine;
using BaseX;
using CodeX;
using System;
using FrooxEngine.LogiX;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;



namespace InterfacialAbsence;
public class InterfacialAbsence : NeosMod
{
    public override string Author => "Cyro";
    public override string Name => "Interfacial Absence";
    public override string Version => "1.0.0";
    
    public override void OnEngineInit()
    {
        Harmony harmony = new Harmony("net.Cyro.InterfacialAbsence");
        harmony.PatchAll();
    }

    [HarmonyPatch]
    public static class LogixTip_Patch
    {
        [HarmonyPatch(typeof(LogixTip), "GenerateMenuItems")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> GenerateMenuItems_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var codes = instructions.ToList();
            int index = codes.FindLastIndex(x => {
                if (!(x.operand is MethodInfo) || x.opcode != OpCodes.Callvirt)
                    return false;
                
                MethodInfo operand = (MethodInfo)x.operand;
                return operand.Name == "AddEnumShiftItem" && operand.GetGenericArguments()[0] == typeof(LogixTip.ExtractMode);
            });

            if (index == -1)
            {
                Msg("InterfacialAbsence: Failed to find AddEnumShiftItem");
                return codes;
            }
            
            int removalRangeIndex = -1;

            for (int i = index; i > 0; i--)
            {
                if (codes[i].opcode == OpCodes.Callvirt && codes[i].operand is MethodInfo)
                {
                    MethodInfo operand = (MethodInfo)codes[i].operand;
                    if (operand.Name == "AddEnumShiftItem" && operand.GetGenericArguments()[0] == typeof(LogixTraversal))
                    {
                        removalRangeIndex = i;
                        break;
                    }
                }
            }
            
            if (removalRangeIndex == -1)
            {
                Msg("InterfacialAbsence: Could not find removal range");
                return codes;
            }
            codes.RemoveRange(removalRangeIndex + 1, index - removalRangeIndex);            
            
            codes.InsertRange(removalRangeIndex + 1, new List<CodeInstruction>() {
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(LogixTip), "_extract")),
                new CodeInstruction(OpCodes.Call, typeof(EvilMethods).GetMethod("EvilInjectExtractModes"))
            });
            return codes;
        }

        [HarmonyPatch(typeof(LogixTip), "OnSecondaryPress")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> OnSecondaryPress_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var codes = instructions.ToList();
            int index = codes.FindLastIndex(x => {
                if (x.opcode != OpCodes.Switch)
                    return false;
                return true;
            });

            if (index == -1)
            {
                Msg("InterfacialAbsence: Failed to find switch");
                return codes;
            }

            int IsInstIndex = -1;

            for (int i = index; i > 0; i--)
            {
                if (codes[i].opcode == OpCodes.Isinst && codes[i].operand is Type)
                {
                    if ((Type)codes[i].operand == typeof(IField))
                    {
                        IsInstIndex = i;
                        break;
                    }
                }
            }

            if (IsInstIndex == -1)
            {
                Msg("InterfacialAbsence: Failed to find IsInst");
                return codes;
            }

            LocalBuilder? LocalVariable = codes[IsInstIndex + 1].operand as LocalBuilder;

            if (LocalVariable == null)
            {
                Msg("InterfacialAbsence: Failed to find local variable");
                Msg(codes[IsInstIndex + 1].operand.GetType().ToString() + ": " + codes[IsInstIndex + 1].operand.ToString());
                return codes;
            }

            Label SwitchEnd = (Label)codes[index + 1].operand;

            int SwitchEndIndex = codes.FindLastIndex(x => x.labels.Any(l => l == SwitchEnd));
            // Msg("InterfacialAbsence: SwitchEndIndex: " + SwitchEndIndex);

            codes.InsertRange(SwitchEndIndex + 1, new List<CodeInstruction>() {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldloc_S, LocalVariable.LocalIndex),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(LogixTip), "_extract")),
                new CodeInstruction(OpCodes.Call, typeof(EvilMethods).GetMethod("EvilSwitchCaseChecker"))
            });
            return codes;
        }
    }
}
