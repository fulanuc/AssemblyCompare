using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using RoR2.ConVar;
using RoR2.Networking;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000214 RID: 532
	public static class Chat
	{
		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x06000A57 RID: 2647 RVA: 0x00008493 File Offset: 0x00006693
		public static ReadOnlyCollection<string> readOnlyLog
		{
			get
			{
				return Chat._readOnlyLog;
			}
		}

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x06000A58 RID: 2648 RVA: 0x00048200 File Offset: 0x00046400
		// (remove) Token: 0x06000A59 RID: 2649 RVA: 0x00048234 File Offset: 0x00046434
		public static event Action onChatChanged;

		// Token: 0x06000A5A RID: 2650 RVA: 0x00048268 File Offset: 0x00046468
		public static void AddMessage(string message)
		{
			int num = Mathf.Max(Chat.cvChatMaxMessages.value, 1);
			while (Chat.log.Count > num)
			{
				Chat.log.RemoveAt(0);
			}
			Chat.log.Add(message);
			if (Chat.onChatChanged != null)
			{
				Chat.onChatChanged();
			}
			Debug.Log(message);
		}

		// Token: 0x06000A5B RID: 2651 RVA: 0x0000849A File Offset: 0x0000669A
		public static void Clear()
		{
			Chat.log.Clear();
			Action action = Chat.onChatChanged;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x06000A5C RID: 2652 RVA: 0x000084B5 File Offset: 0x000066B5
		public static void SendBroadcastChat(Chat.ChatMessageBase message)
		{
			Chat.SendBroadcastChat(message, QosChannelIndex.chat.intVal);
		}

		// Token: 0x06000A5D RID: 2653 RVA: 0x000482C4 File Offset: 0x000464C4
		public static void SendBroadcastChat(Chat.ChatMessageBase message, int channelIndex)
		{
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.StartMessage(59);
			networkWriter.Write(message.GetTypeIndex());
			networkWriter.Write(message);
			networkWriter.FinishMessage();
			foreach (NetworkConnection networkConnection in NetworkServer.connections)
			{
				if (networkConnection != null)
				{
					networkConnection.SendWriter(networkWriter, channelIndex);
				}
			}
		}

		// Token: 0x06000A5E RID: 2654 RVA: 0x000084C7 File Offset: 0x000066C7
		public static void SendPlayerConnectedMessage(NetworkUser user)
		{
			Chat.SendBroadcastChat(new Chat.PlayerChatMessage
			{
				networkPlayerName = user.GetNetworkPlayerName(),
				baseToken = "PLAYER_CONNECTED"
			});
		}

		// Token: 0x06000A5F RID: 2655 RVA: 0x000084EA File Offset: 0x000066EA
		public static void SendPlayerDisconnectedMessage(NetworkUser user)
		{
			Chat.SendBroadcastChat(new Chat.PlayerChatMessage
			{
				networkPlayerName = user.GetNetworkPlayerName(),
				baseToken = "PLAYER_DISCONNECTED"
			});
		}

		// Token: 0x06000A60 RID: 2656 RVA: 0x0000850D File Offset: 0x0000670D
		public static void AddPickupMessage(CharacterBody body, string pickupToken, Color32 pickupColor, uint pickupQuantity)
		{
			Chat.AddMessage(new Chat.PlayerPickupChatMessage
			{
				subjectCharacterBody = body,
				baseToken = "PLAYER_PICKUP",
				pickupToken = pickupToken,
				pickupColor = pickupColor,
				pickupQuantity = pickupQuantity
			});
		}

		// Token: 0x06000A61 RID: 2657 RVA: 0x00048340 File Offset: 0x00046540
		[NetworkMessageHandler(msgType = 59, client = true)]
		private static void HandleBroadcastChat(NetworkMessage netMsg)
		{
			Chat.ChatMessageBase chatMessageBase = Chat.ChatMessageBase.Instantiate(netMsg.reader.ReadByte());
			if (chatMessageBase != null)
			{
				chatMessageBase.Deserialize(netMsg.reader);
				Chat.AddMessage(chatMessageBase);
			}
		}

		// Token: 0x06000A62 RID: 2658 RVA: 0x00048374 File Offset: 0x00046574
		private static void AddMessage(Chat.ChatMessageBase message)
		{
			string text = message.ConstructChatString();
			if (text != null)
			{
				Chat.AddMessage(text);
				message.OnProcessed();
			}
		}

		// Token: 0x06000A63 RID: 2659 RVA: 0x00008540 File Offset: 0x00006740
		[ConCommand(commandName = "say", flags = ConVarFlags.ExecuteOnServer, helpText = "Sends a chat message.")]
		private static void CCSay(ConCommandArgs args)
		{
			args.CheckArgumentCount(1);
			if (args.sender)
			{
				Chat.SendBroadcastChat(new Chat.UserChatMessage
				{
					sender = args.sender.gameObject,
					text = args[0]
				});
			}
		}

		// Token: 0x04000DCE RID: 3534
		private static List<string> log = new List<string>();

		// Token: 0x04000DCF RID: 3535
		private static ReadOnlyCollection<string> _readOnlyLog = Chat.log.AsReadOnly();

		// Token: 0x04000DD1 RID: 3537
		private static IntConVar cvChatMaxMessages = new IntConVar("chat_max_messages", ConVarFlags.None, "30", "Maximum number of chat messages to store.");

		// Token: 0x02000215 RID: 533
		public abstract class ChatMessageBase : MessageBase
		{
			// Token: 0x06000A65 RID: 2661 RVA: 0x000085B5 File Offset: 0x000067B5
			static ChatMessageBase()
			{
				Chat.ChatMessageBase.BuildMessageTypeNetMap();
			}

			// Token: 0x06000A66 RID: 2662
			public abstract string ConstructChatString();

			// Token: 0x06000A67 RID: 2663 RVA: 0x000025F6 File Offset: 0x000007F6
			public virtual void OnProcessed()
			{
			}

			// Token: 0x06000A68 RID: 2664 RVA: 0x00048398 File Offset: 0x00046598
			private static void BuildMessageTypeNetMap()
			{
				foreach (Type type in typeof(Chat.ChatMessageBase).Assembly.GetTypes())
				{
					if (type.IsSubclassOf(typeof(Chat.ChatMessageBase)))
					{
						Chat.ChatMessageBase.chatMessageTypeToIndex.Add(type, (byte)Chat.ChatMessageBase.chatMessageIndexToType.Count);
						Chat.ChatMessageBase.chatMessageIndexToType.Add(type);
					}
				}
			}

			// Token: 0x06000A69 RID: 2665 RVA: 0x00048400 File Offset: 0x00046600
			protected string GetObjectName(GameObject namedObject)
			{
				string result = "???";
				if (namedObject)
				{
					result = namedObject.name;
					NetworkUser networkUser = namedObject.GetComponent<NetworkUser>();
					if (!networkUser)
					{
						networkUser = Util.LookUpBodyNetworkUser(namedObject);
					}
					if (networkUser)
					{
						result = Util.EscapeRichTextForTextMeshPro(networkUser.userName);
					}
				}
				return result;
			}

			// Token: 0x06000A6A RID: 2666 RVA: 0x000085D0 File Offset: 0x000067D0
			public byte GetTypeIndex()
			{
				return Chat.ChatMessageBase.chatMessageTypeToIndex[base.GetType()];
			}

			// Token: 0x06000A6B RID: 2667 RVA: 0x00048450 File Offset: 0x00046650
			public static Chat.ChatMessageBase Instantiate(byte typeIndex)
			{
				Type type = Chat.ChatMessageBase.chatMessageIndexToType[(int)typeIndex];
				Debug.LogFormat("Received chat message typeIndex={0} type={1}", new object[]
				{
					typeIndex,
					(type != null) ? type.Name : null
				});
				if (type != null)
				{
					return (Chat.ChatMessageBase)Activator.CreateInstance(type);
				}
				return null;
			}

			// Token: 0x06000A6D RID: 2669 RVA: 0x000025F6 File Offset: 0x000007F6
			public override void Serialize(NetworkWriter writer)
			{
			}

			// Token: 0x06000A6E RID: 2670 RVA: 0x000025F6 File Offset: 0x000007F6
			public override void Deserialize(NetworkReader reader)
			{
			}

			// Token: 0x04000DD2 RID: 3538
			private static readonly Dictionary<Type, byte> chatMessageTypeToIndex = new Dictionary<Type, byte>();

			// Token: 0x04000DD3 RID: 3539
			private static readonly List<Type> chatMessageIndexToType = new List<Type>();
		}

		// Token: 0x02000216 RID: 534
		public class UserChatMessage : Chat.ChatMessageBase
		{
			// Token: 0x06000A6F RID: 2671 RVA: 0x000484A8 File Offset: 0x000466A8
			public override string ConstructChatString()
			{
				if (this.sender)
				{
					NetworkUser component = this.sender.GetComponent<NetworkUser>();
					if (component)
					{
						return string.Format(CultureInfo.InvariantCulture, "<color=#e5eefc>{0}: {1}</color>", Util.EscapeRichTextForTextMeshPro(component.userName), Util.EscapeRichTextForTextMeshPro(this.text));
					}
				}
				return null;
			}

			// Token: 0x06000A70 RID: 2672 RVA: 0x000085EA File Offset: 0x000067EA
			public override void OnProcessed()
			{
				base.OnProcessed();
				Util.PlaySound("Play_UI_chatMessage", RoR2Application.instance.gameObject);
			}

			// Token: 0x06000A72 RID: 2674 RVA: 0x0000860F File Offset: 0x0000680F
			public override void Serialize(NetworkWriter writer)
			{
				writer.Write(this.sender);
				writer.Write(this.text);
			}

			// Token: 0x06000A73 RID: 2675 RVA: 0x00008629 File Offset: 0x00006829
			public override void Deserialize(NetworkReader reader)
			{
				this.sender = reader.ReadGameObject();
				this.text = reader.ReadString();
			}

			// Token: 0x04000DD4 RID: 3540
			public GameObject sender;

			// Token: 0x04000DD5 RID: 3541
			public string text;
		}

		// Token: 0x02000217 RID: 535
		public class NpcChatMessage : Chat.ChatMessageBase
		{
			// Token: 0x06000A74 RID: 2676 RVA: 0x00048500 File Offset: 0x00046700
			public override string ConstructChatString()
			{
				if (this.sender)
				{
					CharacterBody component = this.sender.GetComponent<CharacterBody>();
					string arg = ((component != null) ? component.GetDisplayName() : null) ?? "???";
					return string.Format(CultureInfo.InvariantCulture, "<style=cWorldEvent>{0}: {1}</style>", arg, Language.GetString(this.baseToken));
				}
				return null;
			}

			// Token: 0x06000A75 RID: 2677 RVA: 0x00008643 File Offset: 0x00006843
			public override void OnProcessed()
			{
				base.OnProcessed();
				Util.PlaySound(this.sound, this.sender);
			}

			// Token: 0x06000A77 RID: 2679 RVA: 0x0000865D File Offset: 0x0000685D
			public override void Serialize(NetworkWriter writer)
			{
				writer.Write(this.sender);
				writer.Write(this.baseToken);
				writer.Write(this.sound);
			}

			// Token: 0x06000A78 RID: 2680 RVA: 0x00008683 File Offset: 0x00006883
			public override void Deserialize(NetworkReader reader)
			{
				this.sender = reader.ReadGameObject();
				this.baseToken = reader.ReadString();
				this.sound = reader.ReadString();
			}

			// Token: 0x04000DD6 RID: 3542
			public GameObject sender;

			// Token: 0x04000DD7 RID: 3543
			public string baseToken;

			// Token: 0x04000DD8 RID: 3544
			public string sound;
		}

		// Token: 0x02000218 RID: 536
		public class SimpleChatMessage : Chat.ChatMessageBase
		{
			// Token: 0x06000A79 RID: 2681 RVA: 0x00048558 File Offset: 0x00046758
			public override string ConstructChatString()
			{
				string text = Language.GetString(this.baseToken);
				if (this.paramTokens != null && this.paramTokens.Length != 0)
				{
					IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
					string format = text;
					object[] args = this.paramTokens;
					text = string.Format(invariantCulture, format, args);
				}
				return text;
			}

			// Token: 0x06000A7B RID: 2683 RVA: 0x000086A9 File Offset: 0x000068A9
			public override void Serialize(NetworkWriter writer)
			{
				writer.Write(this.baseToken);
				GeneratedNetworkCode._WriteArrayString_None(writer, this.paramTokens);
			}

			// Token: 0x06000A7C RID: 2684 RVA: 0x000086C3 File Offset: 0x000068C3
			public override void Deserialize(NetworkReader reader)
			{
				this.baseToken = reader.ReadString();
				this.paramTokens = GeneratedNetworkCode._ReadArrayString_None(reader);
			}

			// Token: 0x04000DD9 RID: 3545
			public string baseToken;

			// Token: 0x04000DDA RID: 3546
			public string[] paramTokens;
		}

		// Token: 0x02000219 RID: 537
		public class SubjectChatMessage : Chat.ChatMessageBase
		{
			// Token: 0x170000B5 RID: 181
			// (get) Token: 0x06000A7E RID: 2686 RVA: 0x000086F6 File Offset: 0x000068F6
			// (set) Token: 0x06000A7D RID: 2685 RVA: 0x000086DD File Offset: 0x000068DD
			public NetworkUser subjectNetworkUser
			{
				get
				{
					if (!this.subject)
					{
						return null;
					}
					return this.subject.GetComponent<NetworkUser>();
				}
				set
				{
					this.subject = (value ? value.gameObject : null);
				}
			}

			// Token: 0x170000B6 RID: 182
			// (get) Token: 0x06000A80 RID: 2688 RVA: 0x00048598 File Offset: 0x00046798
			// (set) Token: 0x06000A7F RID: 2687 RVA: 0x00008712 File Offset: 0x00006912
			public CharacterBody subjectCharacterBody
			{
				get
				{
					GameObject subjectCharacterBodyGameObject = this.subjectCharacterBodyGameObject;
					if (!subjectCharacterBodyGameObject)
					{
						return null;
					}
					return subjectCharacterBodyGameObject.GetComponent<CharacterBody>();
				}
				set
				{
					this.subjectNetworkUser = Util.LookUpBodyNetworkUser(value);
				}
			}

			// Token: 0x170000B7 RID: 183
			// (get) Token: 0x06000A82 RID: 2690 RVA: 0x000485BC File Offset: 0x000467BC
			// (set) Token: 0x06000A81 RID: 2689 RVA: 0x00008720 File Offset: 0x00006920
			public GameObject subjectCharacterBodyGameObject
			{
				get
				{
					NetworkUser subjectNetworkUser = this.subjectNetworkUser;
					if (subjectNetworkUser)
					{
						GameObject masterObject = subjectNetworkUser.masterObject;
						if (masterObject)
						{
							CharacterMaster component = masterObject.GetComponent<CharacterMaster>();
							if (component)
							{
								return component.GetBodyObject();
							}
						}
					}
					return null;
				}
				set
				{
					this.subjectCharacterBody = (value ? value.GetComponent<CharacterBody>() : null);
				}
			}

			// Token: 0x06000A83 RID: 2691 RVA: 0x00048600 File Offset: 0x00046800
			protected bool IsSecondPerson()
			{
				if (LocalUserManager.readOnlyLocalUsersList.Count == 1 && this.subject)
				{
					NetworkUser component = this.subject.GetComponent<NetworkUser>();
					if (component && component.localUser != null)
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x06000A84 RID: 2692 RVA: 0x00008739 File Offset: 0x00006939
			protected string GetResolvedToken()
			{
				if (!this.IsSecondPerson())
				{
					return this.baseToken;
				}
				return this.baseToken + "_2P";
			}

			// Token: 0x06000A85 RID: 2693 RVA: 0x0000875A File Offset: 0x0000695A
			public override string ConstructChatString()
			{
				return string.Format(Language.GetString(this.GetResolvedToken()), base.GetObjectName(this.subject));
			}

			// Token: 0x06000A86 RID: 2694 RVA: 0x00008778 File Offset: 0x00006978
			public override void Serialize(NetworkWriter writer)
			{
				base.Serialize(writer);
				writer.Write(this.subject);
				writer.Write(this.baseToken);
			}

			// Token: 0x06000A87 RID: 2695 RVA: 0x00008799 File Offset: 0x00006999
			public override void Deserialize(NetworkReader reader)
			{
				base.Deserialize(reader);
				this.subject = reader.ReadGameObject();
				this.baseToken = reader.ReadString();
			}

			// Token: 0x04000DDB RID: 3547
			public GameObject subject;

			// Token: 0x04000DDC RID: 3548
			public string baseToken;
		}

		// Token: 0x0200021A RID: 538
		public class SubjectFormatChatMessage : Chat.SubjectChatMessage
		{
			// Token: 0x06000A89 RID: 2697 RVA: 0x00048648 File Offset: 0x00046848
			public override string ConstructChatString()
			{
				string @string = Language.GetString(base.GetResolvedToken());
				string objectName = base.GetObjectName(this.subject);
				string[] array = new string[1 + this.paramTokens.Length];
				array[0] = objectName;
				Array.Copy(this.paramTokens, 0, array, 1, this.paramTokens.Length);
				for (int i = 1; i < array.Length; i++)
				{
					array[i] = Language.GetString(array[i]);
				}
				string format = @string;
				object[] args = array;
				return string.Format(format, args);
			}

			// Token: 0x06000A8A RID: 2698 RVA: 0x000486BC File Offset: 0x000468BC
			public override void Serialize(NetworkWriter writer)
			{
				base.Serialize(writer);
				writer.Write((byte)this.paramTokens.Length);
				for (int i = 0; i < this.paramTokens.Length; i++)
				{
					writer.Write(this.paramTokens[i]);
				}
			}

			// Token: 0x06000A8B RID: 2699 RVA: 0x00048700 File Offset: 0x00046900
			public override void Deserialize(NetworkReader reader)
			{
				base.Deserialize(reader);
				this.paramTokens = new string[(int)reader.ReadByte()];
				for (int i = 0; i < this.paramTokens.Length; i++)
				{
					this.paramTokens[i] = reader.ReadString();
				}
			}

			// Token: 0x04000DDD RID: 3549
			private static readonly string[] empty = new string[0];

			// Token: 0x04000DDE RID: 3550
			public string[] paramTokens = Chat.SubjectFormatChatMessage.empty;
		}

		// Token: 0x0200021B RID: 539
		public class PlayerPickupChatMessage : Chat.SubjectChatMessage
		{
			// Token: 0x06000A8E RID: 2702 RVA: 0x00048748 File Offset: 0x00046948
			public override string ConstructChatString()
			{
				string objectName = base.GetObjectName(this.subject);
				string @string = Language.GetString(base.GetResolvedToken());
				string arg = "";
				if (this.pickupQuantity != 1u)
				{
					arg = "(" + this.pickupQuantity + ")";
				}
				string text = Language.GetString(this.pickupToken) ?? "???";
				text = Util.GenerateColoredString(text, this.pickupColor);
				return string.Format(@string, objectName, text, arg);
			}

			// Token: 0x06000A8F RID: 2703 RVA: 0x000087DA File Offset: 0x000069DA
			public override void Serialize(NetworkWriter writer)
			{
				base.Serialize(writer);
				writer.Write(this.pickupToken);
				writer.Write(this.pickupColor);
				writer.WritePackedUInt32(this.pickupQuantity);
			}

			// Token: 0x06000A90 RID: 2704 RVA: 0x00008807 File Offset: 0x00006A07
			public override void Deserialize(NetworkReader reader)
			{
				base.Deserialize(reader);
				this.pickupToken = reader.ReadString();
				this.pickupColor = reader.ReadColor32();
				this.pickupQuantity = reader.ReadPackedUInt32();
			}

			// Token: 0x04000DDF RID: 3551
			public string pickupToken;

			// Token: 0x04000DE0 RID: 3552
			public Color32 pickupColor;

			// Token: 0x04000DE1 RID: 3553
			public uint pickupQuantity;
		}

		// Token: 0x0200021C RID: 540
		public class PlayerDeathChatMessage : Chat.SubjectFormatChatMessage
		{
			// Token: 0x06000A92 RID: 2706 RVA: 0x000487C4 File Offset: 0x000469C4
			public override string ConstructChatString()
			{
				string text = base.ConstructChatString();
				if (text != null)
				{
					return "<style=cDeath><sprite name=\"Skull\" tint=1> " + text + " <sprite name=\"Skull\" tint=1></color>";
				}
				return text;
			}

			// Token: 0x06000A93 RID: 2707 RVA: 0x0000883C File Offset: 0x00006A3C
			public override void Serialize(NetworkWriter writer)
			{
				base.Serialize(writer);
			}

			// Token: 0x06000A94 RID: 2708 RVA: 0x00008845 File Offset: 0x00006A45
			public override void Deserialize(NetworkReader reader)
			{
				base.Deserialize(reader);
			}
		}

		// Token: 0x0200021D RID: 541
		public class NamedObjectChatMessage : Chat.ChatMessageBase
		{
			// Token: 0x06000A96 RID: 2710 RVA: 0x00008856 File Offset: 0x00006A56
			public override string ConstructChatString()
			{
				return string.Format(Language.GetString(this.baseToken), base.GetObjectName(this.namedObject));
			}

			// Token: 0x06000A98 RID: 2712 RVA: 0x00008874 File Offset: 0x00006A74
			public override void Serialize(NetworkWriter writer)
			{
				writer.Write(this.namedObject);
				writer.Write(this.baseToken);
				GeneratedNetworkCode._WriteArrayString_None(writer, this.paramTokens);
			}

			// Token: 0x06000A99 RID: 2713 RVA: 0x0000889A File Offset: 0x00006A9A
			public override void Deserialize(NetworkReader reader)
			{
				this.namedObject = reader.ReadGameObject();
				this.baseToken = reader.ReadString();
				this.paramTokens = GeneratedNetworkCode._ReadArrayString_None(reader);
			}

			// Token: 0x04000DE2 RID: 3554
			public GameObject namedObject;

			// Token: 0x04000DE3 RID: 3555
			public string baseToken;

			// Token: 0x04000DE4 RID: 3556
			public string[] paramTokens;
		}

		// Token: 0x0200021E RID: 542
		public class PlayerChatMessage : Chat.ChatMessageBase
		{
			// Token: 0x06000A9A RID: 2714 RVA: 0x000088C0 File Offset: 0x00006AC0
			public override string ConstructChatString()
			{
				return string.Format(Language.GetString(this.baseToken), this.networkPlayerName.GetResolvedName());
			}

			// Token: 0x06000A9B RID: 2715 RVA: 0x000088DD File Offset: 0x00006ADD
			public override void Serialize(NetworkWriter writer)
			{
				base.Serialize(writer);
				writer.Write(this.networkPlayerName);
				writer.Write(this.baseToken);
			}

			// Token: 0x06000A9C RID: 2716 RVA: 0x000088FE File Offset: 0x00006AFE
			public override void Deserialize(NetworkReader reader)
			{
				base.Deserialize(reader);
				this.networkPlayerName = reader.ReadNetworkPlayerName();
				this.baseToken = reader.ReadString();
			}

			// Token: 0x04000DE5 RID: 3557
			public NetworkPlayerName networkPlayerName;

			// Token: 0x04000DE6 RID: 3558
			public string baseToken;
		}
	}
}
