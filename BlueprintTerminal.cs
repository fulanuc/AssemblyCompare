using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200026A RID: 618
	public class BlueprintTerminal : NetworkBehaviour
	{
		// Token: 0x06000B9A RID: 2970 RVA: 0x00009372 File Offset: 0x00007572
		private void SetHasBeenPurchased(bool newHasBeenPurchased)
		{
			if (this.hasBeenPurchased != newHasBeenPurchased)
			{
				this.NetworkhasBeenPurchased = newHasBeenPurchased;
				this.Rebuild();
			}
		}

		// Token: 0x06000B9B RID: 2971 RVA: 0x0000938A File Offset: 0x0000758A
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

		// Token: 0x06000B9C RID: 2972 RVA: 0x0004C144 File Offset: 0x0004A344
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

		// Token: 0x06000B9D RID: 2973 RVA: 0x0004C19C File Offset: 0x0004A39C
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

		// Token: 0x06000B9E RID: 2974 RVA: 0x0004C330 File Offset: 0x0004A530
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

		// Token: 0x06000BA0 RID: 2976 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x06000BA1 RID: 2977 RVA: 0x0004C41C File Offset: 0x0004A61C
		// (set) Token: 0x06000BA2 RID: 2978 RVA: 0x000093B9 File Offset: 0x000075B9
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

		// Token: 0x06000BA3 RID: 2979 RVA: 0x0004C430 File Offset: 0x0004A630
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

		// Token: 0x06000BA4 RID: 2980 RVA: 0x0004C49C File Offset: 0x0004A69C
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

		// Token: 0x04000F7D RID: 3965
		[SyncVar(hook = "SetHasBeenPurchased")]
		private bool hasBeenPurchased;

		// Token: 0x04000F7E RID: 3966
		public Transform displayBaseTransform;

		// Token: 0x04000F7F RID: 3967
		[Tooltip("The unlockable string to grant")]
		public BlueprintTerminal.UnlockableOption[] unlockableOptions;

		// Token: 0x04000F80 RID: 3968
		private int unlockableChoice;

		// Token: 0x04000F81 RID: 3969
		public string unlockSoundString;

		// Token: 0x04000F82 RID: 3970
		public float idealDisplayVolume = 1.5f;

		// Token: 0x04000F83 RID: 3971
		public GameObject unlockEffect;

		// Token: 0x04000F84 RID: 3972
		private GameObject displayInstance;

		// Token: 0x0200026B RID: 619
		[Serializable]
		public struct UnlockableOption
		{
			// Token: 0x04000F85 RID: 3973
			public string unlockableName;

			// Token: 0x04000F86 RID: 3974
			public int cost;

			// Token: 0x04000F87 RID: 3975
			public float weight;
		}
	}
}
