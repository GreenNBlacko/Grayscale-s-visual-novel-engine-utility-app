using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class JsonFileIO : MonoBehaviour {
	public TMP_Text titleText;

	[Space(15)]
	public GameObject LoadFileMenu;
	public GameObject LoadFileMenuContent;
	public GameObject LoadProjectFilePrefab;

	[Space(15)]
	public TMP_InputField NewFileInput;
	public GameObject NewFileMenu;
	public GameObject NewFileMenuContent;
	public GameObject NewProjectPrefab;

	[Space(15)]
	public ItemMenu ChapterMenu;
	public ItemMenu CharacterMenu;
	public ItemMenu ArtworkMenu;
	public ItemMenu BGMMenu;

	[Space(15)]
	public GameObject ChapterPrefab;
	public GameObject SentencePrefab;
	public GameObject ChoicePrefab;
	public GameObject SentenceActionPrefab;
	public GameObject CharacterPrefab;

	[Space(15)]
	public SentencesData sentenceData;

	private string loadedFile = "";

	public void SaveFile() {
		if (loadedFile == "") { Debug.LogError("No file Loaded!"); return; }

		sentenceData = GetData();

		string jsonData = JsonUtility.ToJson(sentenceData, true);
		File.WriteAllText(Application.persistentDataPath + "/sentenceData/" + loadedFile + "/Sentences.json", jsonData);
	}

	public void LoadFile(string Path) {
		if (File.Exists(Application.persistentDataPath + "/sentenceData/" + Path + "/Sentences.json")) {
			StreamReader reader = new StreamReader(Application.persistentDataPath + "/sentenceData/" + Path + "/Sentences.json");

			string jsonData = reader.ReadToEnd();
			sentenceData = JsonUtility.FromJson<SentencesData>(jsonData);

			loadedFile = Path;

			titleText.text = "Currently loaded file: </b>" + Path + "/Sentences.json" + "</b>";

			UpdateMenus(JsonUtility.FromJson<SentencesData>(jsonData));
		} else {
			string jsonData = JsonUtility.ToJson(sentenceData, true);

			File.WriteAllText(Application.persistentDataPath + "/sentenceData/" + Path + "/Sentences.json", jsonData);

			LoadFile(Path);
		}
	}

	public void NewFile(string Path) {
		sentenceData = new SentencesData();

		System.IO.FileInfo file1 = new System.IO.FileInfo(Application.persistentDataPath);
		file1.Directory.Create();

		System.IO.FileInfo file2 = new System.IO.FileInfo(Application.persistentDataPath + "/sentenceData/" + Path);
		file2.Directory.Create();

		System.IO.FileInfo file3 = new System.IO.FileInfo(Application.persistentDataPath + "/sentenceData/" + Path + "/Sentences.json");
		file3.Directory.Create();

		LoadFile(Path);
	}

	public bool IsFileLoaded() {
		bool loaded;
		loaded = loadedFile != "";
		return loaded;
	}

	public List<string> GetFilePathNames() {
		List<string> paths = new List<string>();
		try {
			foreach (string d in Directory.GetDirectories(Application.persistentDataPath + "/sentenceData")) {
				paths.Add(Path.GetFileName(d));
			}
		}
		catch {

		}

		return paths;
	}

	public void OpenFileLoad() {
		List<string> projectNames = GetFilePathNames();

		LoadFileMenu.SetActive(true);

		foreach (string project in projectNames) {
			GameObject tmp = Instantiate(LoadProjectFilePrefab, LoadFileMenuContent.transform);
			tmp.GetComponent<ProjectFile>().FilePath = project;
			tmp.name = project;
		}
	}

	public void UpdateMenus(SentencesData data) {
		for (int i = 6; i < FindObjectOfType<MenuSystem>().menus.Count; i++) {
			Destroy(FindObjectOfType<MenuSystem>().menus[i].MenuObject.gameObject);
		}
		FindObjectOfType<MenuSystem>().menus.RemoveRange(6, FindObjectOfType<MenuSystem>().menus.Count - 6);
		foreach (Transform obj in ChapterMenu.ItemList) {
			obj.TryGetComponent(out Chapter chapter);
			if (chapter == null) { continue; }
			chapter.RemoveItem();
		}

		foreach (ChapterData chapter in data.chapters) {
			GameObject chapterObject = Instantiate(ChapterPrefab, ChapterMenu.ItemList);
			chapterObject.name = chapter.ChapterName;

			TMP_InputField.OnChangeEvent onValueChanged = chapterObject.GetComponent<Chapter>().ChapterNameInput.onValueChanged;
			TMP_InputField.SubmitEvent onSubmitEvent = chapterObject.GetComponent<Chapter>().ChapterNameInput.onEndEdit;

			chapterObject.GetComponent<Chapter>().ChapterNameInput.onValueChanged = new TMP_InputField.OnChangeEvent();
			chapterObject.GetComponent<Chapter>().ChapterNameInput.onEndEdit = new TMP_InputField.SubmitEvent();

			chapterObject.GetComponent<Chapter>().ChapterNameInput.text = chapter.ChapterName;
			chapterObject.GetComponent<Chapter>().NextChapter = chapter.NextChapter;

			chapterObject.GetComponent<Chapter>().ChapterName = chapter.ChapterName;

			chapterObject.GetComponent<Chapter>().ChapterNameInput.onValueChanged = onValueChanged;
			chapterObject.GetComponent<Chapter>().ChapterNameInput.onEndEdit = onSubmitEvent;

			chapterObject.GetComponent<Chapter>().CreateSentencesMenu(ChapterMenu.MenuName);

			foreach (SentenceData sentence in chapter.sentences) {
				GameObject sentencesObject = Instantiate(SentencePrefab, chapterObject.GetComponent<Chapter>().GetSentencesMenu().ItemList);

				sentencesObject.TryGetComponent(out Sentence tempSentence);

				tempSentence.SentenceText.text = sentence.Text;
				tempSentence.ArtworkTypeDropDown.value = sentence.ArtworkType;
				if (sentence.ArtworkType == 1)
					tempSentence.ArtworkNameDropDown.value = sentence.BG_ID;
				else if (sentence.ArtworkType == 2)
					tempSentence.ArtworkNameDropDown.value = sentence.CG_ID;
				tempSentence.ChoiceToggle.isOn = sentence.Choice;
				tempSentence.VoicedToggle.isOn = sentence.Voiced;
				if (sentence.Voiced)
					tempSentence.VAClipInput.text = sentence.VoiceClip;

				chapterObject.GetComponent<Chapter>().CreateSentenceSubmenus();

				foreach (ChoiceOptionData choice in sentence.choiceOptions) {
					GameObject choiceObject = Instantiate(ChoicePrefab, tempSentence.ChoicesMenu.GetComponent<ItemMenu>().ItemList);

					choiceObject.TryGetComponent(out Choice tempChoice);

					List<string> chapterNames = ReturnAllChapters();
					chapterNames.RemoveAt(0);

					tempChoice.ChoiceChapterInput.options = new List<TMP_Dropdown.OptionData>();
					foreach (string chapterName in chapterNames) {
						TMP_Dropdown.OptionData name = new TMP_Dropdown.OptionData() { text = chapterName };
						tempChoice.ChoiceChapterInput.options.Add(name);
					}

					tempChoice.ChoiceChapterInput.value = choice.OptionID;
					tempChoice.ChoiceTextInput.text = choice.OptionText;
				}

				foreach (SentenceActionData sentenceAction in sentence.sentenceActions) {
					GameObject sentenceActionsObject = Instantiate(SentenceActionPrefab, tempSentence.SentenceActionsMenu.GetComponent<ItemMenu>().ItemList);

					sentenceActionsObject.TryGetComponent(out SentenceAction tempSentenceAction);

					tempSentenceAction.ActionTypeDropdown.value = sentenceAction.ActionType;

					List<string> characterNames = GetCharacterList();

					tempSentenceAction.CharacterNameDropdown.options = new List<TMP_Dropdown.OptionData>();
					foreach (string characterName in characterNames) {
						TMP_Dropdown.OptionData name = new TMP_Dropdown.OptionData() { text = characterName };
						tempSentenceAction.CharacterNameDropdown.options.Add(name);
					}

					tempSentenceAction.CharacterNameDropdown.value = ReturnCharacterIndex(sentenceAction.CharacterName);

					tempSentenceAction.TransitionToggle.isOn = sentenceAction.Transition;
					tempSentenceAction.StartingPositionDropdown.value = sentenceAction.startingPosition;
					tempSentenceAction.StartingPositionInput[0].text = sentenceAction.customStartingPosition.x.ToString();
					tempSentenceAction.StartingPositionInput[1].text = sentenceAction.customStartingPosition.y.ToString();
					tempSentenceAction.PositionInput[0].text = sentenceAction.position.x.ToString();
					tempSentenceAction.PositionInput[0].text = sentenceAction.position.y.ToString();
					tempSentenceAction.TransitionSpeedSlider.value = sentenceAction.TransitionSpeed;
					tempSentenceAction.FadeInToggle.isOn = sentenceAction.FadeIn;
					tempSentenceAction.FadeOutToggle.isOn = sentenceAction.FadeOut;
					tempSentenceAction.FadeSpeedSlider.value = sentenceAction.FadeSpeed;

					tempSentenceAction.BGMNameDropdown.options = new List<TMP_Dropdown.OptionData>();

					List<string> BGMNames = GetBGMList();

					tempSentenceAction.BGMNameDropdown.options = new List<TMP_Dropdown.OptionData>();
					foreach (string BGMName in BGMNames) {
						TMP_Dropdown.OptionData name = new TMP_Dropdown.OptionData() { text = BGMName };
						tempSentenceAction.CharacterNameDropdown.options.Add(name);
					}

					tempSentenceAction.BGMNameDropdown.value = ReturnBGMIndex(sentenceAction.BGMName);

					tempSentenceAction.DelayInput.text = sentenceAction.Delay.ToString();
				}
			}

			chapterObject.GetComponent<Chapter>().UpdateMenuNames(ChapterMenu.MenuName);
		}

		foreach (Transform obj in CharacterMenu.ItemList) {
			obj.TryGetComponent(out Character character);
			if (character == null) { continue; }
			character.RemoveItem();
		}

		foreach (CharacterData character in data.characterData) {

			GameObject characterObject = Instantiate(CharacterPrefab, CharacterMenu.ItemList);
			characterObject.name = character.characterName;

			characterObject.GetComponent<Character>().NameColor = new Color32(character.nameColorRGB[0], character.nameColorRGB[1], character.nameColorRGB[2], 255);
			characterObject.GetComponent<Character>().TextColor = new Color32(character.textColorRGB[0], character.textColorRGB[1], character.textColorRGB[2], 255);
			characterObject.GetComponent<Character>().Color = new Color32(character.colorRGB[0], character.colorRGB[1], character.colorRGB[2], 255);

			characterObject.GetComponent<Character>().NameGradientColor = new Color32(character.nameColorGradientRGB[0], character.nameColorGradientRGB[1], character.nameColorGradientRGB[2], 255);
			characterObject.GetComponent<Character>().TextGradientColor = new Color32(character.textColorGradientRGB[0], character.textColorGradientRGB[1], character.textColorGradientRGB[2], 255);
			characterObject.GetComponent<Character>().GradientColor = new Color32(character.colorGradientRGB[0], character.colorGradientRGB[1], character.colorGradientRGB[2], 255);

			characterObject.GetComponent<Character>().CharacterName = character.characterName;
			characterObject.GetComponent<Character>().UseSeperateColors = character.useSeperateColors;
			characterObject.GetComponent<Character>().GradientType = character.gradientType;
			characterObject.GetComponent<Character>().UseSeperateGradientColors = character.useSeperateGradientColors;

			characterObject.GetComponent<Character>().CreateColorMenus(true);

			characterObject.GetComponent<Character>().CharacterNameInput.text = character.characterName;
			characterObject.GetComponent<Character>().UseSeperateColorsToggle.isOn = character.useSeperateColors;
			characterObject.GetComponent<Character>().GradientTypeDropdown.value = character.gradientType;
			characterObject.GetComponent<Character>().UseSeperateGradientColorsToggle.isOn = character.useSeperateGradientColors;
		}

		foreach (Transform obj in ChapterMenu.ItemList) {
			obj.TryGetComponent(out Chapter chapter);
			if (chapter == null) { continue; }
			chapter.SetValues();
		}
	}

	public void UpdateNewFileList() {
		List<string> projectNames = GetFilePathNames();

		foreach (Transform project in NewFileMenuContent.transform) {
			Destroy(project.gameObject);
		}

		foreach (string project in projectNames) {
			if (project.Contains(NewFileInput.text)) {
				GameObject tmp = Instantiate(NewProjectPrefab, NewFileMenuContent.transform);
				tmp.GetComponent<ProjectFile>().FilePath = project;
				tmp.name = project;
			}
		}
	}

	public void CloseFileLoad() {
		foreach (Transform project in LoadFileMenuContent.transform) {
			Destroy(project.gameObject);
		}
		LoadFileMenu.SetActive(false);
	}

	public void OpenFileCreate() {
		List<string> projectNames = GetFilePathNames();

		NewFileMenu.SetActive(true);

		foreach (string project in projectNames) {
			if (project.Contains(NewFileInput.text)) {
				GameObject tmp = Instantiate(NewProjectPrefab, NewFileMenuContent.transform);
				tmp.GetComponent<ProjectFile>().FilePath = project;
				tmp.name = project;
			}
		}
	}

	public void CloseFileCreate() {
		NewFileMenu.SetActive(false);
		NewFileInput.text = "";
		foreach (Transform project in NewFileMenuContent.transform) {
			Destroy(project.gameObject);
		}
	}

	public void SaveFile(TMP_InputField input) {
		if (input.text == "") { return; }
		NewFile(input.text);
		CloseFileCreate();
	}

	public SentencesData GetData() {
		SentencesData data = new SentencesData();
		List<ChapterData> chapters = new List<ChapterData>();
		List<CharacterData> characters = new List<CharacterData>();

		foreach (Transform ch in ChapterMenu.ItemList) {
			ch.TryGetComponent(out Chapter chapter);

			if (chapter != null) {
				ChapterData tempChapter = new ChapterData();

				List<SentenceData> sentences = new List<SentenceData>();

				tempChapter.ChapterName = chapter.ChapterName;
				tempChapter.NextChapter = chapter.NextChapter;

				foreach (Transform sen in chapter.GetSentencesMenu().ItemList) {
					sen.TryGetComponent(out Sentence sentence);

					if (sentence != null) {
						SentenceData tempSentence = new SentenceData();

						List<ChoiceOptionData> choices = new List<ChoiceOptionData>();
						List<SentenceActionData> sentenceActions = new List<SentenceActionData>();

						tempSentence.Name = sentence.CharacterName;
						tempSentence.Text = sentence.Text;
						tempSentence.ArtworkType = sentence.ArtworkType;
						if (sentence.ArtworkType == 1) {
							tempSentence.BG_ID = sentence.ArtworkName;
						} else if (sentence.ArtworkType == 2) {
							tempSentence.CG_ID = sentence.ArtworkName;
						}
						tempSentence.Choice = sentence.Choice;
						tempSentence.Voiced = sentence.voiced;
						if (sentence.voiced) {
							tempSentence.VoiceClip = sentence.VAClipPath;
						}

						foreach (Transform choic in sentence.ChoicesMenu.transform) {
							if (choic.TryGetComponent(out Choice choice)) {
								ChoiceOptionData tempChoice = new ChoiceOptionData();

								tempChoice.OptionID = choice.ChoiceChapter;
								tempChoice.OptionText = choice.ChoiceText;

								choices.Add(tempChoice);
							}
						}

						foreach (Transform sentAction in sentence.SentenceActionsMenu.transform) {
							if (sentAction.TryGetComponent(out SentenceAction sentenceAction)) {
								SentenceActionData tempSentenceAction = new SentenceActionData();

								tempSentenceAction.ActionType = sentenceAction.ActionType;
								tempSentenceAction.CharacterName = sentenceAction.CharacterName;
								tempSentenceAction.StateName = sentenceAction.StateName;
								tempSentenceAction.Transition = sentenceAction.Transition;
								tempSentenceAction.startingPosition = sentenceAction.StartingPosition;
								tempSentenceAction.customStartingPosition = sentenceAction.StartPosition;
								tempSentenceAction.position = sentenceAction.Postion;
								tempSentenceAction.TransitionSpeed = sentenceAction.TransitionSpeed;
								tempSentenceAction.FadeIn = sentenceAction.FadeIn;
								tempSentenceAction.FadeOut = sentenceAction.FadeOut;
								tempSentenceAction.FadeSpeed = sentenceAction.FadeSpeed;
								tempSentenceAction.BGMName = GetBGMList()[sentenceAction.BGMName];
								tempSentenceAction.Delay = sentenceAction.Delay;

								sentenceActions.Add(tempSentenceAction);
							}
						}

						tempSentence.choiceOptions = choices.ToArray();
						tempSentence.sentenceActions = sentenceActions.ToArray();

						sentences.Add(tempSentence);
					}
				}
				tempChapter.sentences = sentences.ToArray();

				chapters.Add(tempChapter);
			}
		}

		foreach (Transform charac in CharacterMenu.ItemList) {
			charac.TryGetComponent(out Character character);

			if (character == null) { continue; }

			CharacterData tempCharacter = new CharacterData();

			tempCharacter.useSeperateColors = character.UseSeperateColors;
			tempCharacter.gradientType = character.GradientType;
			tempCharacter.useSeperateGradientColors = character.UseSeperateGradientColors;

			tempCharacter.characterName = character.CharacterName;
			tempCharacter.nameColorRGB = new byte[3] { character.NameColor.r, character.NameColor.g, character.NameColor.b };
			tempCharacter.textColorRGB = new byte[3] { character.TextColor.r, character.TextColor.g, character.TextColor.b };
			tempCharacter.colorRGB = new byte[3] { character.Color.r, character.Color.g, character.Color.b };

			tempCharacter.nameColorGradientRGB = new byte[3] { character.NameGradientColor.r, character.NameGradientColor.g, character.NameGradientColor.b };
			tempCharacter.textColorGradientRGB = new byte[3] { character.TextGradientColor.r, character.TextGradientColor.g, character.TextGradientColor.b };
			tempCharacter.colorGradientRGB = new byte[3] { character.GradientColor.r, character.GradientColor.g, character.GradientColor.b };

			characters.Add(tempCharacter);
		}

		data.chapters = chapters.ToArray();
		data.characterData = characters.ToArray();

		return data;
	}

	public List<string> ReturnAllChapters() {
		List<string> chapterNames = new List<string>();

		chapterNames.Add("None");

		foreach (Transform child in ChapterMenu.ItemList) {
			if (!child.TryGetComponent(out Chapter chapter)) { continue; }
			chapterNames.Add(chapter.ChapterName);
		}

		return chapterNames;
	}

	public List<string> GetCharacterList() {
		List<string> characters = new List<string>();

		foreach (Transform temp in CharacterMenu.ItemList) {
			temp.TryGetComponent(out Character character);

			if (character == null) { continue; }
			characters.Add(character.CharacterName);
		}

		return characters;
	}

	public List<string> GetArtworkList(int Type) {
		List<string> artworks = new List<string>();

		foreach (Transform temp in ArtworkMenu.ItemList) {
			temp.TryGetComponent(out Artwork artwork);

			if (artwork == null) { continue; }
			if (artwork.ArtworkType == Type)
				artworks.Add(artwork.ArtworkName);
		}

		return artworks;
	}

	public List<string> GetBGMList() {
		List<string> BGMs = new List<string>();

		foreach (Transform temp in BGMMenu.ItemList) {
			if (temp.TryGetComponent(out BGM bgm)) {
				BGMs.Add(bgm.BGMName);
			}
		}

		return BGMs;
	}

	public int ReturnCharacterIndex(string CharacterName) {
		List<string> characterNames = GetCharacterList();

		foreach (string characterName in characterNames) {
			if (characterName == CharacterName) {
				return characterNames.IndexOf(characterName);
			}
		}

		return 0;
	}

	public int ReturnBGMIndex(string BGMName) {
		List<string> BGMNames = GetBGMList();

		foreach (string bgmName in BGMNames) {
			if (bgmName == BGMName) {
				return BGMNames.IndexOf(bgmName);
			}
		}

		return 0;
	}
}

[Serializable]
public class SentencesData {
	public ChapterData[] chapters;
	public CharacterData[] characterData;
	public ArtworkData[] artworkData;
	public BGMData[] bgmData;
}

[Serializable]
public class ChapterData {
	public string ChapterName;
	public int NextChapter = -1;
	public SentenceData[] sentences;
}

[Serializable]
public class SentenceData {
	public string Name;

	public string Text;

	public int ArtworkType;

	public int BG_ID;
	public int CG_ID;

	public bool Choice;

	public ChoiceOptionData[] choiceOptions;

	public bool Voiced;
	public string VoiceClip;

	public SentenceActionData[] sentenceActions;
}

[Serializable]
public class ChoiceOptionData {
	public int OptionID;
	public string OptionText;
}

[Serializable]
public class SentenceActionData {
	public int ActionType;

	public bool FadeIn;
	public bool FadeOut;
	public bool Transition;

	public float FadeSpeed;
	public float TransitionSpeed;

	public int startingPosition;
	public Vector2 customStartingPosition;

	public Vector2 position;

	public string CharacterName;
	public string StateName;

	public string BGMName;

	public float Delay;
}

[Serializable]
public class CharacterData {
	public string characterName;

	public CharacterStateData[] characterStates;

	public bool useSeperateColors;

	public string colorHex;
	public byte[] colorRGB;
	public string nameColorHex;
	public byte[] nameColorRGB;
	public string textColorHex;
	public byte[] textColorRGB;

	public int gradientType;

	public bool useSeperateGradientColors;

	public string colorGradientHex;
	public byte[] colorGradientRGB;
	public string nameColorGradientHex;
	public byte[] nameColorGradientRGB;
	public string textColorGradientHex;
	public byte[] textColorGradientRGB;
}

[Serializable]
public class CharacterStateData {
	public string StateName;
	public int StateType;
	public string BaseLayerImagePath;
	public string ExpressionLayerImagePath;
}

[Serializable]
public class ArtworkData {
	public string ArtworkName;
	public int ArtworkType;
	public string ArtworkPath;
}

[Serializable]
public class BGMData {
	public string BGMName;
	public string BGMPath;
}