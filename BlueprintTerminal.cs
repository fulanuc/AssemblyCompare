using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200026A RID: 618
	public class BlueprintTerminal : NetworkBehaviour
	{
		// Token: 0x06000BA3 RID: 2979 RVA: 0x000093CA File Offset: 0x000075CA
		private void SetHasBeenPurchased(bool newHasBeenPurchased)
		{
			if (this.hasBeenPurchased != newHasBeenPurchased)
			{
				this.NetworkhasBeenPurchased = newHasBeenPurchased;
				this.Rebuild();
			}
		}

		// Token: 0x06000BA4 RID: 2980 RVA: 0x000093E2 File Offset: 0x000075E2
		public void Start()
		{
			if (NetworkServer.active)
			{
				this.RollChoice();
			}
			if (NetworkClient.active)
			{
				this.Rebuild();
			}
		}

		// Token: 0x06000BA5 RID: 2981 RVA: 0x0004C350 File Offset: 0x0004A550
		private void RollChoice()
		{
			WeightedSelection<int> weightedSelection = new WeightedSelection<int>(8);
			for (int i = 0; i < this.unlockableOptions.Length; i++)
			{
				weightedSelection.AddChoice(i, this.unlockableOptions[i].weight);
			}
			this.unlockableChoice = weightedSelection.Evaluate(UnityEngine.Random.value);
			this.Rebuild();
		}

		// Token: 0x06000BA6 RID: 2982 RVA: 0x0004C3A8 File Offset: 0x0004A5A8
		private void Rebuild()
		{
			BlueprintTerminal.UnlockableOption unlockableOption = this.unlockableOptions[this.unlockableChoice];
			if (this.displayInstance)
			{
				UnityEngine.Object.Destroy(this.displayInstance);
			}
			this.displayBaseTransform.gameObject.SetActive(!this.hasBeenPurchased);
			if (!this.hasBeenPurchased && this.displayBaseTransform)
			{
				Debug.Log("Found base");
				UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(unlockableOption.unlockableName);
				if (unlockableDef != null)
				{
					Debug.Log("Found unlockable");
					GameObject gameObject = Resources.Load<GameObject>(unlockableDef.displayModelPath);
					if (gameObject)
					{
						Debug.Log("Found prefab");
						this.displayInstance = UnityEngine.Object.Instantiate<GameObject>(gameObject, this.displayBaseTransform.position, this.displayBaseTransform.transform.rotation, this.displayBaseTransform);
						Renderer componentInChildren = this.displayInstance.GetComponentInChildren<Renderer>();
						float num = 1f;
						if (componentInChildren)
						{
							this.displayInstance.transform.rotation = Quaternion.identity;
							Vector3 size = componentInChildren.bounds.size;
							float f = size.x * size.y * size.z;
							num *= Mathf.Pow(this.idealDisplayVolume, 0.333333343f) / Mathf.Pow(f, 0.333333343f);
						}
						this.displayInstance.transform.localScale = new Vector3(num, num, num);
					}
				}
			}
			PurchaseInteraction component = base.GetComponent<PurchaseInteraction>();
			if (component)
			{
				component.Networkcost = unlockableOption.cost;
			}
		}

		// Token: 0x06000BA7 RID: 2983 RVA: 0x0004C53C File Offset: 0x0004A73C
		[Server]
		public void GrantUnlock(Interactor interactor)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.BlueprintTerminal::GrantUnlock(RoR2.Interactor)' called on client");
				return;
			}
			this.SetHasBeenPurchased(true);
			string unlockableName = this.unlockableOptions[this.unlockableChoice].unlockableName;
			EffectManager.instance.SpawnEffect(this.unlockEffect, new EffectData
			{
				origin = base.transform.position
			}, true);
			if (Run.instance)
			{
				Util.PlaySound(this.unlockSoundString, interactor.gameObject);
				Run.instance.GrantUnlockToSinglePlayer(unlockableName, interactor.GetComponent<CharacterBody>());
				string pickupToken = "???";
				UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(unlockableName);
				if (unlockableDef != null)
				{
					pickupToken = unlockableDef.nameToken;
				}
				Chat.SendBroadcastChat(new Chat.PlayerPickupChatMessage
				{
					subjectCharacterBodyGameObject = interactor.gameObject,
					baseToken = "PLAYER_PICKUP",
					pickupToken = pickupToken,
					pickupColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Unlockable),
					pickupQuantity = 1u
				});
			}
		}

		// Token: 0x06000BA9 RID: 2985 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x170000CE RID: 206
		// (get) Token: 0x06000BAA RID: 2986 RVA: 0x0004C628 File Offset: 0x0004A828
		// (set) Token: 0x06000BAB RID: 2987 RVA: 0x00009411 File Offset: 0x00007611
		public bool NetworkhasBeenPurchased
		{
			get
			{
				return this.hasBeenPurchased;
			}
			set
			{
				uint dirtyBit = 1u;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.SetHasBeenPurchased(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<bool>(value, ref this.hasBeenPurchased, dirtyBit);
			}
		}

		// Token: 0x06000BAC RID: 2988 RVA: 0x0004C63C File Offset: 0x0004A83C
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.hasBeenPurchased);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.hasBeenPurchased);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06000BAD RID: 2989 RVA: 0x0004C6A8 File Offset: 0x0004A8A8
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.hasBeenPurchased = reader.ReadBoolean();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.SetHasBeenPurchased(reader.ReadBoolean());
			}
		}

		// Token: 0x04000F83 RID: 3971
		[SyncVar(hook = "SetHasBeenPurchased")]
		private bool hasBeenPurchased;

		// Token: 0x04000F84 RID: 3972
		public Transform displayBaseTransform;

		// Token: 0x04000F85 RID: 3973
		[Tooltip("The unlockable string to grant")]
		public BlueprintTerminal.UnlockableOption[] unlockableOptions;

		// Token: 0x04000F86 RID: 3974
		private int unlockableChoice;

		// Token: 0x04000F87 RID: 3975
		public string unlockSoundString;

		// Token: 0x04000F88 RID: 3976
		public float idealDisplayVolume = 1.5f;

		// Token: 0x04000F89 RID: 3977
		public GameObject unlockEffect;

		// Token: 0x04000F8A RID: 3978
		private GameObject displayInstance;

		// Token: 0x0200026B RID: 619
		[Serializable]
		public struct UnlockableOption
		{
			// Token: 0x04000F8B RID: 3979
			public string unlockableName;

			// Token: 0x04000F8C RID: 3980
			public int cost;

			// Token: 0x04000F8D RID: 3981
			public float weight;
		}
	}
}
