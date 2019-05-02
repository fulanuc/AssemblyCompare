using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200060A RID: 1546
	public class Nameplate : MonoBehaviour
	{
		// Token: 0x060022EF RID: 8943 RVA: 0x0001978D File Offset: 0x0001798D
		public void SetBody(CharacterBody body)
		{
			this.body = body;
		}

		// Token: 0x060022F0 RID: 8944 RVA: 0x000A7C2C File Offset: 0x000A5E2C
		private void LateUpdate()
		{
			string text = "";
			Color color = this.baseColor;
			bool flag = true;
			bool flag2 = false;
			bool flag3 = false;
			if (this.body)
			{
				text = this.body.GetDisplayName();
				flag = this.body.healthComponent.alive;
				flag2 = (!this.body.outOfCombat || !this.body.outOfDanger);
				flag3 = (this.body.healthComponent.combinedHealthFraction < HealthBar.criticallyHurtThreshold);
				CharacterMaster master = this.body.master;
				if (master)
				{
					PlayerCharacterMasterController component = master.GetComponent<PlayerCharacterMasterController>();
					if (component)
					{
						GameObject networkUserObject = component.networkUserObject;
						if (networkUserObject)
						{
							NetworkUser component2 = networkUserObject.GetComponent<NetworkUser>();
							if (component2)
							{
								text = component2.userName;
							}
						}
					}
					else
					{
						text = Language.GetString(this.body.baseNameToken);
					}
				}
			}
			color = (flag2 ? this.combatColor : this.baseColor);
			this.aliveObject.SetActive(flag);
			this.deadObject.SetActive(!flag);
			if (this.criticallyHurtSpriteRenderer)
			{
				this.criticallyHurtSpriteRenderer.enabled = (flag3 && flag);
				this.criticallyHurtSpriteRenderer.color = HealthBar.GetCriticallyHurtColor();
			}
			if (this.label)
			{
				this.label.text = text;
				this.label.color = color;
			}
			SpriteRenderer[] array = this.coloredSprites;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].color = color;
			}
		}

		// Token: 0x040025C8 RID: 9672
		public TextMeshPro label;

		// Token: 0x040025C9 RID: 9673
		private CharacterBody body;

		// Token: 0x040025CA RID: 9674
		public GameObject aliveObject;

		// Token: 0x040025CB RID: 9675
		public GameObject deadObject;

		// Token: 0x040025CC RID: 9676
		public SpriteRenderer criticallyHurtSpriteRenderer;

		// Token: 0x040025CD RID: 9677
		public SpriteRenderer[] coloredSprites;

		// Token: 0x040025CE RID: 9678
		public Color baseColor;

		// Token: 0x040025CF RID: 9679
		public Color combatColor;
	}
}
