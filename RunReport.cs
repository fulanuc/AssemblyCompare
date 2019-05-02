using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using JetBrains.Annotations;
using RoR2.Stats;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200048D RID: 1165
	public class RunReport
	{
		// Token: 0x17000270 RID: 624
		// (get) Token: 0x06001A2B RID: 6699 RVA: 0x00013693 File Offset: 0x00011893
		// (set) Token: 0x06001A2C RID: 6700 RVA: 0x000136B5 File Offset: 0x000118B5
		private string gameModeName
		{
			get
			{
				Run gameModePrefabComponent = GameModeCatalog.GetGameModePrefabComponent(this.gameModeIndex);
				return ((gameModePrefabComponent != null) ? gameModePrefabComponent.name : null) ?? "InvalidGameMode";
			}
			set
			{
				this.gameModeIndex = GameModeCatalog.FindGameModeIndex(value);
			}
		}

		// Token: 0x17000271 RID: 625
		// (get) Token: 0x06001A2D RID: 6701 RVA: 0x000136C3 File Offset: 0x000118C3
		public int playerInfoCount
		{
			get
			{
				return this.playerInfos.Length;
			}
		}

		// Token: 0x06001A2E RID: 6702 RVA: 0x000136CD File Offset: 0x000118CD
		[NotNull]
		public RunReport.PlayerInfo GetPlayerInfo(int i)
		{
			return this.playerInfos[i];
		}

		// Token: 0x06001A2F RID: 6703 RVA: 0x000136D7 File Offset: 0x000118D7
		[CanBeNull]
		public RunReport.PlayerInfo GetPlayerInfoSafe(int i)
		{
			return HGArrayUtilities.GetSafe<RunReport.PlayerInfo>(this.playerInfos, i);
		}

		// Token: 0x06001A30 RID: 6704 RVA: 0x0008554C File Offset: 0x0008374C
		public static RunReport Generate([NotNull] Run run, GameResultType resultType)
		{
			RunReport runReport = new RunReport();
			runReport.gameModeIndex = GameModeCatalog.FindGameModeIndex(run.gameObject.name);
			runReport.seed = run.seed;
			runReport.snapshotTime = Run.FixedTimeStamp.now;
			runReport.runStopwatchValue = run.GetRunStopwatch();
			runReport.gameResultType = resultType;
			runReport.ruleBook.Copy(run.ruleBook);
			runReport.playerInfos = new RunReport.PlayerInfo[PlayerCharacterMasterController.instances.Count];
			for (int i = 0; i < runReport.playerInfos.Length; i++)
			{
				runReport.playerInfos[i] = RunReport.PlayerInfo.Generate(PlayerCharacterMasterController.instances[i]);
			}
			runReport.ResolveLocalInformation();
			return runReport;
		}

		// Token: 0x06001A31 RID: 6705 RVA: 0x000855F8 File Offset: 0x000837F8
		private void ResolveLocalInformation()
		{
			RunReport.PlayerInfo[] array = this.playerInfos;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].ResolveLocalInformation();
			}
		}

		// Token: 0x06001A32 RID: 6706 RVA: 0x00085624 File Offset: 0x00083824
		public void Write(NetworkWriter writer)
		{
			writer.Write((byte)this.gameResultType);
			writer.WritePackedUInt32((uint)this.gameModeIndex);
			writer.Write(this.seed);
			writer.Write(this.snapshotTime);
			writer.Write(this.runStopwatchValue);
			writer.Write(this.ruleBook);
			writer.Write((byte)this.playerInfos.Length);
			for (int i = 0; i < this.playerInfos.Length; i++)
			{
				this.playerInfos[i].Write(writer);
			}
		}

		// Token: 0x06001A33 RID: 6707 RVA: 0x000856AC File Offset: 0x000838AC
		public void Read(NetworkReader reader)
		{
			this.gameResultType = (GameResultType)reader.ReadByte();
			this.gameModeIndex = (int)reader.ReadPackedUInt32();
			this.seed = reader.ReadUInt64();
			this.snapshotTime = reader.ReadFixedTimeStamp();
			this.runStopwatchValue = reader.ReadSingle();
			reader.ReadRuleBook(this.ruleBook);
			int newSize = (int)reader.ReadByte();
			Array.Resize<RunReport.PlayerInfo>(ref this.playerInfos, newSize);
			for (int i = 0; i < this.playerInfos.Length; i++)
			{
				if (this.playerInfos[i] == null)
				{
					this.playerInfos[i] = new RunReport.PlayerInfo();
				}
				this.playerInfos[i].Read(reader);
			}
			Array.Sort<RunReport.PlayerInfo>(this.playerInfos, delegate(RunReport.PlayerInfo a, RunReport.PlayerInfo b)
			{
				if (a.isLocalPlayer == b.isLocalPlayer)
				{
					if (a.isLocalPlayer)
					{
						return b.localPlayerIndex - a.localPlayerIndex;
					}
					return 0;
				}
				else
				{
					if (!a.isLocalPlayer)
					{
						return 1;
					}
					return -1;
				}
			});
		}

		// Token: 0x06001A34 RID: 6708 RVA: 0x00085778 File Offset: 0x00083978
		public static void ToXml(XElement element, RunReport runReport)
		{
			element.RemoveAll();
			element.Add(HGXml.ToXml<string>("version", "2"));
			element.Add(HGXml.ToXml<string>("gameModeName", runReport.gameModeName));
			element.Add(HGXml.ToXml<GameResultType>("gameResultType", runReport.gameResultType));
			element.Add(HGXml.ToXml<ulong>("seed", runReport.seed));
			element.Add(HGXml.ToXml<Run.FixedTimeStamp>("snapshotTime", runReport.snapshotTime));
			element.Add(HGXml.ToXml<float>("runStopwatchValue", runReport.runStopwatchValue));
			element.Add(HGXml.ToXml<RuleBook>("ruleBook", runReport.ruleBook));
			element.Add(HGXml.ToXml<RunReport.PlayerInfo[]>("playerInfos", runReport.playerInfos));
		}

		// Token: 0x06001A35 RID: 6709 RVA: 0x0008583C File Offset: 0x00083A3C
		public static bool FromXml(XElement element, ref RunReport runReport)
		{
			string text = "NO_VERSION";
			XElement xelement = element.Element("version");
			if (xelement != null)
			{
				xelement.Deserialize(ref text);
			}
			if (text != "2" && !(text == "1"))
			{
				Debug.LogFormat("Could not load RunReport with non-upgradeable version \"{0}\".", new object[]
				{
					text
				});
				runReport = null;
				return false;
			}
			string gameModeName = runReport.gameModeName;
			XElement xelement2 = element.Element("gameModeName");
			if (xelement2 != null)
			{
				xelement2.Deserialize(ref gameModeName);
			}
			runReport.gameModeName = gameModeName;
			XElement xelement3 = element.Element("gameResultType");
			if (xelement3 != null)
			{
				xelement3.Deserialize(ref runReport.gameResultType);
			}
			XElement xelement4 = element.Element("seed");
			if (xelement4 != null)
			{
				xelement4.Deserialize(ref runReport.seed);
			}
			XElement xelement5 = element.Element("snapshotTime");
			if (xelement5 != null)
			{
				xelement5.Deserialize(ref runReport.snapshotTime);
			}
			XElement xelement6 = element.Element("runStopwatchValue");
			if (xelement6 != null)
			{
				xelement6.Deserialize(ref runReport.runStopwatchValue);
			}
			XElement xelement7 = element.Element("ruleBook");
			if (xelement7 != null)
			{
				xelement7.Deserialize(ref runReport.ruleBook);
			}
			XElement xelement8 = element.Element("playerInfos");
			if (xelement8 != null)
			{
				xelement8.Deserialize(ref runReport.playerInfos);
			}
			return true;
		}

		// Token: 0x06001A36 RID: 6710 RVA: 0x0008599C File Offset: 0x00083B9C
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			RunReport.runReportsFolder = Application.dataPath + "/RunReports/";
			HGXml.Register<RunReport>(new HGXml.Serializer<RunReport>(RunReport.ToXml), new HGXml.Deserializer<RunReport>(RunReport.FromXml));
			HGXml.Register<RunReport.PlayerInfo>(new HGXml.Serializer<RunReport.PlayerInfo>(RunReport.PlayerInfo.ToXml), new HGXml.Deserializer<RunReport.PlayerInfo>(RunReport.PlayerInfo.FromXml));
			HGXml.Register<RunReport.PlayerInfo[]>(new HGXml.Serializer<RunReport.PlayerInfo[]>(RunReport.PlayerInfo.ArrayToXml), new HGXml.Deserializer<RunReport.PlayerInfo[]>(RunReport.PlayerInfo.ArrayFromXml));
		}

		// Token: 0x06001A37 RID: 6711 RVA: 0x000136E5 File Offset: 0x000118E5
		[NotNull]
		private static string FileNameToPath([NotNull] string fileName)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}{1}.xml", RunReport.runReportsFolder, fileName);
		}

		// Token: 0x06001A38 RID: 6712 RVA: 0x00085A14 File Offset: 0x00083C14
		[CanBeNull]
		public static RunReport Load([NotNull] string fileName)
		{
			string text = RunReport.FileNameToPath(fileName);
			RunReport result;
			try
			{
				XElement xelement = XDocument.Load(text).Element("RunReport");
				if (xelement == null)
				{
					Debug.LogFormat("Could not load RunReport {0}: {1}", new object[]
					{
						text,
						"File is malformed."
					});
					result = null;
				}
				else
				{
					RunReport runReport = new RunReport();
					RunReport.FromXml(xelement, ref runReport);
					result = runReport;
				}
			}
			catch (Exception ex)
			{
				Debug.LogFormat("Could not load RunReport {0}: {1}", new object[]
				{
					text,
					ex.Message
				});
				result = null;
			}
			return result;
		}

		// Token: 0x06001A39 RID: 6713 RVA: 0x00085AAC File Offset: 0x00083CAC
		public static bool Save([NotNull] RunReport runReport, [NotNull] string fileName)
		{
			string text = RunReport.FileNameToPath(fileName);
			bool result;
			try
			{
				if (!Directory.Exists(RunReport.runReportsFolder))
				{
					Directory.CreateDirectory(RunReport.runReportsFolder);
				}
				XDocument xdocument = new XDocument();
				xdocument.Add(HGXml.ToXml<RunReport>("RunReport", runReport));
				xdocument.Save(text);
				result = true;
			}
			catch (Exception ex)
			{
				Debug.LogFormat("Could not save RunReport {0}: {1}", new object[]
				{
					text,
					ex.Message
				});
				result = false;
			}
			return result;
		}

		// Token: 0x06001A3A RID: 6714 RVA: 0x00085B2C File Offset: 0x00083D2C
		public static void TestSerialization(RunReport runReport)
		{
			NetworkWriter networkWriter = new NetworkWriter();
			runReport.Write(networkWriter);
			NetworkReader reader = new NetworkReader(networkWriter.AsArray());
			new RunReport().Read(reader);
		}

		// Token: 0x04001D52 RID: 7506
		private const string currentXmlVersion = "2";

		// Token: 0x04001D53 RID: 7507
		private int gameModeIndex = -1;

		// Token: 0x04001D54 RID: 7508
		public GameResultType gameResultType;

		// Token: 0x04001D55 RID: 7509
		public ulong seed;

		// Token: 0x04001D56 RID: 7510
		public Run.FixedTimeStamp snapshotTime;

		// Token: 0x04001D57 RID: 7511
		public float runStopwatchValue;

		// Token: 0x04001D58 RID: 7512
		public RuleBook ruleBook = new RuleBook();

		// Token: 0x04001D59 RID: 7513
		private RunReport.PlayerInfo[] playerInfos = Array.Empty<RunReport.PlayerInfo>();

		// Token: 0x04001D5A RID: 7514
		private static string runReportsFolder;

		// Token: 0x0200048E RID: 1166
		public class PlayerInfo
		{
			// Token: 0x17000272 RID: 626
			// (get) Token: 0x06001A3C RID: 6716 RVA: 0x00013721 File Offset: 0x00011921
			[CanBeNull]
			public LocalUser localUser
			{
				get
				{
					if (!this.networkUser)
					{
						return null;
					}
					return this.networkUser.localUser;
				}
			}

			// Token: 0x17000273 RID: 627
			// (get) Token: 0x06001A3D RID: 6717 RVA: 0x0001373D File Offset: 0x0001193D
			public bool isLocalPlayer
			{
				get
				{
					return this.localPlayerIndex >= 0;
				}
			}

			// Token: 0x17000274 RID: 628
			// (get) Token: 0x06001A3E RID: 6718 RVA: 0x0001374B File Offset: 0x0001194B
			// (set) Token: 0x06001A3F RID: 6719 RVA: 0x00013772 File Offset: 0x00011972
			public string bodyName
			{
				get
				{
					GameObject bodyPrefab = BodyCatalog.GetBodyPrefab(this.bodyIndex);
					return ((bodyPrefab != null) ? bodyPrefab.gameObject.name : null) ?? "InvalidBody";
				}
				set
				{
					this.bodyIndex = BodyCatalog.FindBodyIndex(value);
				}
			}

			// Token: 0x17000275 RID: 629
			// (get) Token: 0x06001A40 RID: 6720 RVA: 0x00013780 File Offset: 0x00011980
			// (set) Token: 0x06001A41 RID: 6721 RVA: 0x000137A7 File Offset: 0x000119A7
			public string killerBodyName
			{
				get
				{
					GameObject bodyPrefab = BodyCatalog.GetBodyPrefab(this.killerBodyIndex);
					return ((bodyPrefab != null) ? bodyPrefab.gameObject.name : null) ?? "InvalidBody";
				}
				set
				{
					this.killerBodyIndex = BodyCatalog.FindBodyIndex(value);
				}
			}

			// Token: 0x06001A42 RID: 6722 RVA: 0x00085B60 File Offset: 0x00083D60
			public void Write(NetworkWriter writer)
			{
				writer.WriteBodyIndex(this.bodyIndex);
				writer.WriteBodyIndex(this.killerBodyIndex);
				writer.Write(this.master ? this.master.gameObject : null);
				this.statSheet.Write(writer);
				writer.WritePackedUInt32((uint)this.itemAcquisitionOrder.Length);
				for (int i = 0; i < this.itemAcquisitionOrder.Length; i++)
				{
					writer.Write(this.itemAcquisitionOrder[i]);
				}
				writer.WriteItemStacks(this.itemStacks);
				writer.WritePackedUInt32((uint)this.equipment.Length);
				for (int j = 0; j < this.equipment.Length; j++)
				{
					writer.Write(this.equipment[j]);
				}
			}

			// Token: 0x06001A43 RID: 6723 RVA: 0x00085C1C File Offset: 0x00083E1C
			public void Read(NetworkReader reader)
			{
				this.bodyIndex = reader.ReadBodyIndex();
				this.killerBodyIndex = reader.ReadBodyIndex();
				GameObject gameObject = reader.ReadGameObject();
				this.master = (gameObject ? gameObject.GetComponent<CharacterMaster>() : null);
				this.statSheet.Read(reader);
				int newSize = (int)reader.ReadPackedUInt32();
				Array.Resize<ItemIndex>(ref this.itemAcquisitionOrder, newSize);
				for (int i = 0; i < this.itemAcquisitionOrder.Length; i++)
				{
					ItemIndex itemIndex = reader.ReadItemIndex();
					this.itemAcquisitionOrder[i] = itemIndex;
				}
				reader.ReadItemStacks(this.itemStacks);
				int newSize2 = (int)reader.ReadPackedUInt32();
				Array.Resize<EquipmentIndex>(ref this.equipment, newSize2);
				for (int j = 0; j < this.equipment.Length; j++)
				{
					EquipmentIndex equipmentIndex = reader.ReadEquipmentIndex();
					this.equipment[j] = equipmentIndex;
				}
				this.ResolveLocalInformation();
			}

			// Token: 0x06001A44 RID: 6724 RVA: 0x00085CF4 File Offset: 0x00083EF4
			public void ResolveLocalInformation()
			{
				this.name = Util.GetBestMasterName(this.master);
				PlayerCharacterMasterController playerCharacterMasterController = null;
				if (this.master)
				{
					playerCharacterMasterController = this.master.GetComponent<PlayerCharacterMasterController>();
				}
				this.networkUser = null;
				if (playerCharacterMasterController)
				{
					this.networkUser = playerCharacterMasterController.networkUser;
				}
				this.localPlayerIndex = -1;
				this.userProfileFileName = string.Empty;
				if (this.networkUser && this.networkUser.localUser != null)
				{
					this.localPlayerIndex = this.networkUser.localUser.id;
					this.userProfileFileName = this.networkUser.localUser.userProfile.fileName;
				}
			}

			// Token: 0x06001A45 RID: 6725 RVA: 0x00085DA8 File Offset: 0x00083FA8
			public static RunReport.PlayerInfo Generate(PlayerCharacterMasterController playerCharacterMasterController)
			{
				CharacterMaster characterMaster = playerCharacterMasterController.master;
				Inventory inventory = characterMaster.inventory;
				PlayerStatsComponent component = playerCharacterMasterController.GetComponent<PlayerStatsComponent>();
				RunReport.PlayerInfo playerInfo = new RunReport.PlayerInfo();
				playerInfo.networkUser = playerCharacterMasterController.networkUser;
				playerInfo.master = characterMaster;
				playerInfo.bodyIndex = BodyCatalog.FindBodyIndex(characterMaster.bodyPrefab);
				playerInfo.killerBodyIndex = characterMaster.GetKillerBodyIndex();
				StatSheet.Copy(component.currentStats, playerInfo.statSheet);
				playerInfo.itemAcquisitionOrder = inventory.itemAcquisitionOrder.ToArray();
				for (ItemIndex itemIndex = ItemIndex.Syringe; itemIndex < ItemIndex.Count; itemIndex++)
				{
					playerInfo.itemStacks[(int)itemIndex] = inventory.GetItemCount(itemIndex);
				}
				playerInfo.equipment = new EquipmentIndex[inventory.GetEquipmentSlotCount()];
				uint num = 0u;
				while ((ulong)num < (ulong)((long)playerInfo.equipment.Length))
				{
					playerInfo.equipment[(int)num] = inventory.GetEquipment(num).equipmentIndex;
					num += 1u;
				}
				return playerInfo;
			}

			// Token: 0x06001A46 RID: 6726 RVA: 0x00085E80 File Offset: 0x00084080
			public static void ToXml(XElement element, RunReport.PlayerInfo playerInfo)
			{
				element.RemoveAll();
				element.Add(HGXml.ToXml<string>("name", playerInfo.name));
				element.Add(HGXml.ToXml<string>("bodyName", playerInfo.bodyName));
				element.Add(HGXml.ToXml<string>("killerBodyName", playerInfo.killerBodyName));
				element.Add(HGXml.ToXml<StatSheet>("statSheet", playerInfo.statSheet));
				element.Add(HGXml.ToXml<ItemIndex[]>("itemAcquisitionOrder", playerInfo.itemAcquisitionOrder));
				element.Add(HGXml.ToXml<int[]>("itemStacks", playerInfo.itemStacks, RunReport.PlayerInfo.itemStacksRules));
				element.Add(HGXml.ToXml<EquipmentIndex[]>("equipment", playerInfo.equipment, RunReport.PlayerInfo.equipmentRules));
				element.Add(HGXml.ToXml<int>("localPlayerIndex", playerInfo.localPlayerIndex));
				element.Add(HGXml.ToXml<string>("userProfileFileName", playerInfo.userProfileFileName));
			}

			// Token: 0x06001A47 RID: 6727 RVA: 0x00085F64 File Offset: 0x00084164
			public static bool FromXml(XElement element, ref RunReport.PlayerInfo playerInfo)
			{
				playerInfo = new RunReport.PlayerInfo();
				XElement xelement = element.Element("name");
				if (xelement != null)
				{
					xelement.Deserialize(ref playerInfo.name);
				}
				string bodyName = playerInfo.bodyName;
				XElement xelement2 = element.Element("bodyName");
				if (xelement2 != null)
				{
					xelement2.Deserialize(ref bodyName);
				}
				playerInfo.bodyName = bodyName;
				string killerBodyName = playerInfo.killerBodyName;
				XElement xelement3 = element.Element("killerBodyName");
				if (xelement3 != null)
				{
					xelement3.Deserialize(ref killerBodyName);
				}
				playerInfo.killerBodyName = killerBodyName;
				XElement xelement4 = element.Element("statSheet");
				if (xelement4 != null)
				{
					xelement4.Deserialize(ref playerInfo.statSheet);
				}
				XElement xelement5 = element.Element("itemAcquisitionOrder");
				if (xelement5 != null)
				{
					xelement5.Deserialize(ref playerInfo.itemAcquisitionOrder);
				}
				XElement xelement6 = element.Element("itemStacks");
				if (xelement6 != null)
				{
					xelement6.Deserialize(ref playerInfo.itemStacks, RunReport.PlayerInfo.itemStacksRules);
				}
				XElement xelement7 = element.Element("equipment");
				if (xelement7 != null)
				{
					xelement7.Deserialize(ref playerInfo.equipment, RunReport.PlayerInfo.equipmentRules);
				}
				XElement xelement8 = element.Element("localPlayerIndex");
				if (xelement8 != null)
				{
					xelement8.Deserialize(ref playerInfo.localPlayerIndex);
				}
				XElement xelement9 = element.Element("userProfileFileName");
				if (xelement9 != null)
				{
					xelement9.Deserialize(ref playerInfo.userProfileFileName);
				}
				return true;
			}

			// Token: 0x06001A48 RID: 6728 RVA: 0x000860CC File Offset: 0x000842CC
			public static void ArrayToXml(XElement element, RunReport.PlayerInfo[] playerInfos)
			{
				element.RemoveAll();
				for (int i = 0; i < playerInfos.Length; i++)
				{
					element.Add(HGXml.ToXml<RunReport.PlayerInfo>("PlayerInfo", playerInfos[i]));
				}
			}

			// Token: 0x06001A49 RID: 6729 RVA: 0x00086100 File Offset: 0x00084300
			public static bool ArrayFromXml(XElement element, ref RunReport.PlayerInfo[] playerInfos)
			{
				playerInfos = (from e in element.Elements()
				where e.Name == "PlayerInfo"
				select e).Select(delegate(XElement e)
				{
					RunReport.PlayerInfo result = null;
					HGXml.FromXml<RunReport.PlayerInfo>(e, ref result);
					return result;
				}).ToArray<RunReport.PlayerInfo>();
				return true;
			}

			// Token: 0x04001D5B RID: 7515
			[CanBeNull]
			public NetworkUser networkUser;

			// Token: 0x04001D5C RID: 7516
			[CanBeNull]
			public CharacterMaster master;

			// Token: 0x04001D5D RID: 7517
			public int localPlayerIndex = -1;

			// Token: 0x04001D5E RID: 7518
			public string name = string.Empty;

			// Token: 0x04001D5F RID: 7519
			public int bodyIndex = -1;

			// Token: 0x04001D60 RID: 7520
			public int killerBodyIndex = -1;

			// Token: 0x04001D61 RID: 7521
			public StatSheet statSheet = StatSheet.New();

			// Token: 0x04001D62 RID: 7522
			public ItemIndex[] itemAcquisitionOrder = Array.Empty<ItemIndex>();

			// Token: 0x04001D63 RID: 7523
			public int[] itemStacks = new int[78];

			// Token: 0x04001D64 RID: 7524
			public EquipmentIndex[] equipment = Array.Empty<EquipmentIndex>();

			// Token: 0x04001D65 RID: 7525
			public string userProfileFileName = string.Empty;

			// Token: 0x04001D66 RID: 7526
			private static readonly HGXml.SerializationRules<int[]> itemStacksRules = new HGXml.SerializationRules<int[]>
			{
				serializer = delegate(XElement element, int[] value)
				{
					element.RemoveAll();
					element.Add(from itemIndex in ItemCatalog.allItems
					where value[(int)itemIndex] > 0
					select new XElement(itemIndex.ToString(), value[(int)itemIndex]));
				},
				deserializer = delegate(XElement element, ref int[] value)
				{
					Array.Resize<int>(ref value, 78);
					using (IEnumerator<XElement> enumerator = element.Elements().GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							ItemIndex itemIndex;
							if (Enum.TryParse<ItemIndex>(enumerator.Current.Name.LocalName, true, out itemIndex) && itemIndex >= ItemIndex.Syringe)
							{
								HGXml.FromXml<int>(element, ref value[(int)itemIndex]);
							}
						}
					}
					return true;
				}
			};

			// Token: 0x04001D67 RID: 7527
			private static readonly HGXml.SerializationRules<EquipmentIndex[]> equipmentRules = new HGXml.SerializationRules<EquipmentIndex[]>
			{
				serializer = delegate(XElement element, EquipmentIndex[] value)
				{
					element.Value = string.Join(" ", from equipmentIndex in value
					select equipmentIndex.ToString());
				},
				deserializer = delegate(XElement element, ref EquipmentIndex[] value)
				{
					value = element.Value.Split(new char[]
					{
						' '
					}).Select(delegate(string str)
					{
						EquipmentIndex result;
						if (!Enum.TryParse<EquipmentIndex>(str, false, out result))
						{
							return EquipmentIndex.None;
						}
						return result;
					}).ToArray<EquipmentIndex>();
					return true;
				}
			};
		}
	}
}
