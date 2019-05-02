using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000637 RID: 1591
	[RequireComponent(typeof(HudElement))]
	public class SniperRangeIndicator : MonoBehaviour
	{
		// Token: 0x060023B5 RID: 9141 RVA: 0x0001A07D File Offset: 0x0001827D
		private void Awake()
		{
			this.hudElement = base.GetComponent<HudElement>();
		}

		// Token: 0x060023B6 RID: 9142 RVA: 0x000AA78C File Offset: 0x000A898C
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

		// Token: 0x04002698 RID: 9880
		public TextMeshProUGUI label;

		// Token: 0x04002699 RID: 9881
		private HudElement hudElement;
	}
}
