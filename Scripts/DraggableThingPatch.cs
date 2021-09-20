using Assets.Scripts;
using Assets.Scripts.Inventory;
using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Items;
using Assets.Scripts.Objects.Pipes;
using Assets.Scripts.Vehicles;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AllowAIMeERepair.Scripts
{
	[HarmonyPatch(typeof(DraggableThing))]
	public static class DraggableThingPatch
    {
		public static float RepairSpeedScale = 0.6f;
		public static float GetRepairQuantity(Thing thing, DuctTape tape, float actionCompletionRatio = 1.0f)
		{
			bool flag = thing == null;
			float result;
			if (flag)
			{
				result = 0f;
			}
			else
			{
				result = Mathf.Min(GetRepairRatio(thing) * actionCompletionRatio, tape.Quantity);
			}
			return result;
		}

		public static float GetRepairRatio(Thing thing)
		{
			return thing.DamageState.TotalRatio;
		}

		[HarmonyPatch("AttackWith")]
		[HarmonyPrefix]
		public static bool AttackWithPrefix(DraggableThing __instance, ref Thing.DelayedActionInstance __result, Attack attack, bool doAction = true)
		{
			DuctTape ductTape = attack.SourceItem as DuctTape;
			WheeledBase wheeledBase = __instance as WheeledBase;
			bool isRobotRepair = ductTape != null && wheeledBase != null;
			if (isRobotRepair)
			{
				float num = GetRepairQuantity(__instance, ductTape, 1f);
				Thing.DelayedActionInstance delayedActionInstance = new Thing.DelayedActionInstance
				{
					Duration = num * ductTape.RepairSpeed * RepairSpeedScale,
					ActionMessage = "Patch"
				};
				bool flag2 = __instance.DamageState.TotalRatio <= float.Epsilon;
				if (flag2)
				{
					__result = delayedActionInstance.Fail(string.Format("{0} is not damaged", __instance.ToTooltip()));
				}
				bool flag3 = !doAction;
				if (flag3)
				{
					__result = delayedActionInstance;
				}
				ductTape.CallCmdRepair(__instance.netId, num * attack.CompletedRatio);

				__result = DynamicThingPatch.AttackWith(__instance, attack, doAction);

				return false; // skip original method
			}

			DynamicThing sourceItem = attack.SourceItem;
			bool flag = !sourceItem;
			Thing.DelayedActionInstance result;
			if (flag)
			{
				result = null;
			}
			else
			{
				Thing.DelayedActionInstance delayedActionInstance = new Thing.DelayedActionInstance
				{
					Duration = 0f,
					ActionMessage = ActionStrings.Rename
				};
				Labeller labeller = sourceItem as Labeller;
				bool flag2 = labeller;
				if (flag2)
				{
					bool flag3 = !labeller.OnOff;
					if (flag3)
					{
						result = delayedActionInstance.Fail(HelpTextDevice.DeviceNotOn);
					}
					else
					{
						bool flag4 = !labeller.IsOperable;
						if (flag4)
						{
							result = delayedActionInstance.Fail(HelpTextDevice.DeviceNoPower);
						}
						else
						{
							bool flag5 = !doAction;
							if (flag5)
							{
								result = delayedActionInstance;
							}
							else
							{
								labeller.Rename(__instance);
								result = delayedActionInstance;
							}
						}
					}
				}
				else
				{
					result = DynamicThingPatch.AttackWith(__instance, attack, doAction);
				}
			}
			__result = result;

			return false; // skip original method
		}

	}
}
