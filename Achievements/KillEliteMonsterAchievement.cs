using System;
using UnityEngine;

namespace RoR2.Achievements
{
	// Token: 0x020006A4 RID: 1700
	[RegisterAchievement("KillEliteMonster", "Items.Medkit", null, typeof(KillEliteMonsterAchievement.KillEliteMonsterServerAchievement))]
	public class KillEliteMonsterAchievement : BaseAchievement
	{
		// Token: 0x060025CF RID: 9679 RVA: 0x0001B314 File Offset: 0x00019514
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x060025D0 RID: 9680 RVA: 0x0001B323 File Offset: 0x00019523
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x020006A5 RID: 1701
		private class KillEliteMonsterServerAchievement : BaseServerAchievement
		{
			// Token: 0x060025D2 RID: 9682 RVA: 0x0001B9FC File Offset: 0x00019BFC
			public override void OnInstall()
			{
				base.OnInstall();
				GlobalEventManager.onCharacterDeathGlobal += this.OnCharacterDeath;
			}

			// Token: 0x060025D3 RID: 9683 RVA: 0x0001BA15 File Offset: 0x00019C15
			public override void OnUninstall()
			{
				GlobalEventManager.onCharacterDeathGlobal -= this.OnCharacterDeath;
				base.OnUninstall();
			}

			// Token: 0x060025D4 RID: 9684 RVA: 0x000B0854 File Offset: 0x000AEA54
			private void OnCharacterDeath(DamageReport damageReport)
			{
				if (!damageReport.victim)
				{
					return;
				}
				CharacterBody body = damageReport.victim.body;
				if (!body || !body.isElite)
				{
					return;
				}
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
				if (component.masterObject == this.serverAchievementTracker.networkUser.masterObject)
				{
					base.Grant();
				}
			}
		}
	}
}
