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
	// Token: 0x020004B1 RID: 1201
	public class SteamworksRemoteStorageFileSystem : FileSystem
	{
		// Token: 0x17000282 RID: 642
		// (get) Token: 0x06001B20 RID: 6944 RVA: 0x00013D37 File Offset: 0x00011F37
		private static Client steamworksClient
		{
			get
			{
				return Client.Instance;
			}
		}

		// Token: 0x17000283 RID: 643
		// (get) Token: 0x06001B21 RID: 6945 RVA: 0x000141D2 File Offset: 0x000123D2
		private static RemoteStorage remoteStorage
		{
			get
			{
				return SteamworksRemoteStorageFileSystem.steamworksClient.RemoteStorage;
			}
		}

		// Token: 0x06001B22 RID: 6946 RVA: 0x00087EBC File Offset: 0x000860BC
		public SteamworksRemoteStorageFileSystem()
		{
			this.pathToNodeMap[UPath.Root] = this.rootNode;
		}

		// Token: 0x06001B23 RID: 6947 RVA: 0x000025DA File Offset: 0x000007DA
		protected override void CreateDirectoryImpl(UPath path)
		{
		}

		// Token: 0x06001B24 RID: 6948 RVA: 0x000038B4 File Offset: 0x00001AB4
		protected override bool DirectoryExistsImpl(UPath path)
		{
			return true;
		}

		// Token: 0x06001B25 RID: 6949 RVA: 0x000141DE File Offset: 0x000123DE
		protected override void MoveDirectoryImpl(UPath srcPath, UPath destPath)
		{
			this.treeIsDirty = true;
			throw new NotImplementedException();
		}

		// Token: 0x06001B26 RID: 6950 RVA: 0x000141EC File Offset: 0x000123EC
		protected override void DeleteDirectoryImpl(UPath path, bool isRecursive)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001B27 RID: 6951 RVA: 0x000141DE File Offset: 0x000123DE
		protected override void CopyFileImpl(UPath srcPath, UPath destPath, bool overwrite)
		{
			this.treeIsDirty = true;
			throw new NotImplementedException();
		}

		// Token: 0x06001B28 RID: 6952 RVA: 0x000141EC File Offset: 0x000123EC
		protected override void ReplaceFileImpl(UPath srcPath, UPath destPath, UPath destBackupPath, bool ignoreMetadataErrors)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001B29 RID: 6953 RVA: 0x00087F14 File Offset: 0x00086114
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

		// Token: 0x06001B2A RID: 6954 RVA: 0x000141F3 File Offset: 0x000123F3
		protected override bool FileExistsImpl(UPath path)
		{
			this.UpdateDirectories();
			return this.GetFileNode(path) != null;
		}

		// Token: 0x06001B2B RID: 6955 RVA: 0x000141DE File Offset: 0x000123DE
		protected override void MoveFileImpl(UPath srcPath, UPath destPath)
		{
			this.treeIsDirty = true;
			throw new NotImplementedException();
		}

		// Token: 0x06001B2C RID: 6956 RVA: 0x00087F5C File Offset: 0x0008615C
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

		// Token: 0x06001B2D RID: 6957 RVA: 0x00087FA0 File Offset: 0x000861A0
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

		// Token: 0x06001B2E RID: 6958 RVA: 0x000141EC File Offset: 0x000123EC
		protected override FileAttributes GetAttributesImpl(UPath path)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001B2F RID: 6959 RVA: 0x000025DA File Offset: 0x000007DA
		protected override void SetAttributesImpl(UPath path, FileAttributes attributes)
		{
		}

		// Token: 0x06001B30 RID: 6960 RVA: 0x000141EC File Offset: 0x000123EC
		protected override DateTime GetCreationTimeImpl(UPath path)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001B31 RID: 6961 RVA: 0x000025DA File Offset: 0x000007DA
		protected override void SetCreationTimeImpl(UPath path, DateTime time)
		{
		}

		// Token: 0x06001B32 RID: 6962 RVA: 0x000141EC File Offset: 0x000123EC
		protected override DateTime GetLastAccessTimeImpl(UPath path)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001B33 RID: 6963 RVA: 0x000025DA File Offset: 0x000007DA
		protected override void SetLastAccessTimeImpl(UPath path, DateTime time)
		{
		}

		// Token: 0x06001B34 RID: 6964 RVA: 0x000141EC File Offset: 0x000123EC
		protected override DateTime GetLastWriteTimeImpl(UPath path)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001B35 RID: 6965 RVA: 0x000025DA File Offset: 0x000007DA
		protected override void SetLastWriteTimeImpl(UPath path, DateTime time)
		{
		}

		// Token: 0x06001B36 RID: 6966 RVA: 0x0008808C File Offset: 0x0008628C
		private SteamworksRemoteStorageFileSystem.FileNode AddFileToTree(string path)
		{
			SteamworksRemoteStorageFileSystem.FileNode fileNode = new SteamworksRemoteStorageFileSystem.FileNode(path);
			this.AddNodeToTree(fileNode);
			return fileNode;
		}

		// Token: 0x06001B37 RID: 6967 RVA: 0x000880B0 File Offset: 0x000862B0
		private SteamworksRemoteStorageFileSystem.DirectoryNode AddDirectoryToTree(UPath path)
		{
			SteamworksRemoteStorageFileSystem.DirectoryNode directoryNode = new SteamworksRemoteStorageFileSystem.DirectoryNode(path);
			this.AddNodeToTree(directoryNode);
			return directoryNode;
		}

		// Token: 0x06001B38 RID: 6968 RVA: 0x000880CC File Offset: 0x000862CC
		private void AddNodeToTree(SteamworksRemoteStorageFileSystem.Node newNode)
		{
			UPath directory = newNode.path.GetDirectory();
			this.GetDirectoryNode(directory).AddChild(newNode);
			this.pathToNodeMap[newNode.path] = newNode;
		}

		// Token: 0x06001B39 RID: 6969 RVA: 0x00088104 File Offset: 0x00086304
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

		// Token: 0x06001B3A RID: 6970 RVA: 0x00088130 File Offset: 0x00086330
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

		// Token: 0x06001B3B RID: 6971 RVA: 0x00088158 File Offset: 0x00086358
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

		// Token: 0x06001B3C RID: 6972 RVA: 0x00014205 File Offset: 0x00012405
		private void AssertDirectory(SteamworksRemoteStorageFileSystem.Node node, UPath srcPath)
		{
			if (node is SteamworksRemoteStorageFileSystem.FileNode)
			{
				throw new IOException(string.Format("The source directory `{0}` is a file", srcPath));
			}
		}

		// Token: 0x06001B3D RID: 6973 RVA: 0x00014227 File Offset: 0x00012427
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

		// Token: 0x06001B3E RID: 6974 RVA: 0x00014254 File Offset: 0x00012454
		private static void EnterFileSystemShared()
		{
			Monitor.Enter(SteamworksRemoteStorageFileSystem.globalLock);
		}

		// Token: 0x06001B3F RID: 6975 RVA: 0x00014260 File Offset: 0x00012460
		private static void ExitFileSystemShared()
		{
			Monitor.Exit(SteamworksRemoteStorageFileSystem.globalLock);
		}

		// Token: 0x06001B40 RID: 6976 RVA: 0x000141EC File Offset: 0x000123EC
		protected override IFileSystemWatcher WatchImpl(UPath path)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001B41 RID: 6977 RVA: 0x0001426C File Offset: 0x0001246C
		protected override string ConvertPathToInternalImpl(UPath path)
		{
			return path.FullName;
		}

		// Token: 0x06001B42 RID: 6978 RVA: 0x00014275 File Offset: 0x00012475
		protected override UPath ConvertPathFromInternalImpl(string innerPath)
		{
			return new UPath(innerPath);
		}

		// Token: 0x06001B43 RID: 6979 RVA: 0x00088230 File Offset: 0x00086430
		[ConCommand(commandName = "steam_remote_storage_list_files", flags = ConVarFlags.None, helpText = "Lists the files currently being managed by Steamworks remote storage.")]
		private static void CCSteamRemoteStorageListFiles(ConCommandArgs args)
		{
			Debug.Log(string.Join("\n", (from file in SteamworksRemoteStorageFileSystem.remoteStorage.Files
			select string.Format("{0} .. {1}b", file.FileName, file.SizeInBytes)).ToArray<string>()));
		}

		// Token: 0x04001DD3 RID: 7635
		private static readonly object globalLock = new object();

		// Token: 0x04001DD4 RID: 7636
		private string[] allFilePaths = Array.Empty<string>();

		// Token: 0x04001DD5 RID: 7637
		private readonly SteamworksRemoteStorageFileSystem.DirectoryNode rootNode = new SteamworksRemoteStorageFileSystem.DirectoryNode(UPath.Root);

		// Token: 0x04001DD6 RID: 7638
		private readonly Dictionary<UPath, SteamworksRemoteStorageFileSystem.Node> pathToNodeMap = new Dictionary<UPath, SteamworksRemoteStorageFileSystem.Node>();

		// Token: 0x04001DD7 RID: 7639
		private bool treeIsDirty = true;

		// Token: 0x020004B2 RID: 1202
		private struct SteamworksRemoteStoragePath : IEquatable<SteamworksRemoteStorageFileSystem.SteamworksRemoteStoragePath>
		{
			// Token: 0x06001B45 RID: 6981 RVA: 0x00014289 File Offset: 0x00012489
			public SteamworksRemoteStoragePath(string path)
			{
				this.str = path;
			}

			// Token: 0x06001B46 RID: 6982 RVA: 0x00014292 File Offset: 0x00012492
			public static implicit operator SteamworksRemoteStorageFileSystem.SteamworksRemoteStoragePath(string str)
			{
				return new SteamworksRemoteStorageFileSystem.SteamworksRemoteStoragePath(str);
			}

			// Token: 0x06001B47 RID: 6983 RVA: 0x0001429A File Offset: 0x0001249A
			public bool Equals(SteamworksRemoteStorageFileSystem.SteamworksRemoteStoragePath other)
			{
				return string.Equals(this.str, other.str);
			}

			// Token: 0x06001B48 RID: 6984 RVA: 0x00088280 File Offset: 0x00086480
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

			// Token: 0x06001B49 RID: 6985 RVA: 0x000142AD File Offset: 0x000124AD
			public override int GetHashCode()
			{
				if (this.str == null)
				{
					return 0;
				}
				return this.str.GetHashCode();
			}

			// Token: 0x04001DD8 RID: 7640
			public readonly string str;
		}

		// Token: 0x020004B3 RID: 1203
		private class Node
		{
			// Token: 0x06001B4A RID: 6986 RVA: 0x000142C4 File Offset: 0x000124C4
			public Node(UPath path)
			{
				this.path = path.ToAbsolute();
			}

			// Token: 0x04001DD9 RID: 7641
			public readonly UPath path;

			// Token: 0x04001DDA RID: 7642
			public SteamworksRemoteStorageFileSystem.Node parent;
		}

		// Token: 0x020004B4 RID: 1204
		private class FileNode : SteamworksRemoteStorageFileSystem.Node
		{
			// Token: 0x06001B4B RID: 6987 RVA: 0x000142D8 File Offset: 0x000124D8
			public FileNode(SteamworksRemoteStorageFileSystem.SteamworksRemoteStoragePath steamworksRemoteStoragePath) : base(steamworksRemoteStoragePath.str)
			{
				this.steamworksRemoteStoragePath = steamworksRemoteStoragePath;
			}

			// Token: 0x17000284 RID: 644
			// (get) Token: 0x06001B4C RID: 6988 RVA: 0x000142F2 File Offset: 0x000124F2
			private RemoteFile file
			{
				get
				{
					return SteamworksRemoteStorageFileSystem.remoteStorage.OpenFile(this.steamworksRemoteStoragePath.str);
				}
			}

			// Token: 0x06001B4D RID: 6989 RVA: 0x00014309 File Offset: 0x00012509
			public int GetLength()
			{
				return this.file.SizeInBytes;
			}

			// Token: 0x06001B4E RID: 6990 RVA: 0x00014316 File Offset: 0x00012516
			public Stream OpenWrite()
			{
				return this.file.OpenWrite();
			}

			// Token: 0x06001B4F RID: 6991 RVA: 0x00014323 File Offset: 0x00012523
			public Stream OpenRead()
			{
				return this.file.OpenRead();
			}

			// Token: 0x06001B50 RID: 6992 RVA: 0x00014330 File Offset: 0x00012530
			public void Delete()
			{
				this.file.Delete();
			}

			// Token: 0x04001DDB RID: 7643
			public readonly SteamworksRemoteStorageFileSystem.SteamworksRemoteStoragePath steamworksRemoteStoragePath;
		}

		// Token: 0x020004B5 RID: 1205
		private class DirectoryNode : SteamworksRemoteStorageFileSystem.Node
		{
			// Token: 0x17000285 RID: 645
			// (get) Token: 0x06001B51 RID: 6993 RVA: 0x0001433E File Offset: 0x0001253E
			// (set) Token: 0x06001B52 RID: 6994 RVA: 0x00014346 File Offset: 0x00012546
			public int childCount { get; private set; }

			// Token: 0x06001B53 RID: 6995 RVA: 0x0001434F File Offset: 0x0001254F
			public SteamworksRemoteStorageFileSystem.Node GetChild(int i)
			{
				return this.childNodes[i];
			}

			// Token: 0x06001B54 RID: 6996 RVA: 0x000882AC File Offset: 0x000864AC
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

			// Token: 0x06001B55 RID: 6997 RVA: 0x00088304 File Offset: 0x00086504
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

			// Token: 0x06001B56 RID: 6998 RVA: 0x00088378 File Offset: 0x00086578
			public void RemoveChild(SteamworksRemoteStorageFileSystem.Node node)
			{
				int num = Array.IndexOf<SteamworksRemoteStorageFileSystem.Node>(this.childNodes, node);
				if (num >= 0)
				{
					this.RemoveChildAt(num);
				}
			}

			// Token: 0x06001B57 RID: 6999 RVA: 0x000883A0 File Offset: 0x000865A0
			public void RemoveAllChildren()
			{
				for (int i = 0; i < this.childCount; i++)
				{
					this.childNodes[i].parent = null;
					this.childNodes[i] = null;
				}
				this.childCount = 0;
			}

			// Token: 0x06001B58 RID: 7000 RVA: 0x00014359 File Offset: 0x00012559
			public DirectoryNode(UPath path) : base(path)
			{
			}

			// Token: 0x04001DDC RID: 7644
			private SteamworksRemoteStorageFileSystem.Node[] childNodes = Array.Empty<SteamworksRemoteStorageFileSystem.Node>();
		}
	}
}
