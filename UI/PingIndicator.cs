using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000619 RID: 1561
	public class PingIndicator : MonoBehaviour
	{
		// Token: 0x0600232B RID: 9003 RVA: 0x00019ABF File Offset: 0x00017CBF
		private void OnEnable()
		{
			PingIndicator.instancesList.Add(this);
		}

		// Token: 0x0600232C RID: 9004 RVA: 0x00019ACC File Offset: 0x00017CCC
		private void OnDisable()
		{
			PingIndicator.instancesList.Remove(this);
		}

		// Token: 0x0600232D RID: 9005 RVA: 0x000A87E8 File Offset: 0x000A69E8
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
			string displayName = this.pingOwner.GetComponent<PlayerCharacterMasterController>().GetDisplayName();
			string text = (componentInParent != null) ? componentInParent.GetDisplayName() : "";
			this.pingText.enabled = true;
			this.pingText.text = displayName;
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
				Chat.AddMessage(string.Format(Language.GetString("PLAYER_PING_DEFAULT"), displayName));
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
					Chat.AddMessage(string.Format(Language.GetString("PLAYER_PING_ENEMY"), displayName, text));
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
				base.transform.parent.GetComponentInChildren<Renderer>();
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
				Chat.AddMessage(string.Format(Language.GetString("PLAYER_PING_INTERACTABLE"), displayName, text));
				break;
			}
			}
			this.pingText.color = this.textBaseColor * this.pingColor;
			this.fixedTimer = this.pingDuration;
		}

		// Token: 0x0600232E RID: 9006 RVA: 0x000A8D98 File Offset: 0x000A6F98
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

		// Token: 0x040025FF RID: 9727
		public PositionIndicator positionIndicator;

		// Token: 0x04002600 RID: 9728
		public TextMeshPro pingText;

		// Token: 0x04002601 RID: 9729
		public Highlight pingHighlight;

		// Token: 0x04002602 RID: 9730
		public ObjectScaleCurve pingObjectScaleCurve;

		// Token: 0x04002603 RID: 9731
		public GameObject positionIndicatorRoot;

		// Token: 0x04002604 RID: 9732
		public Color textBaseColor;

		// Token: 0x04002605 RID: 9733
		public GameObject[] defaultPingGameObjects;

		// Token: 0x04002606 RID: 9734
		public Color defaultPingColor;

		// Token: 0x04002607 RID: 9735
		public float defaultPingDuration;

		// Token: 0x04002608 RID: 9736
		public GameObject[] enemyPingGameObjects;

		// Token: 0x04002609 RID: 9737
		public Color enemyPingColor;

		// Token: 0x0400260A RID: 9738
		public float enemyPingDuration;

		// Token: 0x0400260B RID: 9739
		public GameObject[] interactablePingGameObjects;

		// Token: 0x0400260C RID: 9740
		public Color interactablePingColor;

		// Token: 0x0400260D RID: 9741
		public float interactablePingDuration;

		// Token: 0x0400260E RID: 9742
		public static List<PingIndicator> instancesList = new List<PingIndicator>();

		// Token: 0x0400260F RID: 9743
		private PingIndicator.PingType pingType;

		// Token: 0x04002610 RID: 9744
		private Color pingColor;

		// Token: 0x04002611 RID: 9745
		private float pingDuration;

		// Token: 0x04002612 RID: 9746
		private PurchaseInteraction pingTargetPurchaseInteraction;

		// Token: 0x04002613 RID: 9747
		[HideInInspector]
		public Vector3 pingOrigin;

		// Token: 0x04002614 RID: 9748
		[HideInInspector]
		public Vector3 pingNormal;

		// Token: 0x04002615 RID: 9749
		[HideInInspector]
		public GameObject pingOwner;

		// Token: 0x04002616 RID: 9750
		[HideInInspector]
		public GameObject pingTarget;

		// Token: 0x04002617 RID: 9751
		private float fixedTimer;

		// Token: 0x0200061A RID: 1562
		public enum PingType
		{
			// Token: 0x04002619 RID: 9753
			Default,
			// Token: 0x0400261A RID: 9754
			Enemy,
			// Token: 0x0400261B RID: 9755
			Interactable,
			// Token: 0x0400261C RID: 9756
			Count
		}
	}
}
