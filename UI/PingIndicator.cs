using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200062B RID: 1579
	public class PingIndicator : MonoBehaviour
	{
		// Token: 0x060023BB RID: 9147 RVA: 0x0001A18D File Offset: 0x0001838D
		private void OnEnable()
		{
			PingIndicator.instancesList.Add(this);
		}

		// Token: 0x060023BC RID: 9148 RVA: 0x0001A19A File Offset: 0x0001839A
		private void OnDisable()
		{
			PingIndicator.instancesList.Remove(this);
		}

		// Token: 0x060023BD RID: 9149 RVA: 0x000A9E64 File Offset: 0x000A8064
		public void RebuildPing()
		{
			base.transform.rotation = Util.QuaternionSafeLookRotation(this.pingNormal);
			base.transform.parent = (this.pingTarget ? this.pingTarget.transform : null);
			base.transform.position = (this.pingTarget ? this.pingTarget.transform.position : this.pingOrigin);
			this.positionIndicator.targetTransform = (this.pingTarget ? this.pingTarget.transform : null);
			this.positionIndicator.defaultPosition = base.transform.position;
			IDisplayNameProvider componentInParent = base.GetComponentInParent<IDisplayNameProvider>();
			ModelLocator modelLocator = null;
			this.pingType = PingIndicator.PingType.Default;
			this.pingObjectScaleCurve.enabled = false;
			this.pingObjectScaleCurve.enabled = true;
			GameObject[] array = this.defaultPingGameObjects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(false);
			}
			array = this.enemyPingGameObjects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(false);
			}
			array = this.interactablePingGameObjects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(false);
			}
			if (this.pingTarget)
			{
				Debug.LogFormat("Ping target {0}", new object[]
				{
					this.pingTarget
				});
				modelLocator = this.pingTarget.GetComponent<ModelLocator>();
				if (componentInParent != null)
				{
					CharacterBody component = this.pingTarget.GetComponent<CharacterBody>();
					if (component)
					{
						this.pingType = PingIndicator.PingType.Enemy;
						base.transform.parent = component.coreTransform;
						base.transform.position = component.coreTransform.position;
					}
					else
					{
						this.pingType = PingIndicator.PingType.Interactable;
					}
				}
			}
			string bestMasterName = Util.GetBestMasterName(this.pingOwner.GetComponent<CharacterMaster>());
			string text = ((MonoBehaviour)componentInParent) ? Util.GetBestBodyName(((MonoBehaviour)componentInParent).gameObject) : "";
			this.pingText.enabled = true;
			this.pingText.text = bestMasterName;
			switch (this.pingType)
			{
			case PingIndicator.PingType.Default:
				this.pingColor = this.defaultPingColor;
				this.pingDuration = this.defaultPingDuration;
				this.pingHighlight.isOn = false;
				array = this.defaultPingGameObjects;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetActive(true);
				}
				Chat.AddMessage(string.Format(Language.GetString("PLAYER_PING_DEFAULT"), bestMasterName));
				break;
			case PingIndicator.PingType.Enemy:
				this.pingColor = this.enemyPingColor;
				this.pingDuration = this.enemyPingDuration;
				array = this.enemyPingGameObjects;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetActive(true);
				}
				if (modelLocator)
				{
					Transform modelTransform = modelLocator.modelTransform;
					if (modelTransform)
					{
						CharacterModel component2 = modelTransform.GetComponent<CharacterModel>();
						if (component2)
						{
							bool flag = false;
							foreach (CharacterModel.RendererInfo rendererInfo in component2.rendererInfos)
							{
								if (!rendererInfo.ignoreOverlays && !flag)
								{
									this.pingHighlight.highlightColor = Highlight.HighlightColor.teleporter;
									this.pingHighlight.targetRenderer = rendererInfo.renderer;
									this.pingHighlight.strength = 1f;
									this.pingHighlight.isOn = true;
									flag = true;
								}
							}
						}
					}
					Chat.AddMessage(string.Format(Language.GetString("PLAYER_PING_ENEMY"), bestMasterName, text));
				}
				break;
			case PingIndicator.PingType.Interactable:
			{
				this.pingColor = this.interactablePingColor;
				this.pingDuration = this.interactablePingDuration;
				this.pingTargetPurchaseInteraction = this.pingTarget.GetComponent<PurchaseInteraction>();
				Sprite sprite = Resources.Load<Sprite>("Textures/MiscIcons/texInventoryIconOutlined");
				SpriteRenderer component3 = this.interactablePingGameObjects[0].GetComponent<SpriteRenderer>();
				ShopTerminalBehavior component4 = this.pingTarget.GetComponent<ShopTerminalBehavior>();
				if (component4)
				{
					PickupIndex pickupIndex = component4.CurrentPickupIndex();
					text = string.Format(CultureInfo.InvariantCulture, "{0} ({1})", text, component4.pickupIndexIsHidden ? "?" : Language.GetString(pickupIndex.GetPickupNameToken()));
				}
				else if (this.pingTarget.gameObject.name.Contains("Shrine"))
				{
					sprite = Resources.Load<Sprite>("Textures/MiscIcons/texShrineIconOutlined");
				}
				else if (this.pingTarget.GetComponent<GenericPickupController>())
				{
					sprite = Resources.Load<Sprite>("Textures/MiscIcons/texLootIconOutlined");
					this.pingDuration = 60f;
				}
				else if (this.pingTarget.GetComponent<TeleporterInteraction>())
				{
					sprite = Resources.Load<Sprite>("Textures/MiscIcons/texTeleporterIconOutlined");
					this.pingDuration = 60f;
				}
				else if (this.pingTarget.GetComponent<SummonMasterBehavior>())
				{
					sprite = Resources.Load<Sprite>("Textures/MiscIcons/texDroneIconOutlined");
				}
				array = this.interactablePingGameObjects;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetActive(true);
				}
				Renderer componentInChildren;
				if (modelLocator)
				{
					componentInChildren = modelLocator.modelTransform.GetComponentInChildren<Renderer>();
				}
				else
				{
					componentInChildren = base.transform.parent.GetComponentInChildren<Renderer>();
				}
				if (componentInChildren)
				{
					this.pingHighlight.highlightColor = Highlight.HighlightColor.interactive;
					this.pingHighlight.targetRenderer = componentInChildren;
					this.pingHighlight.strength = 1f;
					this.pingHighlight.isOn = true;
				}
				component3.sprite = sprite;
				Chat.AddMessage(string.Format(Language.GetString("PLAYER_PING_INTERACTABLE"), bestMasterName, text));
				break;
			}
			}
			this.pingText.color = this.textBaseColor * this.pingColor;
			this.fixedTimer = this.pingDuration;
		}

		// Token: 0x060023BE RID: 9150 RVA: 0x000AA414 File Offset: 0x000A8614
		private void Update()
		{
			if (this.pingType == PingIndicator.PingType.Interactable && this.pingTargetPurchaseInteraction && !this.pingTargetPurchaseInteraction.available)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			this.fixedTimer -= Time.deltaTime;
			if (this.fixedTimer <= 0f)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x0400265A RID: 9818
		public PositionIndicator positionIndicator;

		// Token: 0x0400265B RID: 9819
		public TextMeshPro pingText;

		// Token: 0x0400265C RID: 9820
		public Highlight pingHighlight;

		// Token: 0x0400265D RID: 9821
		public ObjectScaleCurve pingObjectScaleCurve;

		// Token: 0x0400265E RID: 9822
		public GameObject positionIndicatorRoot;

		// Token: 0x0400265F RID: 9823
		public Color textBaseColor;

		// Token: 0x04002660 RID: 9824
		public GameObject[] defaultPingGameObjects;

		// Token: 0x04002661 RID: 9825
		public Color defaultPingColor;

		// Token: 0x04002662 RID: 9826
		public float defaultPingDuration;

		// Token: 0x04002663 RID: 9827
		public GameObject[] enemyPingGameObjects;

		// Token: 0x04002664 RID: 9828
		public Color enemyPingColor;

		// Token: 0x04002665 RID: 9829
		public float enemyPingDuration;

		// Token: 0x04002666 RID: 9830
		public GameObject[] interactablePingGameObjects;

		// Token: 0x04002667 RID: 9831
		public Color interactablePingColor;

		// Token: 0x04002668 RID: 9832
		public float interactablePingDuration;

		// Token: 0x04002669 RID: 9833
		public static List<PingIndicator> instancesList = new List<PingIndicator>();

		// Token: 0x0400266A RID: 9834
		private PingIndicator.PingType pingType;

		// Token: 0x0400266B RID: 9835
		private Color pingColor;

		// Token: 0x0400266C RID: 9836
		private float pingDuration;

		// Token: 0x0400266D RID: 9837
		private PurchaseInteraction pingTargetPurchaseInteraction;

		// Token: 0x0400266E RID: 9838
		[HideInInspector]
		public Vector3 pingOrigin;

		// Token: 0x0400266F RID: 9839
		[HideInInspector]
		public Vector3 pingNormal;

		// Token: 0x04002670 RID: 9840
		[HideInInspector]
		public GameObject pingOwner;

		// Token: 0x04002671 RID: 9841
		[HideInInspector]
		public GameObject pingTarget;

		// Token: 0x04002672 RID: 9842
		private float fixedTimer;

		// Token: 0x0200062C RID: 1580
		public enum PingType
		{
			// Token: 0x04002674 RID: 9844
			Default,
			// Token: 0x04002675 RID: 9845
			Enemy,
			// Token: 0x04002676 RID: 9846
			Interactable,
			// Token: 0x04002677 RID: 9847
			Count
		}
	}
}
