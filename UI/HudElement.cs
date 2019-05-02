using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005F6 RID: 1526
	[DisallowMultipleComponent]
	public class HudElement : MonoBehaviour
	{
		// Token: 0x17000302 RID: 770
		// (get) Token: 0x06002267 RID: 8807 RVA: 0x000190D8 File Offset: 0x000172D8
		// (set) Token: 0x06002268 RID: 8808 RVA: 0x000190E0 File Offset: 0x000172E0
		public HUD hud
		{
			get
			{
				return this._hud;
			}
			set
			{
				this._hud = value;
				if (this._hud)
				{
					this.targetBodyObject = this._hud.targetBodyObject;
					return;
				}
				this.targetBodyObject = null;
			}
		}

		// Token: 0x17000303 RID: 771
		// (get) Token: 0x06002269 RID: 8809 RVA: 0x0001910F File Offset: 0x0001730F
		// (set) Token: 0x0600226A RID: 8810 RVA: 0x00019117 File Offset: 0x00017317
		public GameObject targetBodyObject
		{
			get
			{
				return this._targetBodyObject;
			}
			set
			{
				this._targetBodyObject = value;
				if (this._targetBodyObject)
				{
					this._targetCharacterBody = this._targetBodyObject.GetComponent<CharacterBody>();
				}
			}
		}

		// Token: 0x17000304 RID: 772
		// (get) Token: 0x0600226B RID: 8811 RVA: 0x0001913E File Offset: 0x0001733E
		// (set) Token: 0x0600226C RID: 8812 RVA: 0x00019146 File Offset: 0x00017346
		public CharacterBody targetCharacterBody
		{
			get
			{
				return this._targetCharacterBody;
			}
			set
			{
				this._targetCharacterBody = value;
				if (this.targetCharacterBody)
				{
					this._targetBodyObject = this.targetCharacterBody.gameObject;
				}
			}
		}

		// Token: 0x0400252C RID: 9516
		private HUD _hud;

		// Token: 0x0400252D RID: 9517
		private GameObject _targetBodyObject;

		// Token: 0x0400252E RID: 9518
		private CharacterBody _targetCharacterBody;
	}
}
