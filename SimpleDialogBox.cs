using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace RoR2.UI
{
	// Token: 0x02000632 RID: 1586
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class SimpleDialogBox : UIBehaviour
	{
		// Token: 0x0600239E RID: 9118 RVA: 0x00019F77 File Offset: 0x00018177
		protected override void OnEnable()
		{
			base.OnEnable();
			SimpleDialogBox.instancesList.Add(this);
		}

		// Token: 0x0600239F RID: 9119 RVA: 0x00019F8A File Offset: 0x0001818A
		protected override void OnDisable()
		{
			SimpleDialogBox.instancesList.Remove(this);
			base.OnDisable();
		}

		// Token: 0x060023A0 RID: 9120 RVA: 0x000AA280 File Offset: 0x000A8480
		private static string GetString(SimpleDialogBox.TokenParamsPair pair)
		{
			string text = Language.GetString(pair.token);
			if (pair.formatParams != null && pair.formatParams.Length != 0)
			{
				text = string.Format(text, pair.formatParams);
			}
			return text;
		}

		// Token: 0x17000314 RID: 788
		// (set) Token: 0x060023A1 RID: 9121 RVA: 0x00019F9E File Offset: 0x0001819E
		public SimpleDialogBox.TokenParamsPair headerToken
		{
			set
			{
				this.headerLabel.text = SimpleDialogBox.GetString(value);
			}
		}

		// Token: 0x17000315 RID: 789
		// (set) Token: 0x060023A2 RID: 9122 RVA: 0x00019FB1 File Offset: 0x000181B1
		public SimpleDialogBox.TokenParamsPair descriptionToken
		{
			set
			{
				this.descriptionLabel.text = SimpleDialogBox.GetString(value);
			}
		}

		// Token: 0x060023A3 RID: 9123 RVA: 0x000AA2B8 File Offset: 0x000A84B8
		private MPButton AddButton(UnityAction callback, string displayToken, params object[] formatParams)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.buttonPrefab, this.buttonContainer);
			string text = Language.GetString(displayToken);
			if (formatParams != null && formatParams.Length != 0)
			{
				text = string.Format(text, formatParams);
			}
			gameObject.GetComponentInChildren<TextMeshProUGUI>().text = text;
			MPButton component = gameObject.GetComponent<MPButton>();
			component.onClick.AddListener(callback);
			gameObject.SetActive(true);
			if (!this.defaultChoice)
			{
				this.defaultChoice = component;
			}
			return component;
		}

		// Token: 0x060023A4 RID: 9124 RVA: 0x000AA328 File Offset: 0x000A8528
		public MPButton AddCommandButton(string consoleString, string displayToken, params object[] formatParams)
		{
			return this.AddButton(delegate
			{
				if (!string.IsNullOrEmpty(consoleString))
				{
					Console.instance.SubmitCmd(null, consoleString, true);
				}
				UnityEngine.Object.Destroy(this.rootObject);
			}, displayToken, formatParams);
		}

		// Token: 0x060023A5 RID: 9125 RVA: 0x00019FC4 File Offset: 0x000181C4
		public MPButton AddCancelButton(string displayToken, params object[] formatParams)
		{
			return this.AddButton(delegate
			{
				UnityEngine.Object.Destroy(this.rootObject);
			}, displayToken, formatParams);
		}

		// Token: 0x060023A6 RID: 9126 RVA: 0x000AA360 File Offset: 0x000A8560
		public MPButton AddActionButton(UnityAction action, string displayToken, params object[] formatParams)
		{
			return this.AddButton(delegate
			{
				action();
				UnityEngine.Object.Destroy(this.rootObject);
			}, displayToken, formatParams);
		}

		// Token: 0x060023A7 RID: 9127 RVA: 0x000AA398 File Offset: 0x000A8598
		protected override void Start()
		{
			base.Start();
			if (this.defaultChoice)
			{
				MPEventSystemLocator component = base.GetComponent<MPEventSystemLocator>();
				if (component.eventSystem)
				{
					component.eventSystem.SetSelectedGameObject(this.defaultChoice.gameObject);
				}
			}
		}

		// Token: 0x060023A8 RID: 9128 RVA: 0x00019FDA File Offset: 0x000181DA
		private void Update()
		{
			this.buttonContainer.gameObject.SetActive(this.buttonContainer.childCount > 0);
		}

		// Token: 0x060023A9 RID: 9129 RVA: 0x000AA3E4 File Offset: 0x000A85E4
		public static SimpleDialogBox Create(MPEventSystem owner = null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/SimpleDialogRoot"));
			if (owner)
			{
				MPEventSystemProvider component = gameObject.GetComponent<MPEventSystemProvider>();
				component.eventSystem = owner;
				component.fallBackToMainEventSystem = false;
				component.eventSystem.SetSelectedGameObject(null);
			}
			return gameObject.transform.GetComponentInChildren<SimpleDialogBox>();
		}

		// Token: 0x0400267B RID: 9851
		public static readonly List<SimpleDialogBox> instancesList = new List<SimpleDialogBox>();

		// Token: 0x0400267C RID: 9852
		public GameObject rootObject;

		// Token: 0x0400267D RID: 9853
		public GameObject buttonPrefab;

		// Token: 0x0400267E RID: 9854
		public RectTransform buttonContainer;

		// Token: 0x0400267F RID: 9855
		public TextMeshProUGUI headerLabel;

		// Token: 0x04002680 RID: 9856
		public TextMeshProUGUI descriptionLabel;

		// Token: 0x04002681 RID: 9857
		private MPButton defaultChoice;

		// Token: 0x04002682 RID: 9858
		public object[] descriptionFormatParameters;

		// Token: 0x02000633 RID: 1587
		public struct TokenParamsPair
		{
			// Token: 0x060023AD RID: 9133 RVA: 0x0001A013 File Offset: 0x00018213
			public TokenParamsPair(string token, params object[] formatParams)
			{
				this.token = token;
				this.formatParams = formatParams;
			}

			// Token: 0x04002683 RID: 9859
			public string token;

			// Token: 0x04002684 RID: 9860
			public object[] formatParams;
		}
	}
}
