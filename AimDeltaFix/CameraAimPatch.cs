using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace AimDeltaFix
{
    [HarmonyPatch(typeof(CameraAim), "Update")]
    internal class CameraAimPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var lerpMethod = AccessTools.Method(typeof(Quaternion), "Lerp");
            var counter = 0;

            foreach (var instruction in instructions)
            {
                if (instruction.Calls(lerpMethod))
                {
                    counter++;
                    if (counter == 2)
                    {
                        // Inject our fix right before the second Quaternion.Lerp call.
                        // This modifies the lerp factor (num3 * Time.deltaTime) on the stack
                        // to be frame-rate independent before it's passed to Lerp.
                        yield return new CodeInstruction(
                            OpCodes.Call,
                            AccessTools.Method(typeof(CameraAimPatch), nameof(FixLerpFactor))
                        );
                    }
                }

                yield return instruction;
            }
        }

        // The base game does: Quaternion.Lerp(current, target, num3 * Time.deltaTime)
        //
        // num3 ranges from 50 (smoothing off) to 8 (smoothing max).
        // This works fine at ~60fps, but at higher framerates Time.deltaTime shrinks,
        // making the per-frame lerp factor tiny. The camera barely moves each frame,
        // which feels like lower mouse sensitivity even though the math converges
        // over time.
        //
        // Fix: when smoothing is off, snap directly to target.
        // When smoothing is on, use exponential decay for frame-rate independence.
        public static float FixLerpFactor(float t)
        {
            if (GameplayManager.instance.cameraSmoothing <= 0f)
                return 1f;

            return 1f - Mathf.Exp(-t);
        }
    }
}
