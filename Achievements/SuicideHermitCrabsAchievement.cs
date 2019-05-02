using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x020006B3 RID: 1715
	[RegisterAchievement("SuicideHermitCrabs", "Items.AutoCastEquipment", null, typeof(SuicideHermitCrabsAchievement.SuicideHermitCrabsServerAchievement))]
	public class SuicideHermitCrabsAchievement : BaseAchievement
	{
		// Token: 0x0600260F RID: 9743 RVA: 0x0001BD61 File Offset: 0x00019F61
		public override void OnInstall()
		{
			base.OnInstall();
			this.userProfile.onStatsReceived += this.Check;
			base.SetServerTracked(true);
		}

		// Token: 0x06002610 RID: 9744 RVA: 0x0001BD87 File Offset: 0x00019F87
		public override void OnUninstall()
		{
			this.userProfile.onStatsReceived -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x06002611 RID: 9745 RVA: 0x0001BDA6 File Offset: 0x00019FA6
		private void Check()
		{
			if (this.userProfile.statSheet.GetStatValueULong(StatDef.suicideHermitCrabsAchievementProgress) >= 20UL)
			{
				base.Grant();
			}
		}

		// Token: 0x06002612 RID: 9746 RVA: 0x0001BDC8 File Offset: 0x00019FC8
		public override float ProgressForAchievement()
		{
			return this.userProfile.statSheet.GetStatValueULong(StatDef.suicideHermitCrabsAchievementProgress) / 20f;
		}

		// Token: 0x04002856 RID: 10326
		private const int requirement = 20;

		// Token: 0x020006B4 RID: 1716
		private class SuicideHermitCrabsServerAchievement : BaseServerAchievement
		{
			// Token: 0x06002614 RID: 9748 RVA: 0x0001BDE8 File Offset: 0x00019FE8
			public override void OnInstall()
			{
				base.OnInstall();
				GlobalEventManager.onCharacterDeathGlobal += this.OnCharacterDeath;
			}

			// Token: 0x06002615 RID: 9749 RVA: 0x0001BE01 File Offset: 0x0001A001
			public override void OnUninstall()
			{
				GlobalEventManager.onCharacterDeathGlobal -= this.OnCharacterDeath;
				base.OnUninstall();
			}

			// Token: 0x06002616 RID: 9750 RVA: 0x000B0BFC File Offset: 0x000AEDFC
			private void OnCharacterDeath(DamageReport damageReport)
			{
				if (!damageReport.victimBody)
				{
					return;
				}
				if (damageReport.damageInfo.attacker)
				{
					return;
				}
				if (damageReport.victim.name.Contains("HermitCrab") && damageReport.victimBody.teamComponent.teamIndex != TeamIndex.Player)
				{
					PlayerStatsComponent masterPlayerStatsComponent = base.networkUser.masterPlayerStatsComponent;
					if (masterPlayerStatsComponent)
					{
						masterPlayerStatsComponent.currentStats.PushStatValue(StatDef.suicideHermitCrabsAchievementProgress, 1UL);
					}
				}
			}
		}
	}
}
