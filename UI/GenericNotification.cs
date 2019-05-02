using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005DA RID: 1498
	public class GenericNotification : MonoBehaviour
	{
		// Token: 0x0600219B RID: 8603 RVA: 0x00018714 File Offset: 0x00016914
		private void Start()
		{
			this.age = 0f;
		}

		// Token: 0x0600219C RID: 8604 RVA: 0x000A2BB0 File Offset: 0x000A0DB0
		private void Update()
		{
			this.age += Time.deltaTime;
			float num = Mathf.Clamp01((this.age - (this.duration - this.fadeTime)) / this.fadeTime);
			float alpha = 1f - num;
			for (int i = 0; i < this.fadeRenderers.Length; i++)
			{
				this.fadeRenderers[i].SetAlpha(alpha);
			}
			if (num >= 1f)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x0600219D RID: 8605 RVA: 0x000A2C2C File Offset: 0x000A0E2C
		public void SetItem(ItemIndex itemIndex)
		{
			ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
			this.titleText.token = itemDef.nameToken;
			this.descriptionText.token = itemDef.pickupToken;
			if (itemDef.pickupIconPath != null)
			{
				this.iconImage.texture = Resources.Load<Texture>(itemDef.pickupIconPath);
			}
			this.titleTMP.color = ColorCatalog.GetColor(itemDef.colorIndex);
		}

		// Token: 0x0600219E RID: 8606 RVA: 0x000A2C9C File Offset: 0x000A0E9C
		public void SetEquipment(EquipmentIndex equipmentIndex)
		{
			EquipmentDef equipmentDef = EquipmentCatalog.GetEquipmentDef(equipmentIndex);
			this.titleText.token = equipmentDef.nameToken;
			this.descriptionText.token = equipmentDef.pickupToken;
			if (equipmentDef.pickupIconPath != null)
			{
				this.iconImage.texture = Resources.Load<Texture>(equipmentDef.pickupIconPath);
			}
			this.titleTMP.color = ColorCatalog.GetColor(equipmentDef.colorIndex);
		}

		// Token: 0x0400245A RID: 9306
		public LanguageTextMeshController titleText;

		// Token: 0x0400245B RID: 9307
		public TextMeshProUGUI titleTMP;

		// Token: 0x0400245C RID: 9308
		public LanguageTextMeshController descriptionText;

		// Token: 0x0400245D RID: 9309
		public RawImage iconImage;

		// Token: 0x0400245E RID: 9310
		public CanvasRenderer[] fadeRenderers;

		// Token: 0x0400245F RID: 9311
		public float duration = 1.5f;

		// Token: 0x04002460 RID: 9312
		public float fadeTime = 0.5f;

		// Token: 0x04002461 RID: 9313
		private float age;
	}
}
