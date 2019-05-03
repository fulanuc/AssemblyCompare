using System;

namespace RoR2
{
	// Token: 0x0200045D RID: 1117
	public static class TransitionCommand
	{
		// Token: 0x1700024B RID: 587
		// (get) Token: 0x06001906 RID: 6406 RVA: 0x000129E4 File Offset: 0x00010BE4
		// (set) Token: 0x06001907 RID: 6407 RVA: 0x000129EB File Offset: 0x00010BEB
		public static bool requestPending { get; private set; }

		// Token: 0x06001908 RID: 6408 RVA: 0x000820EC File Offset: 0x000802EC
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

		// Token: 0x06001909 RID: 6409 RVA: 0x0008213C File Offset: 0x0008033C
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

		// Token: 0x04001C63 RID: 7267
		private static float timer;

		// Token: 0x04001C64 RID: 7268
		private static string commandString;
	}
}
