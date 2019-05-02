using System;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005D4 RID: 1492
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class ContextManager : MonoBehaviour
	{
		// Token: 0x060021BA RID: 8634 RVA: 0x000189CD File Offset: 0x00016BCD
		private void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
		}

		// Token: 0x060021BB RID: 8635 RVA: 0x000189DB File Offset: 0x00016BDB
		private void Start()
		{
			this.Update();
		}

		// Token: 0x060021BC RID: 8636 RVA: 0x000A1AF4 File Offset: 0x0009FCF4
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

		// Token: 0x040023F8 RID: 9208
		public TextMeshProUGUI glyphTMP;

		// Token: 0x040023F9 RID: 9209
		public TextMeshProUGUI descriptionTMP;

		// Token: 0x040023FA RID: 9210
		public GameObject contextDisplay;

		// Token: 0x040023FB RID: 9211
		public HUD hud;

		// Token: 0x040023FC RID: 9212
		private MPEventSystemLocator eventSystemLocator;
	}
}
