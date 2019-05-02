using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using KinematicCharacterController;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x0200056E RID: 1390
	public class CharacterNetworkTransform : NetworkBehaviour
	{
		// Token: 0x170002B0 RID: 688
		// (get) Token: 0x06001F01 RID: 7937 RVA: 0x00016B26 File Offset: 0x00014D26
		public static ReadOnlyCollection<CharacterNetworkTransform> readOnlyInstancesList
		{
			get
			{
				return CharacterNetworkTransform._readOnlyInstancesList;
			}
		}

		// Token: 0x170002B1 RID: 689
		// (get) Token: 0x06001F03 RID: 7939 RVA: 0x00016B36 File Offset: 0x00014D36
		// (set) Token: 0x06001F02 RID: 7938 RVA: 0x00016B2D File Offset: 0x00014D2D
		public new Transform transform { get; private set; }

		// Token: 0x170002B2 RID: 690
		// (get) Token: 0x06001F05 RID: 7941 RVA: 0x00016B47 File Offset: 0x00014D47
		// (set) Token: 0x06001F04 RID: 7940 RVA: 0x00016B3E File Offset: 0x00014D3E
		public InputBankTest inputBank { get; private set; }

		// Token: 0x170002B3 RID: 691
		// (get) Token: 0x06001F07 RID: 7943 RVA: 0x00016B58 File Offset: 0x00014D58
		// (set) Token: 0x06001F06 RID: 7942 RVA: 0x00016B4F File Offset: 0x00014D4F
		public CharacterMotor characterMotor { get; set; }

		// Token: 0x170002B4 RID: 692
		// (get) Token: 0x06001F09 RID: 7945 RVA: 0x00016B69 File Offset: 0x00014D69
		// (set) Token: 0x06001F08 RID: 7944 RVA: 0x00016B60 File Offset: 0x00014D60
		public CharacterDirection characterDirection { get; private set; }

		// Token: 0x170002B5 RID: 693
		// (get) Token: 0x06001F0B RID: 7947 RVA: 0x00016B7A File Offset: 0x00014D7A
		// (set) Token: 0x06001F0A RID: 7946 RVA: 0x00016B71 File Offset: 0x00014D71
		private Rigidbody rigidbody { get; set; }

		// Token: 0x06001F0C RID: 7948 RVA: 0x00098520 File Offset: 0x00096720
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

		// Token: 0x06001F0D RID: 7949 RVA: 0x000985D0 File Offset: 0x000967D0
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

		// Token: 0x06001F0E RID: 7950 RVA: 0x000986B8 File Offset: 0x000968B8
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

		// Token: 0x06001F0F RID: 7951 RVA: 0x00098838 File Offset: 0x00096A38
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

		// Token: 0x170002B6 RID: 694
		// (get) Token: 0x06001F10 RID: 7952 RVA: 0x00016B82 File Offset: 0x00014D82
		public float interpolationDelay
		{
			get
			{
				return this.positionTransmitInterval * this.interpolationFactor;
			}
		}

		// Token: 0x170002B7 RID: 695
		// (get) Token: 0x06001F12 RID: 7954 RVA: 0x00016B9A File Offset: 0x00014D9A
		// (set) Token: 0x06001F11 RID: 7953 RVA: 0x00016B91 File Offset: 0x00014D91
		public bool hasEffectiveAuthority { get; private set; }

		// Token: 0x06001F13 RID: 7955 RVA: 0x00016BA2 File Offset: 0x00014DA2
		private void Start()
		{
			this.newestNetSnapshot = this.BuildSnapshot();
			this.UpdateAuthority();
		}

		// Token: 0x06001F14 RID: 7956 RVA: 0x00016BB6 File Offset: 0x00014DB6
		private void OnEnable()
		{
			bool flag = CharacterNetworkTransform.instancesList.Contains(this);
			CharacterNetworkTransform.instancesList.Add(this);
			if (flag)
			{
				Debug.LogError("Instance already in list!");
			}
		}

		// Token: 0x06001F15 RID: 7957 RVA: 0x00016BDA File Offset: 0x00014DDA
		private void OnDisable()
		{
			CharacterNetworkTransform.instancesList.Remove(this);
			if (CharacterNetworkTransform.instancesList.Contains(this))
			{
				Debug.LogError("Instance was not fully removed from list!");
			}
		}

		// Token: 0x06001F16 RID: 7958 RVA: 0x00016BFF File Offset: 0x00014DFF
		private void UpdateAuthority()
		{
			this.hasEffectiveAuthority = Util.HasEffectiveAuthority(base.gameObject);
			if (this.rigidbody)
			{
				this.rigidbody.isKinematic = (!this.hasEffectiveAuthority || this.rigidbodyStartedKinematic);
			}
		}

		// Token: 0x06001F17 RID: 7959 RVA: 0x00016C3B File Offset: 0x00014E3B
		public override void OnStartAuthority()
		{
			this.UpdateAuthority();
		}

		// Token: 0x06001F18 RID: 7960 RVA: 0x00016C3B File Offset: 0x00014E3B
		public override void OnStopAuthority()
		{
			this.UpdateAuthority();
		}

		// Token: 0x06001F19 RID: 7961 RVA: 0x000988A0 File Offset: 0x00096AA0
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

		// Token: 0x06001F1C RID: 7964 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x06001F1D RID: 7965 RVA: 0x0004A818 File Offset: 0x00048A18
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x06001F1E RID: 7966 RVA: 0x000025F6 File Offset: 0x000007F6
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x040021A5 RID: 8613
		private static List<CharacterNetworkTransform> instancesList = new List<CharacterNetworkTransform>();

		// Token: 0x040021A6 RID: 8614
		private static ReadOnlyCollection<CharacterNetworkTransform> _readOnlyInstancesList = new ReadOnlyCollection<CharacterNetworkTransform>(CharacterNetworkTransform.instancesList);

		// Token: 0x040021AC RID: 8620
		[Tooltip("The delay in seconds between position network updates.")]
		public float positionTransmitInterval = 0.1f;

		// Token: 0x040021AD RID: 8621
		[HideInInspector]
		public float lastPositionTransmitTime = float.NegativeInfinity;

		// Token: 0x040021AE RID: 8622
		[Tooltip("The number of packets of buffers to have.")]
		public float interpolationFactor = 2f;

		// Token: 0x040021AF RID: 8623
		public CharacterNetworkTransform.Snapshot newestNetSnapshot;

		// Token: 0x040021B0 RID: 8624
		private List<CharacterNetworkTransform.Snapshot> snapshots = new List<CharacterNetworkTransform.Snapshot>();

		// Token: 0x040021B1 RID: 8625
		public bool debugDuplicatePositions;

		// Token: 0x040021B2 RID: 8626
		public bool debugSnapshotReceived;

		// Token: 0x040021B3 RID: 8627
		private bool rigidbodyStartedKinematic = true;

		// Token: 0x0200056F RID: 1391
		public struct Snapshot
		{
			// Token: 0x06001F1F RID: 7967 RVA: 0x000989D4 File Offset: 0x00096BD4
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

			// Token: 0x06001F20 RID: 7968 RVA: 0x00098A6C File Offset: 0x00096C6C
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

			// Token: 0x040021B5 RID: 8629
			public float serverTime;

			// Token: 0x040021B6 RID: 8630
			public Vector3 position;

			// Token: 0x040021B7 RID: 8631
			public Vector3 moveVector;

			// Token: 0x040021B8 RID: 8632
			public Vector3 aimDirection;

			// Token: 0x040021B9 RID: 8633
			public Quaternion rotation;

			// Token: 0x040021BA RID: 8634
			public bool isGrounded;

			// Token: 0x040021BB RID: 8635
			public const int maxNetworkSize = 57;
		}
	}
}
