using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using RoR2.ConVar;
using RoR2.UI;
using SimpleJSON;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000459 RID: 1113
	public static class Language
	{
		// Token: 0x060018E5 RID: 6373 RVA: 0x0008109C File Offset: 0x0007F29C
		private static Dictionary<string, string> LoadLanguageDictionary([NotNull] string language)
		{
			Dictionary<string, string> dictionary;
			if (!Language.languageDictionaries.TryGetValue(language, out dictionary))
			{
				dictionary = new Dictionary<string, string>();
				Language.languageDictionaries[language] = dictionary;
			}
			return dictionary;
		}

		// Token: 0x060018E6 RID: 6374 RVA: 0x000810CC File Offset: 0x0007F2CC
		private static KeyValuePair<string, string>[] LoadTokensFromFile([NotNull] string path)
		{
			if (File.Exists(path))
			{
				try
				{
					JSONNode jsonnode = JSON.Parse(File.ReadAllText(path, Encoding.UTF8));
					if (jsonnode != null)
					{
						JSONNode jsonnode2 = jsonnode["strings"];
						if (jsonnode2 != null)
						{
							KeyValuePair<string, string>[] array = new KeyValuePair<string, string>[jsonnode2.Count];
							int num = 0;
							foreach (string text in jsonnode2.Keys)
							{
								array[num++] = new KeyValuePair<string, string>(text, jsonnode2[text].Value);
							}
							return array;
						}
					}
				}
				catch (Exception ex)
				{
					Debug.LogFormat("Parsing error in language file \"{0}\". Error: {1}", new object[]
					{
						path,
						ex
					});
				}
			}
			return Array.Empty<KeyValuePair<string, string>>();
		}

		// Token: 0x060018E7 RID: 6375 RVA: 0x00012B52 File Offset: 0x00010D52
		[NotNull]
		private static string GetPathForLanguageFile([NotNull] string language, [NotNull] string fileName)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}/Language/{1}/{2}", Application.dataPath, language, fileName);
		}

		// Token: 0x060018E8 RID: 6376 RVA: 0x000811B8 File Offset: 0x0007F3B8
		private static void ImportLanguageFile([NotNull] string language, [NotNull] string fileName)
		{
			Dictionary<string, string> dictionary = Language.LoadLanguageDictionary(language);
			string pathForLanguageFile = Language.GetPathForLanguageFile(language, fileName);
			if (File.Exists(pathForLanguageFile))
			{
				foreach (KeyValuePair<string, string> keyValuePair in Language.LoadTokensFromFile(pathForLanguageFile))
				{
					dictionary[keyValuePair.Key] = keyValuePair.Value;
				}
			}
		}

		// Token: 0x060018E9 RID: 6377 RVA: 0x00081210 File Offset: 0x0007F410
		[NotNull]
		private static IEnumerable<FileInfo> GetFilesForLanguage([NotNull] string language)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(string.Format("{0}/Language/{1}/", Application.dataPath, language));
			if (directoryInfo.Exists)
			{
				return from file in directoryInfo.GetFiles()
				where file.Extension != ".meta"
				select file;
			}
			return Enumerable.Empty<FileInfo>();
		}

		// Token: 0x060018EA RID: 6378 RVA: 0x0008126C File Offset: 0x0007F46C
		public static void LoadAllFilesForLanguage([NotNull] string language)
		{
			foreach (FileInfo fileInfo in Language.GetFilesForLanguage(language))
			{
				Language.ImportLanguageFile(language, fileInfo.Name);
			}
		}

		// Token: 0x060018EB RID: 6379 RVA: 0x00012B6A File Offset: 0x00010D6A
		private static void UnloadLanguage([NotNull] string language)
		{
			Language.languageDictionaries.Remove(language);
		}

		// Token: 0x060018EC RID: 6380 RVA: 0x000812C0 File Offset: 0x0007F4C0
		public static string GetString([NotNull] string token, [NotNull] string language)
		{
			Dictionary<string, string> dictionary;
			string result;
			if (Language.languageDictionaries.TryGetValue(language, out dictionary) && dictionary.TryGetValue(token, out result))
			{
				return result;
			}
			return token;
		}

		// Token: 0x060018ED RID: 6381 RVA: 0x00012B78 File Offset: 0x00010D78
		public static string GetString([NotNull] string token)
		{
			return Language.GetString(token, Language.currentLanguage);
		}

		// Token: 0x060018EE RID: 6382 RVA: 0x00012B85 File Offset: 0x00010D85
		public static string GetStringFormatted([NotNull] string token, params object[] args)
		{
			return string.Format(Language.GetString(token), args);
		}

		// Token: 0x060018EF RID: 6383 RVA: 0x00012B93 File Offset: 0x00010D93
		public static bool IsTokenInvalid(string token)
		{
			return token == Language.GetString(token);
		}

		// Token: 0x060018F0 RID: 6384 RVA: 0x000812EC File Offset: 0x0007F4EC
		public static void SetCurrentLanguage([NotNull] string language)
		{
			Language.UnloadLanguage(Language.currentLanguage);
			Language.currentLanguage = language;
			Language.LoadAllFilesForLanguage(Language.currentLanguage);
			LanguageTextMeshController[] array = UnityEngine.Object.FindObjectsOfType<LanguageTextMeshController>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].ResolveString();
			}
		}

		// Token: 0x060018F1 RID: 6385 RVA: 0x00012BA1 File Offset: 0x00010DA1
		[ConCommand(commandName = "language_reload", flags = ConVarFlags.None, helpText = "Reloads the current language.")]
		public static void CCLanguageReload(ConCommandArgs args)
		{
			Language.SetCurrentLanguage(Language.currentLanguage);
		}

		// Token: 0x060018F2 RID: 6386 RVA: 0x00081330 File Offset: 0x0007F530
		[ConCommand(commandName = "language_dump_to_json", flags = ConVarFlags.None, helpText = "Combines all files for the given language into a single JSON file.")]
		private static void CCLanguageDumpToJson(ConCommandArgs args)
		{
			args.CheckArgumentCount(1);
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			string language = args[0];
			foreach (FileInfo fileInfo in Language.GetFilesForLanguage(language))
			{
				list.AddRange(Language.LoadTokensFromFile(Language.GetPathForLanguageFile(language, fileInfo.Name)));
			}
			StringBuilder stringBuilder = new StringBuilder();
			JSONNode jsonnode = new JSONObject();
			JSONNode jsonnode2 = jsonnode["strings"] = new JSONObject();
			foreach (KeyValuePair<string, string> keyValuePair in list)
			{
				jsonnode2[keyValuePair.Key] = keyValuePair.Value;
			}
			jsonnode.WriteToStringBuilder(stringBuilder, 0, 1, JSONTextMode.Indent);
			File.WriteAllText("output.json", stringBuilder.ToString(), Encoding.UTF8);
		}

		// Token: 0x04001C42 RID: 7234
		private static string currentLanguage = "";

		// Token: 0x04001C43 RID: 7235
		private static readonly Dictionary<string, Dictionary<string, string>> languageDictionaries = new Dictionary<string, Dictionary<string, string>>();

		// Token: 0x04001C44 RID: 7236
		private static Language.LanguageConVar cvLanguage = new Language.LanguageConVar("language", ConVarFlags.Archive, "EN_US", "Which language to use.");

		// Token: 0x0200045A RID: 1114
		private class LanguageConVar : BaseConVar
		{
			// Token: 0x060018F4 RID: 6388 RVA: 0x000090CD File Offset: 0x000072CD
			public LanguageConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x060018F5 RID: 6389 RVA: 0x00012BDD File Offset: 0x00010DDD
			public override void SetString(string newValue)
			{
				Language.SetCurrentLanguage(newValue);
			}

			// Token: 0x060018F6 RID: 6390 RVA: 0x00012BE5 File Offset: 0x00010DE5
			public override string GetString()
			{
				return Language.currentLanguage;
			}
		}
	}
}
