using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005E2 RID: 1506
	public class HUDBossHealthBarController : MonoBehaviour
	{
		// Token: 0x060021CB RID: 8651 RVA: 0x000A4150 File Offset: 0x000A2350
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

		// Token: 0x060021CC RID: 8652 RVA: 0x000A41C4 File Offset: 0x000A23C4
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
				EliteIndex eliteIndex = EliteCatalog.IsEquipmentElite(bossMaster.inventory.currentEquipmentIndex);
				if (eliteIndex != EliteIndex.None)
				{
					HUDBossHealthBarController.bossNameString = EliteCatalog.GetEliteDef(eliteIndex).prefix + HUDBossHealthBarController.bossNameString;
				}
			}
			return bossMemory;
		}

		// Token: 0x060021CD RID: 8653 RVA: 0x000A42C0 File Offset: 0x000A24C0
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

		// Token: 0x060021CE RID: 8654 RVA: 0x000A42EC File Offset: 0x000A24EC
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

		// Token: 0x060021CF RID: 8655 RVA: 0x0001899A File Offset: 0x00016B9A
		private void OnEnable()
		{
			if (HUDBossHealthBarController.enabledCount++ == 0)
			{
				RoR2Application.onUpdate += HUDBossHealthBarController.Recalculate;
			}
		}

		// Token: 0x060021D0 RID: 8656 RVA: 0x000189BC File Offset: 0x00016BBC
		private void OnDisable()
		{
			if (--HUDBossHealthBarController.enabledCount == 0)
			{
				RoR2Application.onUpdate -= HUDBossHealthBarController.Recalculate;
			}
		}

		// Token: 0x040024C1 RID: 9409
		public GameObject container;

		// Token: 0x040024C2 RID: 9410
		public Image fillRectImage;

		// Token: 0x040024C3 RID: 9411
		public Image delayRectImage;

		// Token: 0x040024C4 RID: 9412
		public TextMeshProUGUI healthLabel;

		// Token: 0x040024C5 RID: 9413
		public TextMeshProUGUI bossNameLabel;

		// Token: 0x040024C6 RID: 9414
		public TextMeshProUGUI bossSubtitleLabel;

		// Token: 0x040024C7 RID: 9415
		private static List<HUDBossHealthBarController.BossMemory> bossMemoryList = new List<HUDBossHealthBarController.BossMemory>();

		// Token: 0x040024C8 RID: 9416
		private static bool shouldBeActive = false;

		// Token: 0x040024C9 RID: 9417
		private static float totalBossHealth = 0f;

		// Token: 0x040024CA RID: 9418
		private static float totalMaxBossHealth = 0f;

		// Token: 0x040024CB RID: 9419
		private static float totalHealthFraction;

		// Token: 0x040024CC RID: 9420
		private static float delayedTotalHealthFraction;

		// Token: 0x040024CD RID: 9421
		private static string healthString = "";

		// Token: 0x040024CE RID: 9422
		private static string bossNameString = "";

		// Token: 0x040024CF RID: 9423
		private static string bossSubtitleResolvedString = "";

		// Token: 0x040024D0 RID: 9424
		private static float healthFractionVelocity = 0f;

		// Token: 0x040024D1 RID: 9425
		private static int enabledCount = 0;

		// Token: 0x020005E3 RID: 1507
		private class BossMemory
		{
			// Token: 0x170002EE RID: 750
			// (get) Token: 0x060021D3 RID: 8659 RVA: 0x000A4484 File Offset: 0x000A2684
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

			// Token: 0x060021D4 RID: 8660 RVA: 0x000A44D4 File Offset: 0x000A26D4
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

			// Token: 0x040024D2 RID: 9426
			public CharacterMaster master;

			// Token: 0x040024D3 RID: 9427
			private bool foundBodyObject;

			// Token: 0x040024D4 RID: 9428
			private HealthComponent _healthComponent;

			// Token: 0x040024D5 RID: 9429
			public float lastKnownHealth;

			// Token: 0x040024D6 RID: 9430
			public float lastKnownMaxHealth;
		}
	}
}
