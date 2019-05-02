using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace RoR2.UI
{
	// Token: 0x02000644 RID: 1604
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class SimpleDialogBox : UIBehaviour
	{
		// Token: 0x0600242E RID: 9262 RVA: 0x0001A645 File Offset: 0x00018845
		protected override void OnEnable()
		{
			base.OnEnable();
			SimpleDialogBox.instancesList.Add(this);
		}

		// Token: 0x0600242F RID: 9263 RVA: 0x0001A658 File Offset: 0x00018858
		protected override void OnDisable()
		{
			SimpleDialogBox.instancesList.Remove(this);
			base.OnDisable();
		}

		// Token: 0x06002430 RID: 9264 RVA: 0x000AB8FC File Offset: 0x000A9AFC
		private static string GetString(SimpleDialogBox.TokenParamsPair pair)
		{
			string text = Language.GetString(pair.token);
			if (pair.formatParams != null && pair.formatParams.Length != 0)
			{
				text = string.Format(text, pair.formatParams);
			}
			return text;
		}

		// Token: 0x17000326 RID: 806
		// (set) Token: 0x06002431 RID: 9265 RVA: 0x0001A66C File Offset: 0x0001886C
		public SimpleDialogBox.TokenParamsPair headerToken
		{
			set
			{
				this.headerLabel.text = SimpleDialogBox.GetString(value);
			}
		}

		// Token: 0x17000327 RID: 807
		// (set) Token: 0x06002432 RID: 9266 RVA: 0x0001A67F File Offset: 0x0001887F
		public SimpleDialogBox.TokenParamsPair descriptionToken
		{
			set
			{
				this.descriptionLabel.text = SimpleDialogBox.GetString(value);
			}
		}

		// Token: 0x06002433 RID: 9267 RVA: 0x000AB934 File Offset: 0x000A9B34
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

		// Token: 0x06002434 RID: 9268 RVA: 0x000AB9A4 File Offset: 0x000A9BA4
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

		// Token: 0x06002435 RID: 9269 RVA: 0x0001A692 File Offset: 0x00018892
		public MPButton AddCancelButton(string displayToken, params object[] formatParams)
		{
			return this.AddButton(delegate
			{
				UnityEngine.Object.Destroy(this.rootObject);
			}, displayToken, formatParams);
		}

		// Token: 0x06002436 RID: 9270 RVA: 0x000AB9DC File Offset: 0x000A9BDC
		public MPButton AddActionButton(UnityAction action, string displayToken, params object[] formatParams)
		{
			return this.AddButton(delegate
			{
				action();
				UnityEngine.Object.Destroy(this.rootObject);
			}, displayToken, formatParams);
		}

		// Token: 0x06002437 RID: 9271 RVA: 0x000ABA14 File Offset: 0x000A9C14
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

		// Token: 0x06002438 RID: 9272 RVA: 0x0001A6A8 File Offset: 0x000188A8
		private void Update()
		{
			this.buttonContainer.gameObject.SetActive(this.buttonContainer.childCount > 0);
		}

		// Token: 0x06002439 RID: 9273 RVA: 0x000ABA60 File Offset: 0x000A9C60
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

		// Token: 0x040026D6 RID: 9942
		public static readonly List<SimpleDialogBox> instancesList = new List<SimpleDialogBox>();

		// Token: 0x040026D7 RID: 9943
		public GameObject rootObject;

		// Token: 0x040026D8 RID: 9944
		public GameObject buttonPrefab;

		// Token: 0x040026D9 RID: 9945
		public RectTransform buttonContainer;

		// Token: 0x040026DA RID: 9946
		public TextMeshProUGUI headerLabel;

		// Token: 0x040026DB RID: 9947
		public TextMeshProUGUI descriptionLabel;

		// Token: 0x040026DC RID: 9948
		private MPButton defaultChoice;

		// Token: 0x040026DD RID: 9949
		public object[] descriptionFormatParameters;

		// Token: 0x02000645 RID: 1605
		public struct TokenParamsPair
		{
			// Token: 0x0600243D RID: 9277 RVA: 0x0001A6E1 File Offset: 0x000188E1
			public TokenParamsPair(string token, params object[] formatParams)
			{
				this.token = token;
				this.formatParams = formatParams;
			}

			// Token: 0x040026DE RID: 9950
			public string token;

			// Token: 0x040026DF RID: 9951
			public object[] formatParams;
		}
	}
}
