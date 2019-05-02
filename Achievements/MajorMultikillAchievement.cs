using System;
using UnityEngine;

namespace RoR2.Achievements
{
	// Token: 0x020006BB RID: 1723
	[RegisterAchievement("MajorMultikill", "Items.BurnNearby", null, typeof(MajorMultikillAchievement.MajorMultikillServerAchievement))]
	public class MajorMultikillAchievement : BaseAchievement
	{
		// Token: 0x0600267E RID: 9854 RVA: 0x0001BA4F File Offset: 0x00019C4F
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x0600267F RID: 9855 RVA: 0x0001BA5E File Offset: 0x00019C5E
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x020006BC RID: 1724
		private class MajorMultikillServerAchievement : BaseServerAchievement
		{
			// Token: 0x06002681 RID: 9857 RVA: 0x0001C2A0 File Offset: 0x0001A4A0
			public override void OnInstall()
			{
				base.OnInstall();
				GlobalEventManager.onCharacterDeathGlobal += this.OnCharacterDeath;
			}

			// Token: 0x06002682 RID: 9858 RVA: 0x0001C2B9 File Offset: 0x0001A4B9
			public override void OnUninstall()
			{
				GlobalEventManager.onCharacterDeathGlobal -= this.OnCharacterDeath;
				base.OnUninstall();
			}

			// Token: 0x06002683 RID: 9859 RVA: 0x000B209C File Offset: 0x000B029C
			private void OnCharacterDeath(DamageReport damageReport)
			{
				GameObject attacker = damageReport.damageInfo.attacker;
				if (!attacker)
				{
					return;
				}
				CharacterBody component = attacker.GetComponent<CharacterBody>();
				if (!component)
				{
					return;
				}
				if (component.multiKillCount >= 15 && component.masterObject == this.serverAchievementTracker.networkUser.masterObject)
				{
					base.Grant();
				}
			}

			// Token: 0x040028AA RID: 10410
			private const int multiKillThreshold = 15;
		}
	}
}
