using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000649 RID: 1609
	[RequireComponent(typeof(HudElement))]
	public class SniperRangeIndicator : MonoBehaviour
	{
		// Token: 0x06002445 RID: 9285 RVA: 0x0001A74B File Offset: 0x0001894B
		private void Awake()
		{
			this.hudElement = base.GetComponent<HudElement>();
		}

		// Token: 0x06002446 RID: 9286 RVA: 0x000ABE08 File Offset: 0x000AA008
		private void FixedUpdate()
		{
			float num = float.PositiveInfinity;
			if (this.hudElement.targetCharacterBody)
			{
				InputBankTest component = this.hudElement.targetCharacterBody.GetComponent<InputBankTest>();
				if (component)
				{
					Ray ray = new Ray(component.aimOrigin, component.aimDirection);
					RaycastHit raycastHit = default(RaycastHit);
					if (Util.CharacterRaycast(this.hudElement.targetCharacterBody.gameObject, ray, out raycastHit, float.PositiveInfinity, LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal))
					{
						num = raycastHit.distance;
					}
				}
			}
			this.label.text = "Dis: " + ((num > 999f) ? "999m" : string.Format("{0:D3}m", Mathf.FloorToInt(num)));
		}

		// Token: 0x040026F3 RID: 9971
		public TextMeshProUGUI label;

		// Token: 0x040026F4 RID: 9972
		private HudElement hudElement;
	}
}
