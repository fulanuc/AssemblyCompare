using System;
using UnityEngine;
using UnityEngine.Events;

namespace RoR2
{
	// Token: 0x020002EB RID: 747
	public class EventOnBodyDeaths : MonoBehaviour
	{
		// Token: 0x06000F24 RID: 3876 RVA: 0x0000BA53 File Offset: 0x00009C53
		private void OnEnable()
		{
			GlobalEventManager.onCharacterDeathGlobal += this.OnCharacterDeath;
		}

		// Token: 0x06000F25 RID: 3877 RVA: 0x0000BA66 File Offset: 0x00009C66
		private void OnDisable()
		{
			GlobalEventManager.onCharacterDeathGlobal -= this.OnCharacterDeath;
		}

		// Token: 0x06000F26 RID: 3878 RVA: 0x0005BE84 File Offset: 0x0005A084
		private void OnCharacterDeath(DamageReport damageReport)
		{
			if (damageReport.victimBody)
			{
				for (int i = 0; i < this.bodyNames.Length; i++)
				{
					if (damageReport.victimBody.name.Contains(this.bodyNames[i]))
					{
						this.currentDeathCount++;
						break;
					}
				}
			}
			if (this.currentDeathCount >= this.targetDeathCount)
			{
				UnityEvent unityEvent = this.onAchieved;
				if (unityEvent == null)
				{
					return;
				}
				unityEvent.Invoke();
			}
		}

		// Token: 0x04001326 RID: 4902
		public string[] bodyNames;

		// Token: 0x04001327 RID: 4903
		private int currentDeathCount;

		// Token: 0x04001328 RID: 4904
		public int targetDeathCount;

		// Token: 0x04001329 RID: 4905
		public UnityEvent onAchieved;
	}
}
