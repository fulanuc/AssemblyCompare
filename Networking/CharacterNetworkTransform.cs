using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using KinematicCharacterController;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x0200057D RID: 1405
	public class CharacterNetworkTransform : NetworkBehaviour
	{
		// Token: 0x170002BD RID: 701
		// (get) Token: 0x06001F6B RID: 8043 RVA: 0x00017005 File Offset: 0x00015205
		public static ReadOnlyCollection<CharacterNetworkTransform> readOnlyInstancesList
		{
			get
			{
				return CharacterNetworkTransform._readOnlyInstancesList;
			}
		}

		// Token: 0x170002BE RID: 702
		// (get) Token: 0x06001F6D RID: 8045 RVA: 0x00017015 File Offset: 0x00015215
		// (set) Token: 0x06001F6C RID: 8044 RVA: 0x0001700C File Offset: 0x0001520C
		public new Transform transform { get; private set; }

		// Token: 0x170002BF RID: 703
		// (get) Token: 0x06001F6F RID: 8047 RVA: 0x00017026 File Offset: 0x00015226
		// (set) Token: 0x06001F6E RID: 8046 RVA: 0x0001701D File Offset: 0x0001521D
		public InputBankTest inputBank { get; private set; }

		// Token: 0x170002C0 RID: 704
		// (get) Token: 0x06001F71 RID: 8049 RVA: 0x00017037 File Offset: 0x00015237
		// (set) Token: 0x06001F70 RID: 8048 RVA: 0x0001702E File Offset: 0x0001522E
		public CharacterMotor characterMotor { get; set; }

		// Token: 0x170002C1 RID: 705
		// (get) Token: 0x06001F73 RID: 8051 RVA: 0x00017048 File Offset: 0x00015248
		// (set) Token: 0x06001F72 RID: 8050 RVA: 0x0001703F File Offset: 0x0001523F
		public CharacterDirection characterDirection { get; private set; }

		// Token: 0x170002C2 RID: 706
		// (get) Token: 0x06001F75 RID: 8053 RVA: 0x00017059 File Offset: 0x00015259
		// (set) Token: 0x06001F74 RID: 8052 RVA: 0x00017050 File Offset: 0x00015250
		private Rigidbody rigidbody { get; set; }

		// Token: 0x06001F76 RID: 8054 RVA: 0x0009923C File Offset: 0x0009743C
		private CharacterNetworkTransform.Snapshot CalcCurrentSnapshot(float time, float interpolationDelay)
		{
			float num = time - interpolationDelay;
			if (this.snapshots.Count < 2)
			{
				CharacterNetworkTransform.Snapshot result = (this.snapshots.Count == 0) ? this.BuildSnapshot() : this.snapshots[0];
				result.serverTime = num;
				return result;
			}
			int num2 = 0;
			while (num2 < this.snapshots.Count - 2 && (this.snapshots[num2].serverTime > num || this.snapshots[num2 + 1].serverTime < num))
			{
				num2++;
			}
			return CharacterNetworkTransform.Snapshot.Interpolate(this.snapshots[num2], this.snapshots[num2 + 1], num);
		}

		// Token: 0x06001F77 RID: 8055 RVA: 0x000992EC File Offset: 0x000974EC
		private CharacterNetworkTransform.Snapshot BuildSnapshot()
		{
			return new CharacterNetworkTransform.Snapshot
			{
				serverTime = ((GameNetworkManager)NetworkManager.singleton).serverFixedTime,
				position = this.transform.position,
				moveVector = (this.inputBank ? this.inputBank.moveVector : Vector3.zero),
				aimDirection = (this.inputBank ? this.inputBank.aimDirection : Vector3.zero),
				rotation = (this.characterDirection ? Quaternion.Euler(0f, this.characterDirection.yaw, 0f) : this.transform.rotation),
				isGrounded = (this.characterMotor && this.characterMotor.isGrounded)
			};
		}

		// Token: 0x06001F78 RID: 8056 RVA: 0x000993D4 File Offset: 0x000975D4
		public void PushSnapshot(CharacterNetworkTransform.Snapshot newSnapshot)
		{
			if (this.debugSnapshotReceived)
			{
				Debug.LogFormat("{0} CharacterNetworkTransform snapshot received.", new object[]
				{
					base.gameObject
				});
			}
			if (this.snapshots.Count > 0 && newSnapshot.serverTime == this.snapshots[this.snapshots.Count - 1].serverTime)
			{
				Debug.Log("Received duplicate time!");
			}
			if (this.debugDuplicatePositions && this.snapshots.Count > 0 && newSnapshot.position == this.snapshots[this.snapshots.Count - 1].position)
			{
				Debug.Log("Received duplicate position!");
			}
			if (((this.snapshots.Count > 0) ? this.snapshots[this.snapshots.Count - 1].serverTime : float.NegativeInfinity) < newSnapshot.serverTime)
			{
				this.snapshots.Add(newSnapshot);
				this.newestNetSnapshot = newSnapshot;
				Debug.DrawLine(newSnapshot.position + Vector3.up, newSnapshot.position + Vector3.down, Color.white, 0.25f);
			}
			float num = ((GameNetworkManager)NetworkManager.singleton).serverFixedTime - this.interpolationDelay * 3f;
			while (this.snapshots.Count > 2 && this.snapshots[1].serverTime < num)
			{
				this.snapshots.RemoveAt(0);
			}
		}

		// Token: 0x06001F79 RID: 8057 RVA: 0x00099554 File Offset: 0x00097754
		private void Awake()
		{
			this.transform = base.transform;
			this.inputBank = base.GetComponent<InputBankTest>();
			this.characterMotor = base.GetComponent<CharacterMotor>();
			this.characterDirection = base.GetComponent<CharacterDirection>();
			this.rigidbody = base.GetComponent<Rigidbody>();
			if (this.rigidbody)
			{
				this.rigidbodyStartedKinematic = this.rigidbody.isKinematic;
			}
		}

		// Token: 0x170002C3 RID: 707
		// (get) Token: 0x06001F7A RID: 8058 RVA: 0x00017061 File Offset: 0x00015261
		public float interpolationDelay
		{
			get
			{
				return this.positionTransmitInterval * this.interpolationFactor;
			}
		}

		// Token: 0x170002C4 RID: 708
		// (get) Token: 0x06001F7C RID: 8060 RVA: 0x00017079 File Offset: 0x00015279
		// (set) Token: 0x06001F7B RID: 8059 RVA: 0x00017070 File Offset: 0x00015270
		public bool hasEffectiveAuthority { get; private set; }

		// Token: 0x06001F7D RID: 8061 RVA: 0x00017081 File Offset: 0x00015281
		private void Start()
		{
			this.newestNetSnapshot = this.BuildSnapshot();
			this.UpdateAuthority();
		}

		// Token: 0x06001F7E RID: 8062 RVA: 0x00017095 File Offset: 0x00015295
		private void OnEnable()
		{
			bool flag = CharacterNetworkTransform.instancesList.Contains(this);
			CharacterNetworkTransform.instancesList.Add(this);
			if (flag)
			{
				Debug.LogError("Instance already in list!");
			}
		}

		// Token: 0x06001F7F RID: 8063 RVA: 0x000170B9 File Offset: 0x000152B9
		private void OnDisable()
		{
			CharacterNetworkTransform.instancesList.Remove(this);
			if (CharacterNetworkTransform.instancesList.Contains(this))
			{
				Debug.LogError("Instance was not fully removed from list!");
			}
		}

		// Token: 0x06001F80 RID: 8064 RVA: 0x000170DE File Offset: 0x000152DE
		private void UpdateAuthority()
		{
			this.hasEffectiveAuthority = Util.HasEffectiveAuthority(base.gameObject);
			if (this.rigidbody)
			{
				this.rigidbody.isKinematic = (!this.hasEffectiveAuthority || this.rigidbodyStartedKinematic);
			}
		}

		// Token: 0x06001F81 RID: 8065 RVA: 0x0001711A File Offset: 0x0001531A
		public override void OnStartAuthority()
		{
			this.UpdateAuthority();
		}

		// Token: 0x06001F82 RID: 8066 RVA: 0x0001711A File Offset: 0x0001531A
		public override void OnStopAuthority()
		{
			this.UpdateAuthority();
		}

		// Token: 0x06001F83 RID: 8067 RVA: 0x000995BC File Offset: 0x000977BC
		private void FixedUpdate()
		{
			if (this.hasEffectiveAuthority)
			{
				this.newestNetSnapshot = this.BuildSnapshot();
				return;
			}
			CharacterNetworkTransform.Snapshot snapshot = this.CalcCurrentSnapshot(GameNetworkManager.singleton.serverFixedTime, this.interpolationDelay);
			if (!this.characterMotor)
			{
				if (this.rigidbodyStartedKinematic)
				{
					this.transform.position = snapshot.position;
				}
				else
				{
					this.rigidbody.MovePosition(snapshot.position);
				}
			}
			if (this.inputBank)
			{
				this.inputBank.moveVector = snapshot.moveVector;
				this.inputBank.aimDirection = snapshot.aimDirection;
			}
			if (this.characterMotor)
			{
				this.characterMotor.netIsGrounded = snapshot.isGrounded;
				KinematicCharacterMotor motor = this.characterMotor.Motor;
				if (motor != null)
				{
					motor.MoveCharacter(snapshot.position);
				}
			}
			if (this.characterDirection)
			{
				this.characterDirection.yaw = snapshot.rotation.eulerAngles.y;
				return;
			}
			if (this.rigidbodyStartedKinematic)
			{
				this.transform.rotation = snapshot.rotation;
				return;
			}
			this.rigidbody.MoveRotation(snapshot.rotation);
		}

		// Token: 0x06001F86 RID: 8070 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x06001F87 RID: 8071 RVA: 0x0004AA24 File Offset: 0x00048C24
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x06001F88 RID: 8072 RVA: 0x000025DA File Offset: 0x000007DA
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x040021E3 RID: 8675
		private static List<CharacterNetworkTransform> instancesList = new List<CharacterNetworkTransform>();

		// Token: 0x040021E4 RID: 8676
		private static ReadOnlyCollection<CharacterNetworkTransform> _readOnlyInstancesList = new ReadOnlyCollection<CharacterNetworkTransform>(CharacterNetworkTransform.instancesList);

		// Token: 0x040021EA RID: 8682
		[Tooltip("The delay in seconds between position network updates.")]
		public float positionTransmitInterval = 0.1f;

		// Token: 0x040021EB RID: 8683
		[HideInInspector]
		public float lastPositionTransmitTime = float.NegativeInfinity;

		// Token: 0x040021EC RID: 8684
		[Tooltip("The number of packets of buffers to have.")]
		public float interpolationFactor = 2f;

		// Token: 0x040021ED RID: 8685
		public CharacterNetworkTransform.Snapshot newestNetSnapshot;

		// Token: 0x040021EE RID: 8686
		private List<CharacterNetworkTransform.Snapshot> snapshots = new List<CharacterNetworkTransform.Snapshot>();

		// Token: 0x040021EF RID: 8687
		public bool debugDuplicatePositions;

		// Token: 0x040021F0 RID: 8688
		public bool debugSnapshotReceived;

		// Token: 0x040021F1 RID: 8689
		private bool rigidbodyStartedKinematic = true;

		// Token: 0x0200057E RID: 1406
		public struct Snapshot
		{
			// Token: 0x06001F89 RID: 8073 RVA: 0x000996F0 File Offset: 0x000978F0
			public static CharacterNetworkTransform.Snapshot Lerp(CharacterNetworkTransform.Snapshot a, CharacterNetworkTransform.Snapshot b, float t)
			{
				return new CharacterNetworkTransform.Snapshot
				{
					position = Vector3.Lerp(a.position, b.position, t),
					moveVector = Vector3.Lerp(a.moveVector, b.moveVector, t),
					aimDirection = Vector3.Slerp(a.aimDirection, b.moveVector, t),
					rotation = Quaternion.Lerp(a.rotation, b.rotation, t),
					isGrounded = ((t > 0.5f) ? b.isGrounded : a.isGrounded)
				};
			}

			// Token: 0x06001F8A RID: 8074 RVA: 0x00099788 File Offset: 0x00097988
			public static CharacterNetworkTransform.Snapshot Interpolate(CharacterNetworkTransform.Snapshot a, CharacterNetworkTransform.Snapshot b, float serverTime)
			{
				float num = (serverTime - a.serverTime) / (b.serverTime - a.serverTime);
				return new CharacterNetworkTransform.Snapshot
				{
					serverTime = serverTime,
					position = Vector3.LerpUnclamped(a.position, b.position, num),
					moveVector = Vector3.Lerp(a.moveVector, b.moveVector, num),
					aimDirection = Vector3.Slerp(a.aimDirection, b.aimDirection, num),
					rotation = Quaternion.Lerp(a.rotation, b.rotation, num),
					isGrounded = ((num > 0.5f) ? b.isGrounded : a.isGrounded)
				};
			}

			// Token: 0x040021F3 RID: 8691
			public float serverTime;

			// Token: 0x040021F4 RID: 8692
			public Vector3 position;

			// Token: 0x040021F5 RID: 8693
			public Vector3 moveVector;

			// Token: 0x040021F6 RID: 8694
			public Vector3 aimDirection;

			// Token: 0x040021F7 RID: 8695
			public Quaternion rotation;

			// Token: 0x040021F8 RID: 8696
			public bool isGrounded;

			// Token: 0x040021F9 RID: 8697
			public const int maxNetworkSize = 57;
		}
	}
}
