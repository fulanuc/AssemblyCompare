using System;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005C2 RID: 1474
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class ContextManager : MonoBehaviour
	{
		// Token: 0x06002129 RID: 8489 RVA: 0x000182D3 File Offset: 0x000164D3
		private void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
		}

		// Token: 0x0600212A RID: 8490 RVA: 0x000182E1 File Offset: 0x000164E1
		private void Start()
		{
			this.Update();
		}

		// Token: 0x0600212B RID: 8491 RVA: 0x000A0520 File Offset: 0x0009E720
		private void Update()
		{
			string text = "";
			string text2 = "";
			bool active = false;
			if (this.hud && this.hud.targetBodyObject)
			{
				InteractionDriver component = this.hud.targetBodyObject.GetComponent<InteractionDriver>();
				if (component)
				{
					GameObject gameObject = component.FindBestInteractableObject();
					if (gameObject)
					{
						PlayerCharacterMasterController component2 = this.hud.targetMaster.GetComponent<PlayerCharacterMasterController>();
						if (component2 && component2.networkUser && component2.networkUser.localUser != null)
						{
							IInteractable component3 = gameObject.GetComponent<IInteractable>();
							if (component3 != null)
							{
								string text3 = (component3.GetInteractability(component.interactor) == Interactability.Available) ? component3.GetContextString(component.interactor) : null;
								if (text3 != null)
								{
									text2 = text3;
									text = string.Format(CultureInfo.InvariantCulture, "<style=cKeyBinding>{0}</style>", Glyphs.GetGlyphString(this.eventSystemLocator, "Interact"));
									active = true;
								}
							}
						}
					}
				}
			}
			this.glyphTMP.text = text;
			this.descriptionTMP.text = text2;
			this.contextDisplay.SetActive(active);
		}

		// Token: 0x040023A4 RID: 9124
		public TextMeshProUGUI glyphTMP;

		// Token: 0x040023A5 RID: 9125
		public TextMeshProUGUI descriptionTMP;

		// Token: 0x040023A6 RID: 9126
		public GameObject contextDisplay;

		// Token: 0x040023A7 RID: 9127
		public HUD hud;

		// Token: 0x040023A8 RID: 9128
		private MPEventSystemLocator eventSystemLocator;
	}
}
