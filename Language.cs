using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using JetBrains.Annotations;
using RoR2.ConVar;
using RoR2.UI;
using SimpleJSON;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200044F RID: 1103
	public static class Language
	{
		// Token: 0x06001890 RID: 6288 RVA: 0x000808B8 File Offset: 0x0007EAB8
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

		// Token: 0x06001891 RID: 6289 RVA: 0x000808E8 File Offset: 0x0007EAE8
		private static void LoadLanguageFile([NotNull] string language, [NotNull] string fileName)
		{
			Dictionary<string, string> dictionary = Language.LoadLanguageDictionary(language);
			string text = string.Format(CultureInfo.InvariantCulture, "{0}/Language/{1}/{2}", Application.dataPath, language, fileName);
			if (File.Exists(text))
			{
				try
				{
					JSONNode jsonnode = JSON.Parse(File.ReadAllText(text));
					if (jsonnode != null)
					{
						JSONNode jsonnode2 = jsonnode["strings"];
						if (jsonnode2 != null)
						{
							foreach (string text2 in jsonnode2.Keys)
							{
								dictionary[text2] = jsonnode2[text2].Value;
							}
						}
					}
				}
				catch (Exception ex)
				{
					Debug.LogFormat("Parsing error in language file \"{0}\". Error: {1}", new object[]
					{
						text,
						ex
					});
				}
			}
		}

		// Token: 0x06001892 RID: 6290 RVA: 0x000809C8 File Offset: 0x0007EBC8
		public static void LoadAllFilesForLanguage([NotNull] string language)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(string.Format("{0}/Language/{1}/", Application.dataPath, language));
			if (directoryInfo.Exists)
			{
				foreach (FileInfo fileInfo in directoryInfo.GetFiles())
				{
					if (fileInfo.Extension != ".meta")
					{
						Language.LoadLanguageFile(language, fileInfo.Name);
					}
				}
			}
		}

		// Token: 0x06001893 RID: 6291 RVA: 0x0001267B File Offset: 0x0001087B
		private static void UnloadLanguage([NotNull] string language)
		{
			Language.languageDictionaries.Remove(language);
		}

		// Token: 0x06001894 RID: 6292 RVA: 0x00080A2C File Offset: 0x0007EC2C
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

		// Token: 0x06001895 RID: 6293 RVA: 0x00012689 File Offset: 0x00010889
		public static string GetString([NotNull] string token)
		{
			return Language.GetString(token, Language.currentLanguage);
		}

		// Token: 0x06001896 RID: 6294 RVA: 0x00012696 File Offset: 0x00010896
		public static string GetStringFormatted([NotNull] string token, params object[] args)
		{
			return string.Format(Language.GetString(token), args);
		}

		// Token: 0x06001897 RID: 6295 RVA: 0x000126A4 File Offset: 0x000108A4
		public static bool IsTokenInvalid(string token)
		{
			return token == Language.GetString(token);
		}

		// Token: 0x06001898 RID: 6296 RVA: 0x00080A58 File Offset: 0x0007EC58
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

		// Token: 0x06001899 RID: 6297 RVA: 0x000126B2 File Offset: 0x000108B2
		[ConCommand(commandName = "language_reload", flags = ConVarFlags.None, helpText = "Reloads the current language.")]
		public static void CCLanguageReload(ConCommandArgs args)
		{
			Language.SetCurrentLanguage(Language.currentLanguage);
		}

		// Token: 0x04001C10 RID: 7184
		private static string currentLanguage = "";

		// Token: 0x04001C11 RID: 7185
		private static readonly Dictionary<string, Dictionary<string, string>> languageDictionaries = new Dictionary<string, Dictionary<string, string>>();

		// Token: 0x04001C12 RID: 7186
		private static Language.LanguageConVar cvLanguage = new Language.LanguageConVar("language", ConVarFlags.Archive, "EN_US", "Which language to use.");

		// Token: 0x02000450 RID: 1104
		private class LanguageConVar : BaseConVar
		{
			// Token: 0x0600189B RID: 6299 RVA: 0x000090A8 File Offset: 0x000072A8
			public LanguageConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x0600189C RID: 6300 RVA: 0x000126EE File Offset: 0x000108EE
			public override void SetString(string newValue)
			{
				Language.SetCurrentLanguage(newValue);
			}

			// Token: 0x0600189D RID: 6301 RVA: 0x000126F6 File Offset: 0x000108F6
			public override string GetString()
			{
				return Language.currentLanguage;
			}
		}
	}
}
