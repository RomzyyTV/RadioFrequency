using System.Collections.Generic;
using System.Reflection.Emit;
using Exiled.API.Features;
using Exiled.API.Features.Pools;
using HarmonyLib;
using RadioFrequency.Features;
using VoiceChat;
using VoiceChat.Networking;

namespace RadioFrequency.Patches
{
    [HarmonyPatch(typeof(VoiceTransceiver), nameof(VoiceTransceiver.ServerReceiveMessage))]
    internal static class VoiceTransceiverPatch
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> ServerReceiveMessagePatch(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int index = newInstructions.FindIndex(i =>
                i.opcode == OpCodes.Stfld &&
                i.StoresField(AccessTools.Field(typeof(VoiceMessage), nameof(VoiceMessage.Channel)))) + 1;

            int continueIndex = newInstructions.FindIndex(i =>
                i.opcode == OpCodes.Call && 
                i.Calls(AccessTools.Method(typeof(HashSet<ReferenceHub>.Enumerator), nameof(HashSet<ReferenceHub>.Enumerator.MoveNext)))) - 1;

            Label continueLabel = newInstructions[continueIndex].labels[0];

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldfld, AccessTools.Field(typeof(VoiceMessage), nameof(VoiceMessage.Speaker))),
                new(OpCodes.Ldloc_3),
                new(OpCodes.Ldloc_S, 5),
                new(OpCodes.Call, AccessTools.Method(typeof(VoiceTransceiverPatch), nameof(VoiceTransceiverPatch.CanSendMessage))),
                new(OpCodes.Brfalse_S, continueLabel)
            });

            foreach (CodeInstruction instruction in newInstructions)
                yield return instruction;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static bool CanSendMessage(ReferenceHub speaker, ReferenceHub target, VoiceChatChannel channel)
        {
            if (channel != VoiceChatChannel.Radio)
                return true;

            Player speakerPlayer = Player.Get(speaker);
            Player targetPlayer = Player.Get(target);

            return Frequency.TryGetPlayerFrequency(speakerPlayer, out Frequency frequency) && frequency.Players.Contains(targetPlayer);
        }
    }
}