using System;

namespace RoR2
{
	// Token: 0x02000468 RID: 1128
	public static class TransitionCommand
	{
		// Token: 0x17000257 RID: 599
		// (get) Token: 0x06001963 RID: 6499 RVA: 0x00012EFE File Offset: 0x000110FE
		// (set) Token: 0x06001964 RID: 6500 RVA: 0x00012F05 File Offset: 0x00011105
		public static bool requestPending { get; private set; }

		// Token: 0x06001965 RID: 6501 RVA: 0x00082A94 File Offset: 0x00080C94
		private static void Update()
		{
			if (FadeToBlackManager.fullyFaded)
			{
				RoR2Application.onUpdate -= TransitionCommand.Update;
				TransitionCommand.requestPending = false;
				FadeToBlackManager.fadeCount--;
				string cmd = TransitionCommand.commandString;
				TransitionCommand.commandString = null;
				Console.instance.SubmitCmd(null, cmd, false);
			}
		}

		// Token: 0x06001966 RID: 6502 RVA: 0x00082AE4 File Offset: 0x00080CE4
		[ConCommand(commandName = "transition_command", flags = ConVarFlags.None, helpText = "Fade out and execute a command at the end of the fadeout.")]
		private static void CCTransitionCommand(ConCommandArgs args)
		{
			args.CheckArgumentCount(1);
			if (TransitionCommand.requestPending)
			{
				return;
			}
			TransitionCommand.requestPending = true;
			TransitionCommand.commandString = args[0];
			FadeToBlackManager.fadeCount++;
			RoR2Application.onUpdate += TransitionCommand.Update;
		}

		// Token: 0x04001C97 RID: 7319
		private static float timer;

		// Token: 0x04001C98 RID: 7320
		private static string commandString;
	}
}
