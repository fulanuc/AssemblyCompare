using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005B2 RID: 1458
	public class DebugStats : MonoBehaviour
	{
		// Token: 0x060020D9 RID: 8409 RVA: 0x00017EC0 File Offset: 0x000160C0
		private void Awake()
		{
			DebugStats.fpsTimestamps = new Queue<float>();
			this.fpsTimestampCount = (int)(this.fpsAverageTime / this.fpsAverageFrequency);
			DebugStats.budgetTimestamps = new Queue<float>();
			this.budgetTimestampCount = (int)(this.budgetAverageTime / this.budgetAverageFrequency);
		}

		// Token: 0x060020DA RID: 8410 RVA: 0x0009EA84 File Offset: 0x0009CC84
		private float GetAverageFPS()
		{
			if (DebugStats.fpsTimestamps.Count == 0)
			{
				return 0f;
			}
			float num = 0f;
			foreach (float num2 in DebugStats.fpsTimestamps)
			{
				num += num2;
			}
			num /= (float)DebugStats.fpsTimestamps.Count;
			return Mathf.Round(num);
		}

		// Token: 0x060020DB RID: 8411 RVA: 0x0009EB00 File Offset: 0x0009CD00
		private float GetAverageParticleCost()
		{
			if (DebugStats.budgetTimestamps.Count == 0)
			{
				return 0f;
			}
			float num = 0f;
			foreach (float num2 in DebugStats.budgetTimestamps)
			{
				num += num2;
			}
			num /= (float)DebugStats.budgetTimestamps.Count;
			return Mathf.Round(num);
		}

		// Token: 0x060020DC RID: 8412 RVA: 0x00017EFE File Offset: 0x000160FE
		private void FixedUpdate()
		{
			if (Input.GetKeyDown(KeyCode.F1))
			{
				DebugStats.showStats = !DebugStats.showStats;
			}
			this.statsRoot.SetActive(DebugStats.showStats);
		}

		// Token: 0x060020DD RID: 8413 RVA: 0x0009EB7C File Offset: 0x0009CD7C
		private void Update()
		{
			this.fpsStopwatch += Time.unscaledDeltaTime;
			this.fpsDisplayStopwatch += Time.unscaledDeltaTime;
			float num = 1f / Time.unscaledDeltaTime;
			if (this.fpsStopwatch >= 1f / this.fpsAverageFrequency)
			{
				this.fpsStopwatch = 0f;
				DebugStats.fpsTimestamps.Enqueue(num);
				if (DebugStats.fpsTimestamps.Count > this.fpsTimestampCount)
				{
					DebugStats.fpsTimestamps.Dequeue();
				}
			}
			if (this.fpsDisplayStopwatch > this.fpsAverageDisplayUpdateFrequency)
			{
				this.fpsDisplayStopwatch = 0f;
				float averageFPS = this.GetAverageFPS();
				this.fpsAverageText.text = string.Concat(new string[]
				{
					"FPS: ",
					Mathf.Round(num).ToString(),
					" (",
					averageFPS.ToString(),
					")"
				});
				TextMeshProUGUI textMeshProUGUI = this.fpsAverageText;
				textMeshProUGUI.text = string.Concat(new string[]
				{
					textMeshProUGUI.text,
					"\n ms: ",
					(Mathf.Round(100000f / num) / 100f).ToString(),
					"(",
					(Mathf.Round(100000f / averageFPS) / 100f).ToString(),
					")"
				});
			}
			this.budgetStopwatch += Time.unscaledDeltaTime;
			this.budgetDisplayStopwatch += Time.unscaledDeltaTime;
			float num2 = (float)VFXBudget.totalCost;
			if (this.budgetStopwatch >= 1f / this.budgetAverageFrequency)
			{
				this.budgetStopwatch = 0f;
				DebugStats.budgetTimestamps.Enqueue(num2);
				if (DebugStats.budgetTimestamps.Count > this.budgetTimestampCount)
				{
					DebugStats.budgetTimestamps.Dequeue();
				}
			}
			if (this.budgetDisplayStopwatch > 1f)
			{
				this.budgetDisplayStopwatch = 0f;
				float averageParticleCost = this.GetAverageParticleCost();
				this.budgetAverageText.text = string.Concat(new string[]
				{
					"Particle Cost: ",
					Mathf.Round(num2).ToString(),
					" (",
					averageParticleCost.ToString(),
					")"
				});
			}
			if (this.teamText)
			{
				string str = "Allies: " + TeamComponent.GetTeamMembers(TeamIndex.Player).Count + "\n";
				string str2 = "Enemies: " + TeamComponent.GetTeamMembers(TeamIndex.Monster).Count + "\n";
				string str3 = "Bosses: " + BossGroup.GetTotalBossCount() + "\n";
				string text = "Directors:";
				foreach (CombatDirector combatDirector in CombatDirector.instancesList)
				{
					string text2 = "\n==[" + combatDirector.gameObject.name + "]==";
					if (combatDirector.enabled)
					{
						string text3 = "\n - Credit: " + combatDirector.monsterCredit.ToString();
						string text4 = "\n - Target: " + (combatDirector.currentSpawnTarget ? combatDirector.currentSpawnTarget.name : "null");
						string text5 = "\n - Last Spawn Card: ";
						if (combatDirector.lastAttemptedMonsterCard != null && combatDirector.lastAttemptedMonsterCard.spawnCard)
						{
							text5 += combatDirector.lastAttemptedMonsterCard.spawnCard.name;
						}
						string text6 = "\n - Reroll Timer: " + Mathf.Round(combatDirector.monsterSpawnTimer);
						text2 = string.Concat(new string[]
						{
							text2,
							text3,
							text4,
							text5,
							text6
						});
					}
					else
					{
						text2 += " (Off)";
					}
					text = text + text2 + "\n \n";
				}
				this.teamText.text = str + str2 + str3 + text;
			}
		}

		// Token: 0x0400231D RID: 8989
		public GameObject statsRoot;

		// Token: 0x0400231E RID: 8990
		public TextMeshProUGUI fpsAverageText;

		// Token: 0x0400231F RID: 8991
		public float fpsAverageFrequency = 1f;

		// Token: 0x04002320 RID: 8992
		public float fpsAverageTime = 60f;

		// Token: 0x04002321 RID: 8993
		public float fpsAverageDisplayUpdateFrequency = 1f;

		// Token: 0x04002322 RID: 8994
		private float fpsStopwatch;

		// Token: 0x04002323 RID: 8995
		private float fpsDisplayStopwatch;

		// Token: 0x04002324 RID: 8996
		private static Queue<float> fpsTimestamps;

		// Token: 0x04002325 RID: 8997
		private int fpsTimestampCount;

		// Token: 0x04002326 RID: 8998
		public TextMeshProUGUI budgetAverageText;

		// Token: 0x04002327 RID: 8999
		public float budgetAverageFrequency = 1f;

		// Token: 0x04002328 RID: 9000
		public float budgetAverageTime = 60f;

		// Token: 0x04002329 RID: 9001
		private const float budgetAverageDisplayUpdateFrequency = 1f;

		// Token: 0x0400232A RID: 9002
		private float budgetStopwatch;

		// Token: 0x0400232B RID: 9003
		private float budgetDisplayStopwatch;

		// Token: 0x0400232C RID: 9004
		private static Queue<float> budgetTimestamps;

		// Token: 0x0400232D RID: 9005
		private int budgetTimestampCount;

		// Token: 0x0400232E RID: 9006
		private static bool showStats;

		// Token: 0x0400232F RID: 9007
		public TextMeshProUGUI teamText;
	}
}
