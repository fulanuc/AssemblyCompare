using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Facepunch.Steamworks;
using JetBrains.Annotations;
using UnityEngine;
using Zio;
using Zio.FileSystems;

namespace RoR2
{
	// Token: 0x020004A4 RID: 1188
	public class SteamworksRemoteStorageFileSystem : FileSystem
	{
		// Token: 0x17000276 RID: 630
		// (get) Token: 0x06001ABD RID: 6845 RVA: 0x00013821 File Offset: 0x00011A21
		private static Client steamworksClient
		{
			get
			{
				return Client.Instance;
			}
		}

		// Token: 0x17000277 RID: 631
		// (get) Token: 0x06001ABE RID: 6846 RVA: 0x00013CC0 File Offset: 0x00011EC0
		private static RemoteStorage remoteStorage
		{
			get
			{
				return SteamworksRemoteStorageFileSystem.steamworksClient.RemoteStorage;
			}
		}

		// Token: 0x06001ABF RID: 6847 RVA: 0x0008734C File Offset: 0x0008554C
		public SteamworksRemoteStorageFileSystem()
		{
			this.pathToNodeMap[UPath.Root] = this.rootNode;
		}

		// Token: 0x06001AC0 RID: 6848 RVA: 0x000025F6 File Offset: 0x000007F6
		protected override void CreateDirectoryImpl(UPath path)
		{
		}

		// Token: 0x06001AC1 RID: 6849 RVA: 0x000038B4 File Offset: 0x00001AB4
		protected override bool DirectoryExistsImpl(UPath path)
		{
			return true;
		}

		// Token: 0x06001AC2 RID: 6850 RVA: 0x00013CCC File Offset: 0x00011ECC
		protected override void MoveDirectoryImpl(UPath srcPath, UPath destPath)
		{
			this.treeIsDirty = true;
			throw new NotImplementedException();
		}

		// Token: 0x06001AC3 RID: 6851 RVA: 0x00013CDA File Offset: 0x00011EDA
		protected override void DeleteDirectoryImpl(UPath path, bool isRecursive)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001AC4 RID: 6852 RVA: 0x00013CCC File Offset: 0x00011ECC
		protected override void CopyFileImpl(UPath srcPath, UPath destPath, bool overwrite)
		{
			this.treeIsDirty = true;
			throw new NotImplementedException();
		}

		// Token: 0x06001AC5 RID: 6853 RVA: 0x00013CDA File Offset: 0x00011EDA
		protected override void ReplaceFileImpl(UPath srcPath, UPath destPath, UPath destBackupPath, bool ignoreMetadataErrors)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001AC6 RID: 6854 RVA: 0x000873A4 File Offset: 0x000855A4
		protected override long GetFileLengthImpl(UPath path)
		{
			int num = 0;
			SteamworksRemoteStorageFileSystem.EnterFileSystemShared();
			try
			{
				this.UpdateDirectories();
				SteamworksRemoteStorageFileSystem.FileNode fileNode = this.GetFileNode(path);
				num = ((fileNode != null) ? fileNode.GetLength() : 0);
			}
			finally
			{
				SteamworksRemoteStorageFileSystem.ExitFileSystemShared();
			}
			return (long)num;
		}

		// Token: 0x06001AC7 RID: 6855 RVA: 0x00013CE1 File Offset: 0x00011EE1
		protected override bool FileExistsImpl(UPath path)
		{
			this.UpdateDirectories();
			return this.GetFileNode(path) != null;
		}

		// Token: 0x06001AC8 RID: 6856 RVA: 0x00013CCC File Offset: 0x00011ECC
		protected override void MoveFileImpl(UPath srcPath, UPath destPath)
		{
			this.treeIsDirty = true;
			throw new NotImplementedException();
		}

		// Token: 0x06001AC9 RID: 6857 RVA: 0x000873EC File Offset: 0x000855EC
		protected override void DeleteFileImpl(UPath path)
		{
			SteamworksRemoteStorageFileSystem.EnterFileSystemShared();
			try
			{
				this.treeIsDirty = true;
				SteamworksRemoteStorageFileSystem.FileNode fileNode = this.GetFileNode(path);
				if (fileNode != null)
				{
					fileNode.Delete();
				}
			}
			finally
			{
				SteamworksRemoteStorageFileSystem.ExitFileSystemShared();
			}
		}

		// Token: 0x06001ACA RID: 6858 RVA: 0x00087430 File Offset: 0x00085630
		protected override Stream OpenFileImpl(UPath path, FileMode mode, FileAccess access, FileShare share)
		{
			SteamworksRemoteStorageFileSystem.EnterFileSystemShared();
			if (!path.IsAbsolute)
			{
				throw new ArgumentException(string.Format("'{0}' must be absolute. {0} = {1}", "path", path));
			}
			Stream result;
			try
			{
				bool flag = false;
				switch (mode)
				{
				case FileMode.CreateNew:
					flag = true;
					break;
				case FileMode.Create:
					flag = true;
					break;
				case FileMode.Append:
					throw new NotImplementedException();
				}
				flag &= (access == FileAccess.Write);
				if (flag)
				{
					this.treeIsDirty = true;
					result = SteamworksRemoteStorageFileSystem.remoteStorage.CreateFile(path.ToRelative().FullName).OpenWrite();
				}
				else if (access != FileAccess.Read)
				{
					if (access != FileAccess.Write)
					{
						throw new NotImplementedException();
					}
					SteamworksRemoteStorageFileSystem.FileNode fileNode = this.GetFileNode(path);
					result = ((fileNode != null) ? fileNode.OpenWrite() : null);
				}
				else
				{
					SteamworksRemoteStorageFileSystem.FileNode fileNode2 = this.GetFileNode(path);
					result = ((fileNode2 != null) ? fileNode2.OpenRead() : null);
				}
			}
			finally
			{
				SteamworksRemoteStorageFileSystem.ExitFileSystemShared();
			}
			return result;
		}

		// Token: 0x06001ACB RID: 6859 RVA: 0x00013CDA File Offset: 0x00011EDA
		protected override FileAttributes GetAttributesImpl(UPath path)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001ACC RID: 6860 RVA: 0x000025F6 File Offset: 0x000007F6
		protected override void SetAttributesImpl(UPath path, FileAttributes attributes)
		{
		}

		// Token: 0x06001ACD RID: 6861 RVA: 0x00013CDA File Offset: 0x00011EDA
		protected override DateTime GetCreationTimeImpl(UPath path)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001ACE RID: 6862 RVA: 0x000025F6 File Offset: 0x000007F6
		protected override void SetCreationTimeImpl(UPath path, DateTime time)
		{
		}

		// Token: 0x06001ACF RID: 6863 RVA: 0x00013CDA File Offset: 0x00011EDA
		protected override DateTime GetLastAccessTimeImpl(UPath path)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001AD0 RID: 6864 RVA: 0x000025F6 File Offset: 0x000007F6
		protected override void SetLastAccessTimeImpl(UPath path, DateTime time)
		{
		}

		// Token: 0x06001AD1 RID: 6865 RVA: 0x00013CDA File Offset: 0x00011EDA
		protected override DateTime GetLastWriteTimeImpl(UPath path)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001AD2 RID: 6866 RVA: 0x000025F6 File Offset: 0x000007F6
		protected override void SetLastWriteTimeImpl(UPath path, DateTime time)
		{
		}

		// Token: 0x06001AD3 RID: 6867 RVA: 0x0008751C File Offset: 0x0008571C
		private SteamworksRemoteStorageFileSystem.FileNode AddFileToTree(string path)
		{
			SteamworksRemoteStorageFileSystem.FileNode fileNode = new SteamworksRemoteStorageFileSystem.FileNode(path);
			this.AddNodeToTree(fileNode);
			return fileNode;
		}

		// Token: 0x06001AD4 RID: 6868 RVA: 0x00087540 File Offset: 0x00085740
		private SteamworksRemoteStorageFileSystem.DirectoryNode AddDirectoryToTree(UPath path)
		{
			SteamworksRemoteStorageFileSystem.DirectoryNode directoryNode = new SteamworksRemoteStorageFileSystem.DirectoryNode(path);
			this.AddNodeToTree(directoryNode);
			return directoryNode;
		}

		// Token: 0x06001AD5 RID: 6869 RVA: 0x0008755C File Offset: 0x0008575C
		private void AddNodeToTree(SteamworksRemoteStorageFileSystem.Node newNode)
		{
			UPath directory = newNode.path.GetDirectory();
			this.GetDirectoryNode(directory).AddChild(newNode);
			this.pathToNodeMap[newNode.path] = newNode;
		}

		// Token: 0x06001AD6 RID: 6870 RVA: 0x00087594 File Offset: 0x00085794
		[CanBeNull]
		private SteamworksRemoteStorageFileSystem.DirectoryNode GetDirectoryNode(UPath directoryPath)
		{
			SteamworksRemoteStorageFileSystem.Node node;
			if (this.pathToNodeMap.TryGetValue(directoryPath, out node))
			{
				return node as SteamworksRemoteStorageFileSystem.DirectoryNode;
			}
			return this.AddDirectoryToTree(directoryPath);
		}

		// Token: 0x06001AD7 RID: 6871 RVA: 0x000875C0 File Offset: 0x000857C0
		[CanBeNull]
		private SteamworksRemoteStorageFileSystem.FileNode GetFileNode(UPath filePath)
		{
			SteamworksRemoteStorageFileSystem.Node node;
			if (this.pathToNodeMap.TryGetValue(filePath, out node))
			{
				return node as SteamworksRemoteStorageFileSystem.FileNode;
			}
			return null;
		}

		// Token: 0x06001AD8 RID: 6872 RVA: 0x000875E8 File Offset: 0x000857E8
		private void UpdateDirectories()
		{
			SteamworksRemoteStorageFileSystem.EnterFileSystemShared();
			try
			{
				if (this.treeIsDirty)
				{
					this.treeIsDirty = false;
					IEnumerable<string> enumerable = from file in SteamworksRemoteStorageFileSystem.remoteStorage.Files
					select file.FileName;
					if (!enumerable.SequenceEqual(this.allFilePaths))
					{
						this.allFilePaths = enumerable.ToArray<string>();
						this.pathToNodeMap.Clear();
						this.pathToNodeMap[UPath.Root] = this.rootNode;
						this.rootNode.RemoveAllChildren();
						foreach (string path in this.allFilePaths)
						{
							this.AddFileToTree(path);
						}
					}
				}
			}
			finally
			{
				SteamworksRemoteStorageFileSystem.ExitFileSystemShared();
			}
		}

		// Token: 0x06001AD9 RID: 6873 RVA: 0x00013CF3 File Offset: 0x00011EF3
		private void AssertDirectory(SteamworksRemoteStorageFileSystem.Node node, UPath srcPath)
		{
			if (node is SteamworksRemoteStorageFileSystem.FileNode)
			{
				throw new IOException(string.Format("The source directory `{0}` is a file", srcPath));
			}
		}

		// Token: 0x06001ADA RID: 6874 RVA: 0x00013D15 File Offset: 0x00011F15
		protected override IEnumerable<UPath> EnumeratePathsImpl(UPath path, string searchPattern, SearchOption searchOption, SearchTarget searchTarget)
		{
			this.UpdateDirectories();
			SearchPattern search = SearchPattern.Parse(ref path, ref searchPattern);
			List<UPath> foldersToProcess = new List<UPath>();
			foldersToProcess.Add(path);
			SortedSet<UPath> entries = new SortedSet<UPath>(UPath.DefaultComparerIgnoreCase);
			while (foldersToProcess.Count > 0)
			{
				UPath upath = foldersToProcess[0];
				foldersToProcess.RemoveAt(0);
				int num = 0;
				entries.Clear();
				SteamworksRemoteStorageFileSystem.EnterFileSystemShared();
				try
				{
					SteamworksRemoteStorageFileSystem.Node directoryNode = this.GetDirectoryNode(upath);
					if (upath == path)
					{
						this.AssertDirectory(directoryNode, upath);
					}
					else if (!(directoryNode is SteamworksRemoteStorageFileSystem.DirectoryNode))
					{
						continue;
					}
					SteamworksRemoteStorageFileSystem.DirectoryNode directoryNode2 = (SteamworksRemoteStorageFileSystem.DirectoryNode)directoryNode;
					for (int i = 0; i < directoryNode2.childCount; i++)
					{
						SteamworksRemoteStorageFileSystem.Node child = directoryNode2.GetChild(i);
						if (!(child is SteamworksRemoteStorageFileSystem.FileNode) || searchTarget != SearchTarget.Directory)
						{
							bool flag = search.Match(child.path);
							bool flag2 = searchOption == SearchOption.AllDirectories && child is SteamworksRemoteStorageFileSystem.DirectoryNode;
							bool flag3 = (child is SteamworksRemoteStorageFileSystem.FileNode && searchTarget != SearchTarget.Directory && flag) || (child is SteamworksRemoteStorageFileSystem.DirectoryNode && searchTarget != SearchTarget.File && flag);
							UPath item = upath / child.path;
							if (flag2)
							{
								foldersToProcess.Insert(num++, item);
							}
							if (flag3)
							{
								entries.Add(item);
							}
						}
					}
				}
				finally
				{
					SteamworksRemoteStorageFileSystem.ExitFileSystemShared();
				}
				foreach (UPath upath2 in entries)
				{
					yield return upath2;
				}
				SortedSet<UPath>.Enumerator enumerator = default(SortedSet<UPath>.Enumerator);
			}
			yield break;
			yield break;
		}

		// Token: 0x06001ADB RID: 6875 RVA: 0x00013D42 File Offset: 0x00011F42
		private static void EnterFileSystemShared()
		{
			Monitor.Enter(SteamworksRemoteStorageFileSystem.globalLock);
		}

		// Token: 0x06001ADC RID: 6876 RVA: 0x00013D4E File Offset: 0x00011F4E
		private static void ExitFileSystemShared()
		{
			Monitor.Exit(SteamworksRemoteStorageFileSystem.globalLock);
		}

		// Token: 0x06001ADD RID: 6877 RVA: 0x00013CDA File Offset: 0x00011EDA
		protected override IFileSystemWatcher WatchImpl(UPath path)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001ADE RID: 6878 RVA: 0x00013D5A File Offset: 0x00011F5A
		protected override string ConvertPathToInternalImpl(UPath path)
		{
			return path.FullName;
		}

		// Token: 0x06001ADF RID: 6879 RVA: 0x00013D63 File Offset: 0x00011F63
		protected override UPath ConvertPathFromInternalImpl(string innerPath)
		{
			return new UPath(innerPath);
		}

		// Token: 0x06001AE0 RID: 6880 RVA: 0x000876C0 File Offset: 0x000858C0
		[ConCommand(commandName = "steam_remote_storage_list_files", flags = ConVarFlags.None, helpText = "Lists the files currently being managed by Steamworks remote storage.")]
		private static void CCSteamRemoteStorageListFiles(ConCommandArgs args)
		{
			Debug.Log(string.Join("\n", (from file in SteamworksRemoteStorageFileSystem.remoteStorage.Files
			select string.Format("{0} .. {1}b", file.FileName, file.SizeInBytes)).ToArray<string>()));
		}

		// Token: 0x04001D9A RID: 7578
		private static readonly object globalLock = new object();

		// Token: 0x04001D9B RID: 7579
		private string[] allFilePaths = Array.Empty<string>();

		// Token: 0x04001D9C RID: 7580
		private readonly SteamworksRemoteStorageFileSystem.DirectoryNode rootNode = new SteamworksRemoteStorageFileSystem.DirectoryNode(UPath.Root);

		// Token: 0x04001D9D RID: 7581
		private readonly Dictionary<UPath, SteamworksRemoteStorageFileSystem.Node> pathToNodeMap = new Dictionary<UPath, SteamworksRemoteStorageFileSystem.Node>();

		// Token: 0x04001D9E RID: 7582
		private bool treeIsDirty = true;

		// Token: 0x020004A5 RID: 1189
		private struct SteamworksRemoteStoragePath : IEquatable<SteamworksRemoteStorageFileSystem.SteamworksRemoteStoragePath>
		{
			// Token: 0x06001AE2 RID: 6882 RVA: 0x00013D77 File Offset: 0x00011F77
			public SteamworksRemoteStoragePath(string path)
			{
				this.str = path;
			}

			// Token: 0x06001AE3 RID: 6883 RVA: 0x00013D80 File Offset: 0x00011F80
			public static implicit operator SteamworksRemoteStorageFileSystem.SteamworksRemoteStoragePath(string str)
			{
				return new SteamworksRemoteStorageFileSystem.SteamworksRemoteStoragePath(str);
			}

			// Token: 0x06001AE4 RID: 6884 RVA: 0x00013D88 File Offset: 0x00011F88
			public bool Equals(SteamworksRemoteStorageFileSystem.SteamworksRemoteStoragePath other)
			{
				return string.Equals(this.str, other.str);
			}

			// Token: 0x06001AE5 RID: 6885 RVA: 0x00087710 File Offset: 0x00085910
			public override bool Equals(object obj)
			{
				if (obj == null)
				{
					return false;
				}
				if (obj is SteamworksRemoteStorageFileSystem.SteamworksRemoteStoragePath)
				{
					SteamworksRemoteStorageFileSystem.SteamworksRemoteStoragePath other = (SteamworksRemoteStorageFileSystem.SteamworksRemoteStoragePath)obj;
					return this.Equals(other);
				}
				return false;
			}

			// Token: 0x06001AE6 RID: 6886 RVA: 0x00013D9B File Offset: 0x00011F9B
			public override int GetHashCode()
			{
				if (this.str == null)
				{
					return 0;
				}
				return this.str.GetHashCode();
			}

			// Token: 0x04001D9F RID: 7583
			public readonly string str;
		}

		// Token: 0x020004A6 RID: 1190
		private class Node
		{
			// Token: 0x06001AE7 RID: 6887 RVA: 0x00013DB2 File Offset: 0x00011FB2
			public Node(UPath path)
			{
				this.path = path.ToAbsolute();
			}

			// Token: 0x04001DA0 RID: 7584
			public readonly UPath path;

			// Token: 0x04001DA1 RID: 7585
			public SteamworksRemoteStorageFileSystem.Node parent;
		}

		// Token: 0x020004A7 RID: 1191
		private class FileNode : SteamworksRemoteStorageFileSystem.Node
		{
			// Token: 0x06001AE8 RID: 6888 RVA: 0x00013DC6 File Offset: 0x00011FC6
			public FileNode(SteamworksRemoteStorageFileSystem.SteamworksRemoteStoragePath steamworksRemoteStoragePath) : base(steamworksRemoteStoragePath.str)
			{
				this.steamworksRemoteStoragePath = steamworksRemoteStoragePath;
			}

			// Token: 0x17000278 RID: 632
			// (get) Token: 0x06001AE9 RID: 6889 RVA: 0x00013DE0 File Offset: 0x00011FE0
			private RemoteFile file
			{
				get
				{
					return SteamworksRemoteStorageFileSystem.remoteStorage.OpenFile(this.steamworksRemoteStoragePath.str);
				}
			}

			// Token: 0x06001AEA RID: 6890 RVA: 0x00013DF7 File Offset: 0x00011FF7
			public int GetLength()
			{
				return this.file.SizeInBytes;
			}

			// Token: 0x06001AEB RID: 6891 RVA: 0x00013E04 File Offset: 0x00012004
			public Stream OpenWrite()
			{
				return this.file.OpenWrite();
			}

			// Token: 0x06001AEC RID: 6892 RVA: 0x00013E11 File Offset: 0x00012011
			public Stream OpenRead()
			{
				return this.file.OpenRead();
			}

			// Token: 0x06001AED RID: 6893 RVA: 0x00013E1E File Offset: 0x0001201E
			public void Delete()
			{
				this.file.Delete();
			}

			// Token: 0x04001DA2 RID: 7586
			public readonly SteamworksRemoteStorageFileSystem.SteamworksRemoteStoragePath steamworksRemoteStoragePath;
		}

		// Token: 0x020004A8 RID: 1192
		private class DirectoryNode : SteamworksRemoteStorageFileSystem.Node
		{
			// Token: 0x17000279 RID: 633
			// (get) Token: 0x06001AEE RID: 6894 RVA: 0x00013E2C File Offset: 0x0001202C
			// (set) Token: 0x06001AEF RID: 6895 RVA: 0x00013E34 File Offset: 0x00012034
			public int childCount { get; private set; }

			// Token: 0x06001AF0 RID: 6896 RVA: 0x00013E3D File Offset: 0x0001203D
			public SteamworksRemoteStorageFileSystem.Node GetChild(int i)
			{
				return this.childNodes[i];
			}

			// Token: 0x06001AF1 RID: 6897 RVA: 0x0008773C File Offset: 0x0008593C
			public void AddChild(SteamworksRemoteStorageFileSystem.Node node)
			{
				int childCount = this.childCount + 1;
				this.childCount = childCount;
				if (this.childCount > this.childNodes.Length)
				{
					Array.Resize<SteamworksRemoteStorageFileSystem.Node>(ref this.childNodes, this.childCount);
				}
				this.childNodes[this.childCount - 1] = node;
				node.parent = this;
			}

			// Token: 0x06001AF2 RID: 6898 RVA: 0x00087794 File Offset: 0x00085994
			public void RemoveChildAt(int i)
			{
				if (this.childCount > 0)
				{
					this.childNodes[i].parent = null;
				}
				int num = this.childCount - 1;
				while (i < num)
				{
					this.childNodes[i] = this.childNodes[i + 1];
					i++;
				}
				if (this.childCount > 0)
				{
					this.childNodes[this.childCount - 1] = null;
				}
				int childCount = this.childCount - 1;
				this.childCount = childCount;
			}

			// Token: 0x06001AF3 RID: 6899 RVA: 0x00087808 File Offset: 0x00085A08
			public void RemoveChild(SteamworksRemoteStorageFileSystem.Node node)
			{
				int num = Array.IndexOf<SteamworksRemoteStorageFileSystem.Node>(this.childNodes, node);
				if (num >= 0)
				{
					this.RemoveChildAt(num);
				}
			}

			// Token: 0x06001AF4 RID: 6900 RVA: 0x00087830 File Offset: 0x00085A30
			public void RemoveAllChildren()
			{
				for (int i = 0; i < this.childCount; i++)
				{
					this.childNodes[i].parent = null;
					this.childNodes[i] = null;
				}
				this.childCount = 0;
			}

			// Token: 0x06001AF5 RID: 6901 RVA: 0x00013E47 File Offset: 0x00012047
			public DirectoryNode(UPath path) : base(path)
			{
			}

			// Token: 0x04001DA3 RID: 7587
			private SteamworksRemoteStorageFileSystem.Node[] childNodes = Array.Empty<SteamworksRemoteStorageFileSystem.Node>();
		}
	}
}
