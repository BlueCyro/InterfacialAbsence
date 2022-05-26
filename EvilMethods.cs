using HarmonyLib;
using NeosModLoader;
using FrooxEngine;
using BaseX;
using CodeX;
using System;
using FrooxEngine.LogiX;
using FrooxEngine.LogiX.Actions;
using FrooxEngine.LogiX.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;


namespace InterfacialAbsence;

public static class EvilMethods
{
    public static void EvilInjectExtractModes(ContextMenu menu, Sync<LogixTip.ExtractMode> extractMode)
    {
        var options = new List<OptionDescription<LogixTip.ExtractMode>>();
        string formatString = "Extract: {0}";

        foreach(LogixTip.ExtractMode mode in Enum.GetValues(typeof(LogixTip.ExtractMode)))
        {
            string label = String.Format(formatString, StringHelper.BeautifyName(mode.ToString()));
            options.Add(new OptionDescription<LogixTip.ExtractMode>(mode, label, color.Cyan, NeosAssets.Common.Icons.External)); 
        }
        
        options.Add(new OptionDescription<LogixTip.ExtractMode>((LogixTip.ExtractMode)3, String.Format(formatString, "Linked Register"), color.Green, NeosAssets.Common.Icons.External));
        options.Add(new OptionDescription<LogixTip.ExtractMode>((LogixTip.ExtractMode)4, String.Format(formatString, "Drive Value Node"), color.Green, NeosAssets.Common.Icons.External));

        var MenuItem = menu.AddItem("ExtractModes", NeosAssets.Common.Icons.External, color.White);
        MenuItem.SetupValueCycle(extractMode, options);
    }

    public static void EvilSwitchCaseChecker(LogixTip __instance, IField field, Sync<LogixTip.ExtractMode> extractMode)
    {
        switch(extractMode.Value)
        {
            case (LogixTip.ExtractMode)3:
                if (field == null || !field.GetType().IsGenericType || !field.GetType().GetGenericArguments()[0].IsValueType)
                    break;
                
                Type NodeType = field.GetType().GetGenericArguments()[0];

                Slot NodeSlot = __instance.LocalUserSpace.AddSlot(field.Name);
                NodeSlot.Tag = field.Name;
                AccessTools.Method(typeof(LogixTip), "PositionSpawnedNode").Invoke(__instance, new object[] { NodeSlot });
                Type RegisterType = typeof(ValueRegister<>).MakeGenericType(NodeType);
                var Register = (LogixNode)NodeSlot.AttachComponent(RegisterType);
                
                Register.GenerateVisual();
                IField RegisterField = (IField)AccessTools.Field(RegisterType, "Value").GetValue(Register);
                RegisterField.DriveFrom(field, true);
                break;
            case (LogixTip.ExtractMode)4:
                if (field == null || !field.GetType().IsGenericType)
                    break;
                
                Type GenericType = field.GetType().GetGenericArguments()[0];
                Type DriveNodeType = !(field is ISyncRef) && (GenericType.IsValueType || GenericType == typeof(string)) ? GenericType : typeof(BaseX.RefID);
                Slot DriverSlot = __instance.LocalUserSpace.AddSlot("Drive " + field.Name);
                DriverSlot.GlobalScale = new float3(1f, 1f, 1f) * __instance.LocalUserRoot.GlobalScale;
                Slot RefNode = __instance.LocalUserSpace.AddSlot("Ref");
                RefNode.GlobalScale = new float3(1f, 1f, 1f) * __instance.LocalUserRoot.GlobalScale;

                DriverSlot.Tag = "Drive " + field.Name;
                AccessTools.Method(typeof(LogixTip), "PositionSpawnedNode").Invoke(__instance, new object[] { DriverSlot });
                Type DriverType = typeof(DriveValueNode<>).MakeGenericType(DriveNodeType);
                var Driver = (LogixNode)DriverSlot.AttachComponent(DriverType);
                Driver.GenerateVisual();
                ISyncRef DriverDriveTarget = (ISyncRef)AccessTools.Field(DriverType, "DriveTarget").GetValue(Driver);

                Type IFieldType = typeof(IField<>).MakeGenericType(DriveNodeType);
                Type RefType = typeof(ReferenceNode<>).MakeGenericType(IFieldType);
                var Ref = (IReferenceNode)RefNode.AttachComponent(RefType);
                ((LogixNode)Ref).GenerateVisual();
                Ref.SetRefTarget(field);
                DriverDriveTarget.Target = Ref;

                RefNode.SetParent(DriverSlot);
                RefNode.LocalPosition = new float3(0.11f, 0.09f, 0f);
                RefNode.LocalRotation = floatQ.Euler(0f, 180f, 0f);

                AccessTools.DeclaredMethod(typeof(DriveValueNode<>).MakeGenericType(DriveNodeType), "StartDrive").Invoke(Driver, new object[] { });
                break;
            default:
                break;
        }
    }
}

