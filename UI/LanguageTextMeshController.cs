using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005EC RID: 1516
	public class LanguageTextMeshController : MonoBehaviour
	{
		// Token: 0x170002F4 RID: 756
		// (get) Token: 0x0600220F RID: 8719 RVA: 0x00018CE4 File Offset: 0x00016EE4
		// (set) Token: 0x06002210 RID: 8720 RVA: 0x00018CEC File Offset: 0x00016EEC
		public string token
		{
			get
			{
				return this._token;
			}
			set
			{
				if (value != this.previousToken)
				{
					this._token = value;
					this.ResolveString();
					this.UpdateLabel();
				}
			}
		}

		// Token: 0x06002211 RID: 8721 RVA: 0x00018D0F File Offset: 0x00016F0F
		public void ResolveString()
		{
			this.previousToken = this._token;
			this.resolvedString = Language.GetString(this._token);
		}

		// Token: 0x06002212 RID: 8722 RVA: 0x00018D2E File Offset: 0x00016F2E
		private void CacheComponents()
		{
			this.text = base.GetComponent<Text>();
			this.textMesh = base.GetComponent<TextMesh>();
			this.textMeshPro = base.GetComponent<TextMeshPro>();
			this.textMeshProUGui = base.GetComponent<TextMeshProUGUI>();
		}

		// Token: 0x06002213 RID: 8723 RVA: 0x00018D60 File Offset: 0x00016F60
		private void Awake()
		{
			this.CacheComponents();
		}

		// Token: 0x06002214 RID: 8724 RVA: 0x00018D60 File Offset: 0x00016F60
		private void OnValidate()
		{
			this.CacheComponents();
		}

		// Token: 0x06002215 RID: 8725 RVA: 0x00018D68 File Offset: 0x00016F68
		private void Start()
		{
			this.ResolveString();
			this.UpdateLabel();
		}

		// Token: 0x06002216 RID: 8726 RVA: 0x000A50E4 File Offset: 0x000A32E4
		private void UpdateLabel()
		{
			if (this.text)
			{
				this.text.text = this.resolvedString;
			}
			if (this.textMesh)
			{
				this.textMesh.text = this.resolvedString;
			}
			if (this.textMeshPro)
			{
				this.textMeshPro.text = this.resolvedString;
			}
			if (this.textMeshProUGui)
			{
				this.textMeshProUGui.text = this.resolvedString;
			}
		}

		// Token: 0x0400250C RID: 9484
		[SerializeField]
		private string _token;

		// Token: 0x0400250D RID: 9485
		private string previousToken;

		// Token: 0x0400250E RID: 9486
		private string resolvedString;

		// Token: 0x0400250F RID: 9487
		private Text text;

		// Token: 0x04002510 RID: 9488
		private TextMesh textMesh;

		// Token: 0x04002511 RID: 9489
		private TextMeshPro textMeshPro;

		// Token: 0x04002512 RID: 9490
		private TextMeshProUGUI textMeshProUGui;
	}
}
