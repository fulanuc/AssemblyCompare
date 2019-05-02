using System;
using System.Collections.Generic;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000347 RID: 839
	public class JetpackController : NetworkBehaviour
	{
		// Token: 0x0600116E RID: 4462 RVA: 0x0000D4F1 File Offset: 0x0000B6F1
		private void OnEnable()
		{
			JetpackController.instancesList.Add(this);
		}

		// Token: 0x0600116F RID: 4463 RVA: 0x0000D4FE File Offset: 0x0000B6FE
		private void OnDisable()
		{
			JetpackController.instancesList.Remove(this);
		}

		// Token: 0x06001170 RID: 4464 RVA: 0x00065CDC File Offset: 0x00063EDC
		public static JetpackController FindJetpackController(GameObject targetObject)
		{
			if (!targetObject)
			{
				return null;
			}
			for (int i = 0; i < JetpackController.instancesList.Count; i++)
			{
				if (JetpackController.instancesList[i].targetObject == targetObject)
				{
					return JetpackController.instancesList[i];
				}
			}
			return null;
		}

		// Token: 0x06001171 RID: 4465 RVA: 0x00065D30 File Offset: 0x00063F30
		private void Start()
		{
			this.SetupWings();
			if (this.targetObject)
			{
				this.targetBody = this.targetObject.GetComponent<CharacterBody>();
				this.targetCharacterMotor = this.targetObject.GetComponent<CharacterMotor>();
				this.targetInputBank = this.targetObject.GetComponent<InputBankTest>();
				this.targetHasAuthority = Util.HasEffectiveAuthority(this.targetObject);
				if (NetworkServer.active && this.targetBody)
				{
					this.targetBody.AddBuff(BuffIndex.BugWings);
				}
			}
		}

		// Token: 0x06001172 RID: 4466 RVA: 0x0000D50C File Offset: 0x0000B70C
		private void OnDestroy()
		{
			if (NetworkServer.active && this.targetBody)
			{
				this.targetBody.RemoveBuff(BuffIndex.BugWings);
			}
		}

		// Token: 0x06001173 RID: 4467 RVA: 0x0000D52F File Offset: 0x0000B72F
		public void ResetTimer()
		{
			this.stopwatch = 0f;
			if (NetworkServer.active)
			{
				this.CallRpcResetTimer();
			}
		}

		// Token: 0x06001174 RID: 4468 RVA: 0x0000D549 File Offset: 0x0000B749
		[ClientRpc]
		private void RpcResetTimer()
		{
			if (NetworkServer.active)
			{
				return;
			}
			this.ResetTimer();
		}

		// Token: 0x06001175 RID: 4469 RVA: 0x0000D559 File Offset: 0x0000B759
		private void SyncJumpInputActive(bool state)
		{
			if (this.jumpInputActive == state)
			{
				return;
			}
			if (this.targetHasAuthority)
			{
				return;
			}
			this.NetworkjumpInputActive = state;
		}

		// Token: 0x06001176 RID: 4470 RVA: 0x0000D575 File Offset: 0x0000B775
		private void SetJumpInputActive(bool state)
		{
			if (this.jumpInputActive == state)
			{
				return;
			}
			if (this.targetHasAuthority && !NetworkServer.active)
			{
				this.SendJetpackJumpState(state);
			}
			this.NetworkjumpInputActive = state;
		}

		// Token: 0x06001177 RID: 4471 RVA: 0x00065DB8 File Offset: 0x00063FB8
		[Client]
		private void SendJetpackJumpState(bool state)
		{
			if (!NetworkClient.active)
			{
				Debug.LogWarning("[Client] function 'System.Void RoR2.JetpackController::SendJetpackJumpState(System.Boolean)' called on server");
				return;
			}
			JetpackController.stateMessageBuffer.jetpackControllerObject = base.gameObject;
			JetpackController.stateMessageBuffer.state = state;
			NetworkManager.singleton.client.connection.Send(69, JetpackController.stateMessageBuffer);
		}

		// Token: 0x06001178 RID: 4472 RVA: 0x0000D59E File Offset: 0x0000B79E
		[NetworkMessageHandler(msgType = 69, client = false, server = true)]
		private static void HandleSendJumpInputActive(NetworkMessage netMsg)
		{
			netMsg.ReadMessage<JetpackController.SetJetpackJumpStateMessage>(JetpackController.stateMessageBuffer);
			GameObject jetpackControllerObject = JetpackController.stateMessageBuffer.jetpackControllerObject;
			if (jetpackControllerObject == null)
			{
				return;
			}
			JetpackController component = jetpackControllerObject.GetComponent<JetpackController>();
			if (component == null)
			{
				return;
			}
			component.SetJumpInputActive(JetpackController.stateMessageBuffer.state);
		}

		// Token: 0x06001179 RID: 4473 RVA: 0x00065E14 File Offset: 0x00064014
		private void FixedUpdate()
		{
			this.stopwatch += Time.fixedDeltaTime;
			if (NetworkServer.active && !this.wingTransform)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			if (this.targetObject)
			{
				base.transform.position = this.targetObject.transform.position;
			}
			if (this.targetObject && this.targetHasAuthority)
			{
				this.SetJumpInputActive(this.targetInputBank && this.targetInputBank.jump.down && this.stopwatch < this.duration);
				if (this.targetCharacterMotor)
				{
					if (this.jumpInputActive)
					{
						Vector3 velocity = this.targetCharacterMotor.velocity;
						velocity.y = Mathf.Max(Mathf.MoveTowards(velocity.y, this.targetBody.jumpPower / 3f, this.acceleration * Time.fixedDeltaTime), velocity.y);
						this.targetCharacterMotor.velocity = velocity;
					}
					else
					{
						this.targetCharacterMotor.velocity += new Vector3(0f, Physics.gravity.y * -0.5f * Time.fixedDeltaTime, 0f);
					}
				}
			}
			if (this.stopwatch >= this.duration)
			{
				bool flag = !this.targetCharacterMotor || !this.targetCharacterMotor.isGrounded;
				if (this.wingAnimator)
				{
					this.wingAnimator.SetBool("wingsReady", false);
				}
				this.ShowMotionLines(false);
				if (NetworkServer.active && !flag)
				{
					UnityEngine.Object.Destroy(base.gameObject);
					return;
				}
			}
			else if (this.wingAnimator)
			{
				this.wingAnimator.SetBool("wingsReady", true);
				if (this.jumpInputActive)
				{
					this.wingAnimator.SetFloat("fly.playbackRate", 10f, 0.1f, Time.fixedDeltaTime);
				}
				else
				{
					this.wingAnimator.SetFloat("fly.playbackRate", 0f, 0.2f, Time.fixedDeltaTime);
				}
				this.ShowMotionLines(this.jumpInputActive);
			}
		}

		// Token: 0x0600117A RID: 4474 RVA: 0x00066050 File Offset: 0x00064250
		private Transform FindWings()
		{
			ModelLocator component = this.targetObject.GetComponent<ModelLocator>();
			if (component)
			{
				Transform modelTransform = component.modelTransform;
				if (modelTransform)
				{
					CharacterModel component2 = modelTransform.GetComponent<CharacterModel>();
					if (component2)
					{
						List<GameObject> equipmentDisplayObjects = component2.GetEquipmentDisplayObjects(EquipmentIndex.Jetpack);
						if (equipmentDisplayObjects.Count > 0)
						{
							return equipmentDisplayObjects[0].transform;
						}
					}
				}
			}
			return null;
		}

		// Token: 0x0600117B RID: 4475 RVA: 0x000660B0 File Offset: 0x000642B0
		private void ShowMotionLines(bool showWings)
		{
			for (int i = 0; i < this.wingMotions.Length; i++)
			{
				if (this.wingMotions[i])
				{
					this.wingMotions[i].SetActive(showWings);
				}
			}
			this.wingMeshObject.SetActive(!showWings);
			if (this.hasBegunSoundLoop != showWings)
			{
				if (showWings)
				{
					if (showWings)
					{
						Util.PlaySound("Play_item_use_bugWingFlapLoop", base.gameObject);
					}
				}
				else
				{
					Util.PlaySound("Stop_item_use_bugWingFlapLoop", base.gameObject);
				}
				this.hasBegunSoundLoop = showWings;
			}
		}

		// Token: 0x0600117C RID: 4476 RVA: 0x00066138 File Offset: 0x00064338
		public void SetupWings()
		{
			this.wingTransform = this.FindWings();
			if (this.wingTransform)
			{
				this.wingAnimator = this.wingTransform.GetComponentInChildren<Animator>();
				ChildLocator component = this.wingTransform.GetComponent<ChildLocator>();
				if (this.wingAnimator)
				{
					this.wingAnimator.SetBool("wingsReady", true);
				}
				if (component)
				{
					this.wingMotions = new GameObject[4];
					this.wingMotions[0] = component.FindChild("WingMotionLargeL").gameObject;
					this.wingMotions[1] = component.FindChild("WingMotionLargeR").gameObject;
					this.wingMotions[2] = component.FindChild("WingMotionSmallL").gameObject;
					this.wingMotions[3] = component.FindChild("WingMotionSmallR").gameObject;
					this.wingMeshObject = component.FindChild("WingMesh").gameObject;
				}
			}
		}

		// Token: 0x0600117E RID: 4478 RVA: 0x0006622C File Offset: 0x0006442C
		static JetpackController()
		{
			NetworkBehaviour.RegisterRpcDelegate(typeof(JetpackController), JetpackController.kRpcRpcResetTimer, new NetworkBehaviour.CmdDelegate(JetpackController.InvokeRpcRpcResetTimer));
			NetworkCRC.RegisterBehaviour("JetpackController", 0);
		}

		// Token: 0x0600117F RID: 4479 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x17000186 RID: 390
		// (get) Token: 0x06001180 RID: 4480 RVA: 0x00066288 File Offset: 0x00064488
		// (set) Token: 0x06001181 RID: 4481 RVA: 0x0000D5D3 File Offset: 0x0000B7D3
		public GameObject NetworktargetObject
		{
			get
			{
				return this.targetObject;
			}
			set
			{
				base.SetSyncVarGameObject(value, ref this.targetObject, 1u, ref this.___targetObjectNetId);
			}
		}

		// Token: 0x17000187 RID: 391
		// (get) Token: 0x06001182 RID: 4482 RVA: 0x0006629C File Offset: 0x0006449C
		// (set) Token: 0x06001183 RID: 4483 RVA: 0x0000D5ED File Offset: 0x0000B7ED
		public bool NetworkjumpInputActive
		{
			get
			{
				return this.jumpInputActive;
			}
			set
			{
				uint dirtyBit = 2u;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.SyncJumpInputActive(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<bool>(value, ref this.jumpInputActive, dirtyBit);
			}
		}

		// Token: 0x06001184 RID: 4484 RVA: 0x0000D62C File Offset: 0x0000B82C
		protected static void InvokeRpcRpcResetTimer(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcResetTimer called on server.");
				return;
			}
			((JetpackController)obj).RpcResetTimer();
		}

		// Token: 0x06001185 RID: 4485 RVA: 0x000662B0 File Offset: 0x000644B0
		public void CallRpcResetTimer()
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcResetTimer called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)JetpackController.kRpcRpcResetTimer);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			this.SendRPCInternal(networkWriter, 0, "RpcResetTimer");
		}

		// Token: 0x06001186 RID: 4486 RVA: 0x0006631C File Offset: 0x0006451C
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.targetObject);
				writer.Write(this.jumpInputActive);
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
				writer.Write(this.targetObject);
			}
			if ((base.syncVarDirtyBits & 2u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.jumpInputActive);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06001187 RID: 4487 RVA: 0x000663C8 File Offset: 0x000645C8
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.___targetObjectNetId = reader.ReadNetworkId();
				this.jumpInputActive = reader.ReadBoolean();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.targetObject = reader.ReadGameObject();
			}
			if ((num & 2) != 0)
			{
				this.SyncJumpInputActive(reader.ReadBoolean());
			}
		}

		// Token: 0x06001188 RID: 4488 RVA: 0x0000D64F File Offset: 0x0000B84F
		public override void PreStartClient()
		{
			if (!this.___targetObjectNetId.IsEmpty())
			{
				this.NetworktargetObject = ClientScene.FindLocalObject(this.___targetObjectNetId);
			}
		}

		// Token: 0x0400156F RID: 5487
		private static readonly List<JetpackController> instancesList = new List<JetpackController>();

		// Token: 0x04001570 RID: 5488
		[SyncVar]
		public GameObject targetObject;

		// Token: 0x04001571 RID: 5489
		public float duration;

		// Token: 0x04001572 RID: 5490
		public float acceleration;

		// Token: 0x04001573 RID: 5491
		private float stopwatch;

		// Token: 0x04001574 RID: 5492
		private CharacterBody targetBody;

		// Token: 0x04001575 RID: 5493
		private Transform wingTransform;

		// Token: 0x04001576 RID: 5494
		private Animator wingAnimator;

		// Token: 0x04001577 RID: 5495
		private GameObject[] wingMotions;

		// Token: 0x04001578 RID: 5496
		private GameObject wingMeshObject;

		// Token: 0x04001579 RID: 5497
		private CharacterMotor targetCharacterMotor;

		// Token: 0x0400157A RID: 5498
		private InputBankTest targetInputBank;

		// Token: 0x0400157B RID: 5499
		private bool targetHasAuthority;

		// Token: 0x0400157C RID: 5500
		private bool hasBegunSoundLoop;

		// Token: 0x0400157D RID: 5501
		[SyncVar(hook = "SyncJumpInputActive")]
		private bool jumpInputActive;

		// Token: 0x0400157E RID: 5502
		private static JetpackController.SetJetpackJumpStateMessage stateMessageBuffer = new JetpackController.SetJetpackJumpStateMessage();

		// Token: 0x0400157F RID: 5503
		private NetworkInstanceId ___targetObjectNetId;

		// Token: 0x04001580 RID: 5504
		private static int kRpcRpcResetTimer = 1278379706;

		// Token: 0x02000348 RID: 840
		private class SetJetpackJumpStateMessage : MessageBase
		{
			// Token: 0x0600118A RID: 4490 RVA: 0x0000D673 File Offset: 0x0000B873
			public override void Serialize(NetworkWriter writer)
			{
				writer.Write(this.jetpackControllerObject);
				writer.Write(this.state);
			}

			// Token: 0x0600118B RID: 4491 RVA: 0x0000D68D File Offset: 0x0000B88D
			public override void Deserialize(NetworkReader reader)
			{
				this.jetpackControllerObject = reader.ReadGameObject();
				this.state = reader.ReadBoolean();
			}

			// Token: 0x04001581 RID: 5505
			public GameObject jetpackControllerObject;

			// Token: 0x04001582 RID: 5506
			public bool state;
		}
	}
}
