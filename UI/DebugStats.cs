using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005A0 RID: 1440
	public class DebugStats : MonoBehaviour
	{
		// Token: 0x06002048 RID: 8264 RVA: 0x000177C6 File Offset: 0x000159C6
		private void Awake()
		{
			DebugStats.fpsTimestamps = new Queue<float>();
			this.fpsTimestampCount = (int)(this.fpsAverageTime / this.fpsAverageFrequency);
			DebugStats.budgetTimestamps = new Queue<float>();
			this.budgetTimestampCount = (int)(this.budgetAverageTime / this.budgetAverageFrequency);
		}

		// Token: 0x06002049 RID: 8265 RVA: 0x0009D4B0 File Offset: 0x0009B6B0
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

		// Token: 0x0600204A RID: 8266 RVA: 0x0009D52C File Offset: 0x0009B72C
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

		// Token: 0x0600204B RID: 8267 RVA: 0x00017804 File Offset: 0x00015A04
		private void FixedUpdate()
		{
			if (Input.GetKeyDown(KeyCode.F1))
			{
				DebugStats.showStats = !DebugStats.showStats;
			}
			this.statsRoot.SetActive(DebugStats.showStats);
		}

		// Token: 0x0600204C RID: 8268 RVA: 0x0009D5A8 File Offset: 0x0009B7A8
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

		// Token: 0x040022C9 RID: 8905
		public GameObject statsRoot;

		// Token: 0x040022CA RID: 8906
		public TextMeshProUGUI fpsAverageText;

		// Token: 0x040022CB RID: 8907
		public float fpsAverageFrequency = 1f;

		// Token: 0x040022CC RID: 8908
		public float fpsAverageTime = 60f;

		// Token: 0x040022CD RID: 8909
		public float fpsAverageDisplayUpdateFrequency = 1f;

		// Token: 0x040022CE RID: 8910
		private float fpsStopwatch;

		// Token: 0x040022CF RID: 8911
		private float fpsDisplayStopwatch;

		// Token: 0x040022D0 RID: 8912
		private static Queue<float> fpsTimestamps;

		// Token: 0x040022D1 RID: 8913
		private int fpsTimestampCount;

		// Token: 0x040022D2 RID: 8914
		public TextMeshProUGUI budgetAverageText;

		// Token: 0x040022D3 RID: 8915
		public float budgetAverageFrequency = 1f;

		// Token: 0x040022D4 RID: 8916
		public float budgetAverageTime = 60f;

		// Token: 0x040022D5 RID: 8917
		private const float budgetAverageDisplayUpdateFrequency = 1f;

		// Token: 0x040022D6 RID: 8918
		private float budgetStopwatch;

		// Token: 0x040022D7 RID: 8919
		private float budgetDisplayStopwatch;

		// Token: 0x040022D8 RID: 8920
		private static Queue<float> budgetTimestamps;

		// Token: 0x040022D9 RID: 8921
		private int budgetTimestampCount;

		// Token: 0x040022DA RID: 8922
		private static bool showStats;

		// Token: 0x040022DB RID: 8923
		public TextMeshProUGUI teamText;
	}
}
