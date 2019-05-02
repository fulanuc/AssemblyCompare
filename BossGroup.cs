using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000270 RID: 624
	public class BossGroup : NetworkBehaviour
	{
		// Token: 0x170000CE RID: 206
		// (get) Token: 0x06000BAD RID: 2989 RVA: 0x00009427 File Offset: 0x00007627
		// (set) Token: 0x06000BAE RID: 2990 RVA: 0x0000942E File Offset: 0x0000762E
		public static BossGroup instance { get; private set; }

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x06000BAF RID: 2991 RVA: 0x00009436 File Offset: 0x00007636
		// (set) Token: 0x06000BB0 RID: 2992 RVA: 0x0000943E File Offset: 0x0000763E
		public ReadOnlyCollection<CharacterMaster> readOnlyMembersList { get; private set; }

		// Token: 0x06000BB1 RID: 2993 RVA: 0x0004C678 File Offset: 0x0004A878
		private void Awake()
		{
			if (NetworkServer.active)
			{
				this.onDestroyCallbacks = new List<OnDestroyCallback>();
				GlobalEventManager.onCharacterDeathGlobal += this.OnCharacterDeathCallback;
				this.rng = new Xoroshiro128Plus(Run.instance.bossRewardRng.nextUlong);
				this.bossDrops = new List<PickupIndex>();
			}
			this.readOnlyMembersList = new ReadOnlyCollection<CharacterMaster>(this.membersList);
			BossGroup.instance = this;
			BossGroup._instancesList.Add(this);
			if (BossGroup._instancesList.Count == 1)
			{
				BossGroup.instance = BossGroup._instancesList[0];
			}
		}

		// Token: 0x06000BB2 RID: 2994 RVA: 0x0004C70C File Offset: 0x0004A90C
		private void OnDestroy()
		{
			if (NetworkServer.active)
			{
				GlobalEventManager.onCharacterDeathGlobal -= this.OnCharacterDeathCallback;
			}
			for (int i = this.membersList.Count - 1; i >= 0; i--)
			{
				this.RemoveMemberAt(i);
			}
			this.onDestroyCallbacks = null;
			BossGroup._instancesList.Remove(this);
			BossGroup.instance = ((BossGroup._instancesList.Count > 0) ? BossGroup._instancesList[0] : null);
		}

		// Token: 0x06000BB3 RID: 2995 RVA: 0x00009447 File Offset: 0x00007647
		private void FixedUpdate()
		{
			this.fixedAge += Time.fixedDeltaTime;
		}

		// Token: 0x06000BB4 RID: 2996 RVA: 0x0004C784 File Offset: 0x0004A984
		[Server]
		public void AddMember(CharacterMaster memberMaster)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.BossGroup::AddMember(RoR2.CharacterMaster)' called on client");
				return;
			}
			if (this.membersList.Count >= 255)
			{
				Debug.LogFormat("Cannot add character {0} to BossGroup! Limit of {1} members already reached.", new object[]
				{
					memberMaster,
					byte.MaxValue
				});
				return;
			}
			this.membersList.Add(memberMaster);
			memberMaster.isBoss = true;
			BossGroup.totalBossCountDirty = true;
			base.SetDirtyBit(1u);
			this.onDestroyCallbacks.Add(OnDestroyCallback.AddCallback(memberMaster.gameObject, new Action<OnDestroyCallback>(this.OnMemberDestroyed)));
			Run.instance.OnServerBossAdded(this, memberMaster);
		}

		// Token: 0x06000BB5 RID: 2997 RVA: 0x0004C828 File Offset: 0x0004AA28
		[Server]
		private void OnCharacterDeathCallback(DamageReport damageReport)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.BossGroup::OnCharacterDeathCallback(RoR2.DamageReport)' called on client");
				return;
			}
			DamageInfo damageInfo = damageReport.damageInfo;
			GameObject gameObject = damageReport.victim.gameObject;
			CharacterBody component = gameObject.GetComponent<CharacterBody>();
			if (!component)
			{
				return;
			}
			CharacterMaster master = component.master;
			if (!master)
			{
				return;
			}
			DeathRewards component2 = gameObject.GetComponent<DeathRewards>();
			if (component2)
			{
				PickupIndex pickupIndex = (PickupIndex)component2.bossPickup;
				if (pickupIndex != PickupIndex.none)
				{
					this.bossDrops.Add(pickupIndex);
				}
			}
			GameObject victimMasterGameObject = master.gameObject;
			int num = this.membersList.FindIndex((CharacterMaster x) => x.gameObject == victimMasterGameObject);
			if (num >= 0)
			{
				this.RemoveMemberAt(num);
				if (!this.defeated && this.membersList.Count == 0)
				{
					Run.instance.OnServerBossKilled(true);
					if (component)
					{
						int participatingPlayerCount = Run.instance.participatingPlayerCount;
						if (participatingPlayerCount != 0 && this.dropPosition)
						{
							ItemIndex itemIndex = Run.instance.availableTier2DropList[this.rng.RangeInt(0, Run.instance.availableTier2DropList.Count)].itemIndex;
							int num2 = participatingPlayerCount * (1 + (TeleporterInteraction.instance ? TeleporterInteraction.instance.shrineBonusStacks : 0));
							float angle = 360f / (float)num2;
							Vector3 vector = Quaternion.AngleAxis((float)UnityEngine.Random.Range(0, 360), Vector3.up) * (Vector3.up * 40f + Vector3.forward * 5f);
							Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
							int i = 0;
							while (i < num2)
							{
								PickupIndex pickupIndex2 = new PickupIndex(itemIndex);
								if (this.bossDrops.Count > 0 && this.rng.nextNormalizedFloat <= this.bossDropChance)
								{
									pickupIndex2 = this.bossDrops[this.rng.RangeInt(0, this.bossDrops.Count)];
								}
								PickupDropletController.CreatePickupDroplet(pickupIndex2, this.dropPosition.position, vector);
								i++;
								vector = rotation * vector;
							}
						}
					}
					this.defeated = true;
					Action<BossGroup> action = BossGroup.onBossGroupDefeatedServer;
					if (action == null)
					{
						return;
					}
					action(this);
					return;
				}
				else
				{
					Run.instance.OnServerBossKilled(false);
				}
			}
		}

		// Token: 0x14000007 RID: 7
		// (add) Token: 0x06000BB6 RID: 2998 RVA: 0x0004CA9C File Offset: 0x0004AC9C
		// (remove) Token: 0x06000BB7 RID: 2999 RVA: 0x0004CAD0 File Offset: 0x0004ACD0
		public static event Action<BossGroup> onBossGroupDefeatedServer;

		// Token: 0x06000BB8 RID: 3000 RVA: 0x0004CB04 File Offset: 0x0004AD04
		[Server]
		private void RemoveMember(CharacterMaster memberMaster)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.BossGroup::RemoveMember(RoR2.CharacterMaster)' called on client");
				return;
			}
			int num = this.membersList.IndexOf(memberMaster);
			if (num != -1)
			{
				this.RemoveMemberAt(num);
			}
		}

		// Token: 0x06000BB9 RID: 3001 RVA: 0x0000945B File Offset: 0x0000765B
		private void RemoveMemberAt(int memberIndex)
		{
			this.membersList.RemoveAt(memberIndex);
			BossGroup.totalBossCountDirty = true;
			if (this.onDestroyCallbacks != null)
			{
				this.onDestroyCallbacks.RemoveAt(memberIndex);
			}
			base.SetDirtyBit(1u);
		}

		// Token: 0x06000BBA RID: 3002 RVA: 0x0004CB40 File Offset: 0x0004AD40
		[Server]
		public void OnMemberDestroyed(OnDestroyCallback onDestroyCallback)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.BossGroup::OnMemberDestroyed(RoR2.OnDestroyCallback)' called on client");
				return;
			}
			if (onDestroyCallback)
			{
				GameObject gameObject = onDestroyCallback.gameObject;
				CharacterMaster characterMaster = gameObject ? gameObject.GetComponent<CharacterMaster>() : null;
				if (characterMaster)
				{
					this.membersList.Remove(characterMaster);
				}
			}
		}

		// Token: 0x06000BBB RID: 3003 RVA: 0x0004CB98 File Offset: 0x0004AD98
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			uint num = base.syncVarDirtyBits;
			if (initialState)
			{
				num = 1u;
			}
			bool flag = (num & 1u) > 0u;
			writer.Write((byte)num);
			if (flag)
			{
				writer.Write((byte)this.membersList.Count);
				for (int i = 0; i < this.membersList.Count; i++)
				{
					CharacterMaster characterMaster = this.membersList[i];
					GameObject value = characterMaster ? characterMaster.gameObject : null;
					writer.Write(value);
				}
			}
			return !initialState && num > 0u;
		}

		// Token: 0x06000BBC RID: 3004 RVA: 0x0004CC18 File Offset: 0x0004AE18
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if ((reader.ReadByte() & 1) > 0)
			{
				for (int i = 0; i < this.membersList.Count; i++)
				{
					if (this.membersList[i])
					{
						this.membersList[i].isBoss = false;
					}
				}
				this.membersList.Clear();
				byte b = reader.ReadByte();
				for (byte b2 = 0; b2 < b; b2 += 1)
				{
					GameObject gameObject = reader.ReadGameObject();
					CharacterMaster characterMaster = gameObject ? gameObject.GetComponent<CharacterMaster>() : null;
					this.membersList.Add(characterMaster);
					if (characterMaster)
					{
						characterMaster.isBoss = true;
					}
				}
				BossGroup.totalBossCountDirty = true;
			}
		}

		// Token: 0x06000BBD RID: 3005 RVA: 0x0004CCD0 File Offset: 0x0004AED0
		public static int GetTotalBossCount()
		{
			if (BossGroup.totalBossCountDirty)
			{
				BossGroup.totalBossCountDirty = false;
				BossGroup.lastTotalBossCount = 0;
				for (int i = 0; i < BossGroup._instancesList.Count; i++)
				{
					BossGroup.lastTotalBossCount += BossGroup._instancesList[i].membersList.Count;
				}
			}
			return BossGroup.lastTotalBossCount;
		}

		// Token: 0x06000BC0 RID: 3008 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x04000F97 RID: 3991
		private static readonly List<BossGroup> _instancesList = new List<BossGroup>();

		// Token: 0x04000F98 RID: 3992
		public static readonly ReadOnlyCollection<BossGroup> readOnlyInstancesList = new ReadOnlyCollection<BossGroup>(BossGroup._instancesList);

		// Token: 0x04000F99 RID: 3993
		private readonly List<CharacterMaster> membersList = new List<CharacterMaster>();

		// Token: 0x04000F9B RID: 3995
		private List<OnDestroyCallback> onDestroyCallbacks;

		// Token: 0x04000F9C RID: 3996
		private bool defeated;

		// Token: 0x04000F9D RID: 3997
		private Xoroshiro128Plus rng;

		// Token: 0x04000F9E RID: 3998
		public Transform dropPosition;

		// Token: 0x04000F9F RID: 3999
		public float bossDropChance = 0.15f;

		// Token: 0x04000FA0 RID: 4000
		private const uint membersListDirtyBit = 1u;

		// Token: 0x04000FA1 RID: 4001
		public float fixedAge;

		// Token: 0x04000FA2 RID: 4002
		private List<PickupIndex> bossDrops;

		// Token: 0x04000FA4 RID: 4004
		private static int lastTotalBossCount = 0;

		// Token: 0x04000FA5 RID: 4005
		private static bool totalBossCountDirty = false;
	}
}
