using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005F4 RID: 1524
	public class HUDBossHealthBarController : MonoBehaviour
	{
		// Token: 0x0600225C RID: 8796 RVA: 0x000A5724 File Offset: 0x000A3924
		private void LateUpdate()
		{
			this.container.SetActive(HUDBossHealthBarController.shouldBeActive);
			if (HUDBossHealthBarController.shouldBeActive)
			{
				this.fillRectImage.fillAmount = HUDBossHealthBarController.totalHealthFraction;
				this.delayRectImage.fillAmount = HUDBossHealthBarController.delayedTotalHealthFraction;
				this.healthLabel.text = HUDBossHealthBarController.healthString;
				this.bossNameLabel.text = HUDBossHealthBarController.bossNameString;
				this.bossSubtitleLabel.text = HUDBossHealthBarController.bossSubtitleResolvedString;
			}
		}

		// Token: 0x0600225D RID: 8797 RVA: 0x000A5798 File Offset: 0x000A3998
		private static HUDBossHealthBarController.BossMemory GetBossMemory(CharacterMaster bossMaster)
		{
			if (!bossMaster)
			{
				return null;
			}
			for (int i = 0; i < HUDBossHealthBarController.bossMemoryList.Count; i++)
			{
				if (HUDBossHealthBarController.bossMemoryList[i].master == bossMaster)
				{
					return HUDBossHealthBarController.bossMemoryList[i];
				}
			}
			HUDBossHealthBarController.BossMemory bossMemory = new HUDBossHealthBarController.BossMemory
			{
				master = bossMaster
			};
			HUDBossHealthBarController.bossMemoryList.Add(bossMemory);
			if (HUDBossHealthBarController.bossMemoryList.Count == 1)
			{
				HUDBossHealthBarController.bossNameString = Language.GetString(bossMaster.bodyPrefab.GetComponent<CharacterBody>().baseNameToken);
				string text = bossMaster.bodyPrefab.GetComponent<CharacterBody>().GetSubtitle();
				if (text.Length == 0)
				{
					text = Language.GetString("NULL_SUBTITLE");
				}
				HUDBossHealthBarController.bossSubtitleResolvedString = "<sprite name=\"CloudLeft\" tint=1> " + text + "<sprite name=\"CloudRight\" tint=1>";
				HUDBossHealthBarController.bossNameString = Util.GetBestBodyName(bossMaster.GetBodyObject());
			}
			return bossMemory;
		}

		// Token: 0x0600225E RID: 8798 RVA: 0x000A5874 File Offset: 0x000A3A74
		private static HealthComponent GetCharacterHealthComponent(CharacterMaster master)
		{
			if (master)
			{
				GameObject bodyObject = master.GetBodyObject();
				if (bodyObject)
				{
					return bodyObject.GetComponent<HealthComponent>();
				}
			}
			return null;
		}

		// Token: 0x0600225F RID: 8799 RVA: 0x000A58A0 File Offset: 0x000A3AA0
		private static void Recalculate()
		{
			HUDBossHealthBarController.totalBossHealth = 0f;
			HUDBossHealthBarController.totalMaxBossHealth = 0f;
			if (BossGroup.instance)
			{
				ReadOnlyCollection<CharacterMaster> readOnlyMembersList = BossGroup.instance.readOnlyMembersList;
				for (int i = 0; i < readOnlyMembersList.Count; i++)
				{
					HUDBossHealthBarController.GetBossMemory(readOnlyMembersList[i]);
				}
			}
			for (int j = 0; j < HUDBossHealthBarController.bossMemoryList.Count; j++)
			{
				HUDBossHealthBarController.BossMemory bossMemory = HUDBossHealthBarController.bossMemoryList[j];
				bossMemory.UpdateLastKnownHealth();
				HUDBossHealthBarController.totalBossHealth += bossMemory.lastKnownHealth;
				HUDBossHealthBarController.totalMaxBossHealth += bossMemory.lastKnownMaxHealth;
			}
			HUDBossHealthBarController.shouldBeActive = (HUDBossHealthBarController.totalBossHealth != 0f);
			if (HUDBossHealthBarController.shouldBeActive)
			{
				HUDBossHealthBarController.totalHealthFraction = ((HUDBossHealthBarController.totalMaxBossHealth == 0f) ? 0f : Mathf.Clamp01(HUDBossHealthBarController.totalBossHealth / HUDBossHealthBarController.totalMaxBossHealth));
				HUDBossHealthBarController.delayedTotalHealthFraction = Mathf.SmoothDamp(HUDBossHealthBarController.delayedTotalHealthFraction, HUDBossHealthBarController.totalHealthFraction, ref HUDBossHealthBarController.healthFractionVelocity, 0.1f);
				HUDBossHealthBarController.healthString = Mathf.FloorToInt(HUDBossHealthBarController.totalBossHealth) + "/" + Mathf.FloorToInt(HUDBossHealthBarController.totalMaxBossHealth);
				return;
			}
			HUDBossHealthBarController.bossMemoryList.Clear();
		}

		// Token: 0x06002260 RID: 8800 RVA: 0x00019094 File Offset: 0x00017294
		private void OnEnable()
		{
			if (HUDBossHealthBarController.enabledCount++ == 0)
			{
				RoR2Application.onUpdate += HUDBossHealthBarController.Recalculate;
			}
		}

		// Token: 0x06002261 RID: 8801 RVA: 0x000190B6 File Offset: 0x000172B6
		private void OnDisable()
		{
			if (--HUDBossHealthBarController.enabledCount == 0)
			{
				RoR2Application.onUpdate -= HUDBossHealthBarController.Recalculate;
			}
		}

		// Token: 0x04002515 RID: 9493
		public GameObject container;

		// Token: 0x04002516 RID: 9494
		public Image fillRectImage;

		// Token: 0x04002517 RID: 9495
		public Image delayRectImage;

		// Token: 0x04002518 RID: 9496
		public TextMeshProUGUI healthLabel;

		// Token: 0x04002519 RID: 9497
		public TextMeshProUGUI bossNameLabel;

		// Token: 0x0400251A RID: 9498
		public TextMeshProUGUI bossSubtitleLabel;

		// Token: 0x0400251B RID: 9499
		private static List<HUDBossHealthBarController.BossMemory> bossMemoryList = new List<HUDBossHealthBarController.BossMemory>();

		// Token: 0x0400251C RID: 9500
		private static bool shouldBeActive = false;

		// Token: 0x0400251D RID: 9501
		private static float totalBossHealth = 0f;

		// Token: 0x0400251E RID: 9502
		private static float totalMaxBossHealth = 0f;

		// Token: 0x0400251F RID: 9503
		private static float totalHealthFraction;

		// Token: 0x04002520 RID: 9504
		private static float delayedTotalHealthFraction;

		// Token: 0x04002521 RID: 9505
		private static string healthString = "";

		// Token: 0x04002522 RID: 9506
		private static string bossNameString = "";

		// Token: 0x04002523 RID: 9507
		private static string bossSubtitleResolvedString = "";

		// Token: 0x04002524 RID: 9508
		private static float healthFractionVelocity = 0f;

		// Token: 0x04002525 RID: 9509
		private static int enabledCount = 0;

		// Token: 0x020005F5 RID: 1525
		private class BossMemory
		{
			// Token: 0x17000301 RID: 769
			// (get) Token: 0x06002264 RID: 8804 RVA: 0x000A5A38 File Offset: 0x000A3C38
			public HealthComponent healthComponent
			{
				get
				{
					if (!this.foundBodyObject && this.master)
					{
						GameObject bodyObject = this.master.GetBodyObject();
						if (bodyObject)
						{
							this._healthComponent = bodyObject.GetComponent<HealthComponent>();
							this.foundBodyObject = true;
						}
					}
					return this._healthComponent;
				}
			}

			// Token: 0x06002265 RID: 8805 RVA: 0x000A5A88 File Offset: 0x000A3C88
			public void UpdateLastKnownHealth()
			{
				HealthComponent healthComponent = this.healthComponent;
				if (healthComponent)
				{
					this.lastKnownHealth = Mathf.Max(healthComponent.health + healthComponent.shield, 0f);
					this.lastKnownMaxHealth = healthComponent.fullHealth + healthComponent.fullShield;
					return;
				}
				this.lastKnownHealth = 0f;
			}

			// Token: 0x04002526 RID: 9510
			public CharacterMaster master;

			// Token: 0x04002527 RID: 9511
			private bool foundBodyObject;

			// Token: 0x04002528 RID: 9512
			private HealthComponent _healthComponent;

			// Token: 0x04002529 RID: 9513
			public float lastKnownHealth;

			// Token: 0x0400252A RID: 9514
			public float lastKnownMaxHealth;

			// Token: 0x0400252B RID: 9515
			public string lastKnownName;
		}
	}
}
