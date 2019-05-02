using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005FE RID: 1534
	public class LanguageTextMeshController : MonoBehaviour
	{
		// Token: 0x17000307 RID: 775
		// (get) Token: 0x060022A0 RID: 8864 RVA: 0x000193DE File Offset: 0x000175DE
		// (set) Token: 0x060022A1 RID: 8865 RVA: 0x000193E6 File Offset: 0x000175E6
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

		// Token: 0x060022A2 RID: 8866 RVA: 0x00019409 File Offset: 0x00017609
		public void ResolveString()
		{
			this.previousToken = this._token;
			this.resolvedString = Language.GetString(this._token);
		}

		// Token: 0x060022A3 RID: 8867 RVA: 0x00019428 File Offset: 0x00017628
		private void CacheComponents()
		{
			this.text = base.GetComponent<Text>();
			this.textMesh = base.GetComponent<TextMesh>();
			this.textMeshPro = base.GetComponent<TextMeshPro>();
			this.textMeshProUGui = base.GetComponent<TextMeshProUGUI>();
		}

		// Token: 0x060022A4 RID: 8868 RVA: 0x0001945A File Offset: 0x0001765A
		private void Awake()
		{
			this.CacheComponents();
		}

		// Token: 0x060022A5 RID: 8869 RVA: 0x0001945A File Offset: 0x0001765A
		private void OnValidate()
		{
			this.CacheComponents();
		}

		// Token: 0x060022A6 RID: 8870 RVA: 0x00019462 File Offset: 0x00017662
		private void Start()
		{
			this.ResolveString();
			this.UpdateLabel();
		}

		// Token: 0x060022A7 RID: 8871 RVA: 0x000A6698 File Offset: 0x000A4898
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

		// Token: 0x04002561 RID: 9569
		[SerializeField]
		private string _token;

		// Token: 0x04002562 RID: 9570
		private string previousToken;

		// Token: 0x04002563 RID: 9571
		private string resolvedString;

		// Token: 0x04002564 RID: 9572
		private Text text;

		// Token: 0x04002565 RID: 9573
		private TextMesh textMesh;

		// Token: 0x04002566 RID: 9574
		private TextMeshPro textMeshPro;

		// Token: 0x04002567 RID: 9575
		private TextMeshProUGUI textMeshProUGui;
	}
}
