using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005E4 RID: 1508
	[DisallowMultipleComponent]
	public class HudElement : MonoBehaviour
	{
		// Token: 0x170002EF RID: 751
		// (get) Token: 0x060021D6 RID: 8662 RVA: 0x000189DE File Offset: 0x00016BDE
		// (set) Token: 0x060021D7 RID: 8663 RVA: 0x000189E6 File Offset: 0x00016BE6
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

		// Token: 0x170002F0 RID: 752
		// (get) Token: 0x060021D8 RID: 8664 RVA: 0x00018A15 File Offset: 0x00016C15
		// (set) Token: 0x060021D9 RID: 8665 RVA: 0x00018A1D File Offset: 0x00016C1D
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

		// Token: 0x170002F1 RID: 753
		// (get) Token: 0x060021DA RID: 8666 RVA: 0x00018A44 File Offset: 0x00016C44
		// (set) Token: 0x060021DB RID: 8667 RVA: 0x00018A4C File Offset: 0x00016C4C
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

		// Token: 0x040024D7 RID: 9431
		private HUD _hud;

		// Token: 0x040024D8 RID: 9432
		private GameObject _targetBodyObject;

		// Token: 0x040024D9 RID: 9433
		private CharacterBody _targetCharacterBody;
	}
}
