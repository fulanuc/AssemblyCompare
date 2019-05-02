using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005EC RID: 1516
	public class GenericNotification : MonoBehaviour
	{
		// Token: 0x0600222C RID: 8748 RVA: 0x00018E0E File Offset: 0x0001700E
		private void Start()
		{
			this.age = 0f;
		}

		// Token: 0x0600222D RID: 8749 RVA: 0x000A4184 File Offset: 0x000A2384
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

		// Token: 0x0600222E RID: 8750 RVA: 0x000A4200 File Offset: 0x000A2400
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

		// Token: 0x0600222F RID: 8751 RVA: 0x000A4270 File Offset: 0x000A2470
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

		// Token: 0x040024AE RID: 9390
		public LanguageTextMeshController titleText;

		// Token: 0x040024AF RID: 9391
		public TextMeshProUGUI titleTMP;

		// Token: 0x040024B0 RID: 9392
		public LanguageTextMeshController descriptionText;

		// Token: 0x040024B1 RID: 9393
		public RawImage iconImage;

		// Token: 0x040024B2 RID: 9394
		public CanvasRenderer[] fadeRenderers;

		// Token: 0x040024B3 RID: 9395
		public float duration = 1.5f;

		// Token: 0x040024B4 RID: 9396
		public float fadeTime = 0.5f;

		// Token: 0x040024B5 RID: 9397
		private float age;
	}
}
