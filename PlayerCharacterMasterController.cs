using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Rewired;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200038A RID: 906
	[RequireComponent(typeof(PingerController))]
	[RequireComponent(typeof(CharacterMaster))]
	public class PlayerCharacterMasterController : NetworkBehaviour
	{
		// Token: 0x170001A5 RID: 421
		// (get) Token: 0x060012F8 RID: 4856 RVA: 0x0000E880 File Offset: 0x0000CA80
		public static ReadOnlyCollection<PlayerCharacterMasterController> instances
		{
			get
			{
				return PlayerCharacterMasterController._instancesReadOnly;
			}
		}

		// Token: 0x170001A6 RID: 422
		// (get) Token: 0x060012F9 RID: 4857 RVA: 0x0000E887 File Offset: 0x0000CA87
		// (set) Token: 0x060012FA RID: 4858 RVA: 0x0000E88F File Offset: 0x0000CA8F
		public CharacterMaster master { get; private set; }

		// Token: 0x170001A7 RID: 423
		// (get) Token: 0x060012FB RID: 4859 RVA: 0x0000E898 File Offset: 0x0000CA98
		public bool hasEffectiveAuthority
		{
			get
			{
				return this.master.hasEffectiveAuthority;
			}
		}

		// Token: 0x060012FC RID: 4860 RVA: 0x0000E8A5 File Offset: 0x0000CAA5
		private void OnSyncNetworkUserInstanceId(NetworkInstanceId value)
		{
			this.resolvedNetworkUserGameObjectInstance = null;
			this.resolvedNetworkUserInstance = null;
			this.networkUserResolved = (value == NetworkInstanceId.Invalid);
			this.NetworknetworkUserInstanceId = value;
		}

		// Token: 0x170001A8 RID: 424
		// (get) Token: 0x060012FD RID: 4861 RVA: 0x0006B290 File Offset: 0x00069490
		// (set) Token: 0x060012FE RID: 4862 RVA: 0x0006B2E8 File Offset: 0x000694E8
		public GameObject networkUserObject
		{
			get
			{
				if (!this.networkUserResolved)
				{
					this.resolvedNetworkUserGameObjectInstance = Util.FindNetworkObject(this.networkUserInstanceId);
					this.resolvedNetworkUserInstance = null;
					if (this.resolvedNetworkUserGameObjectInstance)
					{
						this.resolvedNetworkUserInstance = this.resolvedNetworkUserGameObjectInstance.GetComponent<NetworkUser>();
						this.networkUserResolved = true;
					}
				}
				return this.resolvedNetworkUserGameObjectInstance;
			}
			set
			{
				NetworkInstanceId networknetworkUserInstanceId = NetworkInstanceId.Invalid;
				this.resolvedNetworkUserGameObjectInstance = null;
				this.resolvedNetworkUserInstance = null;
				this.networkUserResolved = true;
				if (value)
				{
					NetworkIdentity component = value.GetComponent<NetworkIdentity>();
					if (component)
					{
						networknetworkUserInstanceId = component.netId;
						this.resolvedNetworkUserGameObjectInstance = value;
						this.resolvedNetworkUserInstance = value.GetComponent<NetworkUser>();
					}
				}
				this.NetworknetworkUserInstanceId = networknetworkUserInstanceId;
			}
		}

		// Token: 0x170001A9 RID: 425
		// (get) Token: 0x060012FF RID: 4863 RVA: 0x0000E8CD File Offset: 0x0000CACD
		public NetworkUser networkUser
		{
			get
			{
				if (!this.networkUserObject)
				{
					return null;
				}
				return this.resolvedNetworkUserInstance;
			}
		}

		// Token: 0x170001AA RID: 426
		// (get) Token: 0x06001300 RID: 4864 RVA: 0x0000E8E4 File Offset: 0x0000CAE4
		public bool isConnected
		{
			get
			{
				return this.networkUserObject;
			}
		}

		// Token: 0x06001301 RID: 4865 RVA: 0x0000E8F1 File Offset: 0x0000CAF1
		private void Awake()
		{
			this.master = base.GetComponent<CharacterMaster>();
			this.netid = base.GetComponent<NetworkIdentity>();
			this.pingerController = base.GetComponent<PingerController>();
		}

		// Token: 0x06001302 RID: 4866 RVA: 0x0000E917 File Offset: 0x0000CB17
		private void OnEnable()
		{
			PlayerCharacterMasterController._instances.Add(this);
			if (PlayerCharacterMasterController.onPlayerAdded != null)
			{
				PlayerCharacterMasterController.onPlayerAdded(this);
			}
		}

		// Token: 0x06001303 RID: 4867 RVA: 0x0000E936 File Offset: 0x0000CB36
		private void OnDisable()
		{
			PlayerCharacterMasterController._instances.Remove(this);
			if (PlayerCharacterMasterController.onPlayerRemoved != null)
			{
				PlayerCharacterMasterController.onPlayerRemoved(this);
			}
		}

		// Token: 0x06001304 RID: 4868 RVA: 0x0000E956 File Offset: 0x0000CB56
		private void Start()
		{
			if (NetworkServer.active && this.networkUser)
			{
				this.CallRpcIncrementRunCount();
			}
		}

		// Token: 0x06001305 RID: 4869 RVA: 0x0006B348 File Offset: 0x00069548
		[ClientRpc]
		private void RpcIncrementRunCount()
		{
			if (this.networkUser)
			{
				LocalUser localUser = this.networkUser.localUser;
				if (localUser != null)
				{
					localUser.userProfile.totalRunCount += 1u;
				}
			}
		}

		// Token: 0x06001306 RID: 4870 RVA: 0x0006B384 File Offset: 0x00069584
		private void Update()
		{
			if (this.netid.hasAuthority)
			{
				this.SetBody(this.master.GetBodyObject());
				NetworkUser networkUser = this.networkUser;
				if (this.bodyInputs && networkUser && networkUser.inputPlayer != null)
				{
					this.sprintInputPressReceived |= networkUser.inputPlayer.GetButtonDown("Sprint");
					CameraRigController cameraRigController = networkUser.cameraRigController;
					if (cameraRigController)
					{
						if (networkUser.localUser != null && !networkUser.localUser.isUIFocused)
						{
							Vector2 vector = new Vector2(networkUser.inputPlayer.GetAxis("MoveHorizontal"), networkUser.inputPlayer.GetAxis("MoveVertical"));
							float sqrMagnitude = vector.sqrMagnitude;
							if (sqrMagnitude > 1f)
							{
								vector /= Mathf.Sqrt(sqrMagnitude);
							}
							if (this.bodyIsFlier)
							{
								this.bodyInputs.moveVector = cameraRigController.transform.right * vector.x + cameraRigController.transform.forward * vector.y;
							}
							else
							{
								float y = cameraRigController.transform.eulerAngles.y;
								this.bodyInputs.moveVector = Quaternion.Euler(0f, y, 0f) * new Vector3(vector.x, 0f, vector.y);
							}
						}
						else
						{
							this.bodyInputs.moveVector = Vector3.zero;
						}
						this.bodyInputs.aimDirection = (cameraRigController.crosshairWorldPosition - this.bodyInputs.aimOrigin).normalized;
					}
					CharacterEmoteDefinitions component = this.bodyInputs.GetComponent<CharacterEmoteDefinitions>();
					if (component)
					{
						if (Input.GetKeyDown("g"))
						{
							this.bodyInputs.emoteRequest = component.FindEmoteIndex("Point");
							return;
						}
						if (Input.GetKeyDown("t"))
						{
							this.bodyInputs.emoteRequest = component.FindEmoteIndex("Surprise");
						}
					}
				}
			}
		}

		// Token: 0x06001307 RID: 4871 RVA: 0x0006B594 File Offset: 0x00069794
		private void FixedUpdate()
		{
			NetworkUser networkUser = this.networkUser;
			if (this.bodyInputs)
			{
				if (networkUser && networkUser.localUser != null && !networkUser.localUser.isUIFocused)
				{
					Player inputPlayer = networkUser.localUser.inputPlayer;
					bool flag = false;
					if (this.body)
					{
						flag = this.body.isSprinting;
						if (this.sprintInputPressReceived)
						{
							this.sprintInputPressReceived = false;
							flag = !flag;
						}
						if (flag)
						{
							Vector3 aimDirection = this.bodyInputs.aimDirection;
							aimDirection.y = 0f;
							aimDirection.Normalize();
							Vector3 moveVector = this.bodyInputs.moveVector;
							moveVector.y = 0f;
							moveVector.Normalize();
							if (Vector3.Dot(aimDirection, moveVector) < PlayerCharacterMasterController.sprintMinAimMoveDot)
							{
								flag = false;
							}
						}
					}
					this.bodyInputs.skill1.PushState(inputPlayer.GetButton("PrimarySkill"));
					this.bodyInputs.skill2.PushState(inputPlayer.GetButton("SecondarySkill"));
					this.bodyInputs.skill3.PushState(inputPlayer.GetButton("UtilitySkill"));
					this.bodyInputs.skill4.PushState(inputPlayer.GetButton("SpecialSkill"));
					this.bodyInputs.interact.PushState(inputPlayer.GetButton("Interact"));
					this.bodyInputs.jump.PushState(inputPlayer.GetButton("Jump"));
					this.bodyInputs.sprint.PushState(flag);
					this.bodyInputs.activateEquipment.PushState(inputPlayer.GetButton("Equipment"));
					this.bodyInputs.ping.PushState(inputPlayer.GetButton("Ping"));
				}
				else
				{
					this.bodyInputs.skill1.PushState(false);
					this.bodyInputs.skill2.PushState(false);
					this.bodyInputs.skill3.PushState(false);
					this.bodyInputs.skill4.PushState(false);
					this.bodyInputs.interact.PushState(false);
					this.bodyInputs.jump.PushState(false);
					this.bodyInputs.sprint.PushState(false);
					this.bodyInputs.activateEquipment.PushState(false);
					this.bodyInputs.ping.PushState(false);
				}
				this.CheckPinging();
			}
		}

		// Token: 0x06001308 RID: 4872 RVA: 0x0006B7FC File Offset: 0x000699FC
		private void CheckPinging()
		{
			if (this.hasEffectiveAuthority && this.body && this.bodyInputs && this.bodyInputs.ping.justPressed)
			{
				this.pingerController.AttemptPing(new Ray(this.bodyInputs.aimOrigin, this.bodyInputs.aimDirection), this.body.gameObject);
			}
		}

		// Token: 0x06001309 RID: 4873 RVA: 0x0006B870 File Offset: 0x00069A70
		public string GetDisplayName()
		{
			string result = "";
			if (this.networkUserObject)
			{
				NetworkUser component = this.networkUserObject.GetComponent<NetworkUser>();
				if (component)
				{
					result = component.userName;
				}
			}
			return result;
		}

		// Token: 0x0600130A RID: 4874 RVA: 0x0006B8AC File Offset: 0x00069AAC
		private void SetBody(GameObject newBody)
		{
			if (newBody)
			{
				this.body = newBody.GetComponent<CharacterBody>();
				this.bodyInputs = newBody.GetComponent<InputBankTest>();
				this.bodyIsFlier = newBody.GetComponent<RigidbodyMotor>();
				return;
			}
			this.body = null;
			this.bodyInputs = null;
			this.bodyIsFlier = false;
		}

		// Token: 0x170001AB RID: 427
		// (get) Token: 0x0600130B RID: 4875 RVA: 0x0000E972 File Offset: 0x0000CB72
		public bool preventGameOver
		{
			get
			{
				return this.master.preventGameOver;
			}
		}

		// Token: 0x0600130C RID: 4876 RVA: 0x0000E97F File Offset: 0x0000CB7F
		[Server]
		public void OnBodyDeath()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.PlayerCharacterMasterController::OnBodyDeath()' called on client");
				return;
			}
		}

		// Token: 0x0600130D RID: 4877 RVA: 0x000025F6 File Offset: 0x000007F6
		public void OnBodyStart()
		{
		}

		// Token: 0x14000019 RID: 25
		// (add) Token: 0x0600130E RID: 4878 RVA: 0x0006B900 File Offset: 0x00069B00
		// (remove) Token: 0x0600130F RID: 4879 RVA: 0x0006B934 File Offset: 0x00069B34
		public static event Action<PlayerCharacterMasterController> onPlayerAdded;

		// Token: 0x1400001A RID: 26
		// (add) Token: 0x06001310 RID: 4880 RVA: 0x0006B968 File Offset: 0x00069B68
		// (remove) Token: 0x06001311 RID: 4881 RVA: 0x0006B99C File Offset: 0x00069B9C
		public static event Action<PlayerCharacterMasterController> onPlayerRemoved;

		// Token: 0x06001312 RID: 4882 RVA: 0x0000E996 File Offset: 0x0000CB96
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void Init()
		{
			GlobalEventManager.onCharacterDeathGlobal += delegate(DamageReport damageReport)
			{
				GameObject attacker = damageReport.damageInfo.attacker;
				if (attacker)
				{
					CharacterBody component = attacker.GetComponent<CharacterBody>();
					if (component)
					{
						GameObject masterObject = component.masterObject;
						if (masterObject)
						{
							PlayerCharacterMasterController component2 = masterObject.GetComponent<PlayerCharacterMasterController>();
							if (component2 && Util.CheckRoll(1f * component2.lunarCoinChanceMultiplier, 0f, null))
							{
								PickupDropletController.CreatePickupDroplet(PickupIndex.lunarCoin1, damageReport.victim.transform.position, Vector3.up * 10f);
								component2.lunarCoinChanceMultiplier *= 0.5f;
							}
						}
					}
				}
			};
		}

		// Token: 0x06001314 RID: 4884 RVA: 0x0006B9D0 File Offset: 0x00069BD0
		static PlayerCharacterMasterController()
		{
			NetworkBehaviour.RegisterRpcDelegate(typeof(PlayerCharacterMasterController), PlayerCharacterMasterController.kRpcRpcIncrementRunCount, new NetworkBehaviour.CmdDelegate(PlayerCharacterMasterController.InvokeRpcRpcIncrementRunCount));
			NetworkCRC.RegisterBehaviour("PlayerCharacterMasterController", 0);
		}

		// Token: 0x06001315 RID: 4885 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x170001AC RID: 428
		// (get) Token: 0x06001316 RID: 4886 RVA: 0x0006BA40 File Offset: 0x00069C40
		// (set) Token: 0x06001317 RID: 4887 RVA: 0x0000E9E5 File Offset: 0x0000CBE5
		public NetworkInstanceId NetworknetworkUserInstanceId
		{
			get
			{
				return this.networkUserInstanceId;
			}
			set
			{
				base.SetSyncVar<NetworkInstanceId>(value, ref this.networkUserInstanceId, 1u);
			}
		}

		// Token: 0x06001318 RID: 4888 RVA: 0x0000E9F9 File Offset: 0x0000CBF9
		protected static void InvokeRpcRpcIncrementRunCount(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcIncrementRunCount called on server.");
				return;
			}
			((PlayerCharacterMasterController)obj).RpcIncrementRunCount();
		}

		// Token: 0x06001319 RID: 4889 RVA: 0x0006BA54 File Offset: 0x00069C54
		public void CallRpcIncrementRunCount()
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcIncrementRunCount called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)PlayerCharacterMasterController.kRpcRpcIncrementRunCount);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			this.SendRPCInternal(networkWriter, 0, "RpcIncrementRunCount");
		}

		// Token: 0x0600131A RID: 4890 RVA: 0x0006BAC0 File Offset: 0x00069CC0
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.networkUserInstanceId);
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
				writer.Write(this.networkUserInstanceId);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x0600131B RID: 4891 RVA: 0x0006BB2C File Offset: 0x00069D2C
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.networkUserInstanceId = reader.ReadNetworkId();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.networkUserInstanceId = reader.ReadNetworkId();
			}
		}

		// Token: 0x040016B7 RID: 5815
		private static List<PlayerCharacterMasterController> _instances = new List<PlayerCharacterMasterController>();

		// Token: 0x040016B8 RID: 5816
		private static ReadOnlyCollection<PlayerCharacterMasterController> _instancesReadOnly = new ReadOnlyCollection<PlayerCharacterMasterController>(PlayerCharacterMasterController._instances);

		// Token: 0x040016B9 RID: 5817
		private CharacterBody body;

		// Token: 0x040016BA RID: 5818
		private InputBankTest bodyInputs;

		// Token: 0x040016BB RID: 5819
		private bool bodyIsFlier;

		// Token: 0x040016BD RID: 5821
		private PingerController pingerController;

		// Token: 0x040016BE RID: 5822
		[SyncVar]
		private NetworkInstanceId networkUserInstanceId;

		// Token: 0x040016BF RID: 5823
		private GameObject resolvedNetworkUserGameObjectInstance;

		// Token: 0x040016C0 RID: 5824
		private bool networkUserResolved;

		// Token: 0x040016C1 RID: 5825
		private NetworkUser resolvedNetworkUserInstance;

		// Token: 0x040016C2 RID: 5826
		public float cameraMinPitch = -70f;

		// Token: 0x040016C3 RID: 5827
		public float cameraMaxPitch = 70f;

		// Token: 0x040016C4 RID: 5828
		public GameObject crosshair;

		// Token: 0x040016C5 RID: 5829
		public Vector3 crosshairPosition;

		// Token: 0x040016C6 RID: 5830
		private NetworkIdentity netid;

		// Token: 0x040016C7 RID: 5831
		private static readonly float sprintMinAimMoveDot = Mathf.Cos(1.04719758f);

		// Token: 0x040016C8 RID: 5832
		private bool sprintInputPressReceived;

		// Token: 0x040016CB RID: 5835
		private float lunarCoinChanceMultiplier = 0.5f;

		// Token: 0x040016CC RID: 5836
		private static int kRpcRpcIncrementRunCount = 1915650359;
	}
}
