using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200061C RID: 1564
	public class Nameplate : MonoBehaviour
	{
		// Token: 0x0600237F RID: 9087 RVA: 0x00019E44 File Offset: 0x00018044
		public void SetBody(CharacterBody body)
		{
			this.body = body;
		}

		// Token: 0x06002380 RID: 9088 RVA: 0x000A92A8 File Offset: 0x000A74A8
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

		// Token: 0x04002623 RID: 9763
		public TextMeshPro label;

		// Token: 0x04002624 RID: 9764
		private CharacterBody body;

		// Token: 0x04002625 RID: 9765
		public GameObject aliveObject;

		// Token: 0x04002626 RID: 9766
		public GameObject deadObject;

		// Token: 0x04002627 RID: 9767
		public SpriteRenderer criticallyHurtSpriteRenderer;

		// Token: 0x04002628 RID: 9768
		public SpriteRenderer[] coloredSprites;

		// Token: 0x04002629 RID: 9769
		public Color baseColor;

		// Token: 0x0400262A RID: 9770
		public Color combatColor;
	}
}
