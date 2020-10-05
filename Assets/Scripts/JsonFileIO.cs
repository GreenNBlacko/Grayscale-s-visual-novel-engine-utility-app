using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

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
	public GameObject CharacterStatePrefab;
	public GameObject ArtworkPrefab;
	public GameObject BGMPrefab;

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

		foreach (Transform obj in CharacterMenu.ItemList) {
			obj.TryGetComponent(out Character character);
			if (character == null) { continue; }
			character.RemoveItem();
		}

		foreach (Transform obj in ArtworkMenu.ItemList) {
			obj.TryGetComponent(out Artwork artwork);
			if (artwork == null) { continue; }
			artwork.RemoveItem();
		}

		foreach (Transform obj in BGMMenu.ItemList) {
			obj.TryGetComponent(out BGM bgm);
			if (bgm == null) { continue; }
			bgm.RemoveItem();
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

			characterObject.GetComponent<Character>().CreateCharacterStatesMenu(CharacterMenu.MenuName);

			foreach (CharacterStateData characterState in character.characterStates) {
				GameObject characterStateObject = Instantiate(CharacterStatePrefab, characterObject.GetComponent<Character>().GetCharacterStatesMenu().ItemList);

				characterStateObject.GetComponent<CharacterState>().StateNameInput.text = characterState.StateName;
				characterStateObject.GetComponent<CharacterState>().StateTypeDropdown.value = characterState.StateType;
				characterStateObject.GetComponent<CharacterState>().BaseLayerInput.text = characterState.BaseLayerImagePath;
				characterStateObject.GetComponent<CharacterState>().ExpressionLayerInput.text = characterState.ExpressionLayerImagePath;

				characterStateObject.GetComponent<CharacterState>().SetValues();
			}
		}

		foreach(ArtworkData artwork in data.artworkData) {
			GameObject artworkObject = Instantiate(ArtworkPrefab, ArtworkMenu.ItemList);

			artworkObject.GetComponent<Artwork>().ArtworkNameInput.text = artwork.ArtworkName;
			artworkObject.GetComponent<Artwork>().ArtworkTypeDropdown.value = artwork.ArtworkType;
			artworkObject.GetComponent<Artwork>().ArtworkPathInput.text = artwork.ArtworkPath;

			artworkObject.GetComponent<Artwork>().SetValues();
		}

		foreach (BGMData bgm in data.bgmData) {
			GameObject bgmObject = Instantiate(BGMPrefab, BGMMenu.ItemList);

			bgmObject.GetComponent<BGM>().BGMNameInput.text = bgm.BGMName;
			bgmObject.GetComponent<BGM>().BGMPathInput.text = bgm.BGMPath;

			bgmObject.GetComponent<BGM>().SetValues();
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

				List<string> characterNames = GetCharacterList();

				tempSentence.NameDropDown.options.Clear();
				foreach (string characterName in characterNames) {
					TMP_Dropdown.OptionData name = new TMP_Dropdown.OptionData() { text = characterName };
					tempSentence.NameDropDown.options.Add(name);
				}

				tempSentence.NameDropDown.value = ReturnCharacterIndex(sentence.Name);

				tempSentence.SentenceText.text = sentence.Text;
				tempSentence.ArtworkTypeDropDown.value = sentence.ArtworkType;

				tempSentence.ArtworkNameDropDown.options.Clear();

				List<string> ArtworkList = GetArtworkList(sentence.ArtworkType - 1);

				foreach (string art in ArtworkList) {
					TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData();

					optionData.text = art;

					tempSentence.ArtworkNameDropDown.options.Add(optionData);
				}

				TMP_Dropdown.DropdownEvent tempAction = tempSentence.ArtworkNameDropDown.onValueChanged;
				tempSentence.ArtworkNameDropDown.onValueChanged = new TMP_Dropdown.DropdownEvent();

				if (sentence.ArtworkType == 1)
					tempSentence.ArtworkName = sentence.BG_ID;
				else if (sentence.ArtworkType == 2) {
					tempSentence.ArtworkName = sentence.CG_ID;
				}

				tempSentence.ArtworkNameDropDown.onValueChanged = tempAction;

				tempSentence.ChoiceToggle.isOn = sentence.Choice;
				tempSentence.VoicedToggle.isOn = sentence.Voiced;
				if (sentence.Voiced)
					tempSentence.VAClipInput.text = sentence.VoiceClip;

				chapterObject.GetComponent<Chapter>().CreateSentenceSubmenus();

				tempSentence.SetValues();

				foreach (ChoiceOptionData choice in sentence.choiceOptions) {
					GameObject choiceObject = Instantiate(ChoicePrefab, tempSentence.ChoicesMenu.GetComponent<ItemMenu>().ItemList);

					choiceObject.TryGetComponent(out Choice tempChoice);

					List<string> chapterNames = ReturnAllChapters();
					chapterNames.RemoveAt(0);

					tempChoice.ChoiceChapterInput.options.Clear();
					foreach (string chapterName in chapterNames) {
						TMP_Dropdown.OptionData name = new TMP_Dropdown.OptionData() { text = chapterName };
						tempChoice.ChoiceChapterInput.options.Add(name);
					}

					TMP_Dropdown.DropdownEvent tmp = tempChoice.ChoiceChapterInput.onValueChanged;
					tempChoice.ChoiceChapterInput.onValueChanged = new TMP_Dropdown.DropdownEvent();

					tempChoice.ChoiceChapterInput.value = choice.OptionID;
					tempChoice.ChoiceChapter = choice.OptionID;
					tempChoice.ChoiceTextInput.text = choice.OptionText;

					tempChoice.ChoiceChapterInput.onValueChanged = tmp;
				}

				foreach (SentenceActionData sentenceAction in sentence.sentenceActions) {
					GameObject sentenceActionsObject = Instantiate(SentenceActionPrefab, tempSentence.SentenceActionsMenu.GetComponent<ItemMenu>().ItemList);

					sentenceActionsObject.TryGetComponent(out SentenceAction tempSentenceAction);

					tempSentenceAction.ActionTypeDropdown.value = sentenceAction.ActionType;

					characterNames = GetCharacterList();

					tempSentenceAction.CharacterNameDropdown.options.Clear();
					foreach (string characterName in characterNames) {
						TMP_Dropdown.OptionData name = new TMP_Dropdown.OptionData() { text = characterName };
						tempSentenceAction.CharacterNameDropdown.options.Add(name);
					}

					tempSentenceAction.CharacterNameDropdown.value = ReturnCharacterIndex(sentenceAction.CharacterName);

					List<string> characterStateNames = GetCharacterStates(sentenceAction.CharacterName);

					tempSentenceAction.StateNameDropdown.options.Clear();
					foreach (string characterState in characterStateNames) {
						TMP_Dropdown.OptionData name = new TMP_Dropdown.OptionData() { text = characterState };
						tempSentenceAction.StateNameDropdown.options.Add(name);
					}

					tempSentenceAction.StateNameDropdown.value = ReturnCharacterStateIndex(sentenceAction.CharacterName, sentenceAction.StateName);

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

					tempSentenceAction.BGMNameDropdown.options.Clear();

					List<string> BGMNames = GetBGMList();

					tempSentenceAction.BGMNameDropdown.options.Clear();
					foreach (string BGMName in BGMNames) {
						TMP_Dropdown.OptionData name = new TMP_Dropdown.OptionData() { text = BGMName };
						tempSentenceAction.CharacterNameDropdown.options.Add(name);
					}

					tempSentenceAction.BGMName = ReturnBGMIndex(sentenceAction.BGMName);

					tempSentenceAction.DelayInput.text = sentenceAction.Delay.ToString();

					tempSentenceAction.SetDropdownValues();

					tempSentenceAction.SetValues();
				}
			}

			chapterObject.GetComponent<Chapter>().UpdateMenuNames(ChapterMenu.MenuName);
		}

		foreach (Transform obj in ChapterMenu.ItemList) {
			obj.TryGetComponent(out Chapter chapter);
			if (chapter == null) { continue; }
			chapter.SetValues();
			foreach (Transform sentenceObj in chapter.GetSentencesMenu().ItemList) {
				if (sentenceObj.TryGetComponent(out Sentence sentence)) {
					sentence.SetValues();
					foreach (Transform choiceObj in sentence.ChoicesMenu.GetComponent<ItemMenu>().ItemList) {
						if (choiceObj.TryGetComponent(out Choice choice)) {
							choice.SetValue(choice.ChoiceTextInput);
							choice.SetValue(choice.ChoiceChapterInput);
						}
					}

					foreach (Transform sentenceActionObj in sentence.SentenceActionsMenu.GetComponent<ItemMenu>().ItemList) {
						if (sentenceActionObj.TryGetComponent(out SentenceAction sentenceAction)) {
							sentenceAction.SetValues();
						}
					}
				}
			}
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
		List<ArtworkData> artworks = new List<ArtworkData>();
		List<BGMData> BGMs = new List<BGMData>();

		foreach (Transform ch in ChapterMenu.ItemList) {
			ch.TryGetComponent(out Chapter chapter);

			if (chapter != null) {
				chapter.SetValue(chapter.ChapterNameInput);
				chapter.SetValue(chapter.NextChapterList);
				ChapterData tempChapter = new ChapterData();

				List<SentenceData> sentences = new List<SentenceData>();

				tempChapter.ChapterName = chapter.ChapterName;
				tempChapter.NextChapter = chapter.NextChapter;

				foreach (Transform sen in chapter.GetSentencesMenu().ItemList) {
					sen.TryGetComponent(out Sentence sentence);

					if (sentence != null) {
						sentence.SetValues();
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

						foreach (Transform choic in sentence.ChoicesMenu.GetComponent<ItemMenu>().ItemList) {
							if (choic.TryGetComponent(out Choice choice)) {
								choice.SetValue(choice.ChoiceTextInput);
								choice.SetValue(choice.ChoiceChapterInput);
								ChoiceOptionData tempChoice = new ChoiceOptionData();

								tempChoice.OptionID = choice.ChoiceChapter;
								tempChoice.OptionText = choice.ChoiceText;

								choices.Add(tempChoice);
							}
						}

						foreach (Transform sentAction in sentence.SentenceActionsMenu.GetComponent<ItemMenu>().ItemList) {
							if (sentAction.TryGetComponent(out SentenceAction sentenceAction)) {
								sentenceAction.SetValues();
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

			List<CharacterStateData> characterStates = new List<CharacterStateData>();

			foreach (Transform characState in character.GetCharacterStatesMenu().ItemList) {
				if (characState.TryGetComponent(out CharacterState characterState)) {
					CharacterStateData tempCharacterState = new CharacterStateData();

					tempCharacterState.StateName = characterState.StateName;
					tempCharacterState.StateType = characterState.StateType;
					tempCharacterState.BaseLayerImagePath = characterState.BaseLayerPath;
					tempCharacterState.ExpressionLayerImagePath = characterState.ExpressionLayerPath;

					characterStates.Add(tempCharacterState);
				}
			}

			tempCharacter.characterStates = characterStates.ToArray();

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

		foreach(Transform art in ArtworkMenu.ItemList) {
			if(art.TryGetComponent(out Artwork artwork)) {
				ArtworkData tempArtwork = new ArtworkData();

				tempArtwork.ArtworkName = artwork.ArtworkName;
				tempArtwork.ArtworkType = artwork.ArtworkType;
				tempArtwork.ArtworkPath = artwork.ArtworkPath;

				artworks.Add(tempArtwork);
			}
		}

		foreach(Transform bgmusic in BGMMenu.ItemList) {
			if(bgmusic.TryGetComponent(out BGM bgm)) {
				BGMData tempBGM = new BGMData();

				tempBGM.BGMName = bgm.BGMName;
				tempBGM.BGMPath = bgm.BGMPath;

				BGMs.Add(tempBGM);
			}
		}

		data.chapters = chapters.ToArray();
		data.characterData = characters.ToArray();
		data.artworkData = artworks.ToArray();
		data.bgmData = BGMs.ToArray();

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

		foreach (string name in characters.ToArray()) {
			if (name == "") {
				characters.Remove(name);
			}
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

	public List<string> GetCharacterStates(string CharacterName) {
		List<string> characterStates = new List<string>();

		foreach (Transform obj in CharacterMenu.ItemList) {
			if (obj.TryGetComponent(out Character character)) {
				if (character.CharacterName == CharacterName) {
					foreach (Transform charState in character.GetCharacterStatesMenu().ItemList) {
						if (charState.TryGetComponent(out CharacterState characterState)) { characterStates.Add(characterState.StateName); }
					}
				}
			}
		}

		return characterStates;
	}

	public int ReturnArtworkIndex(int ArtworkType, string ArtworkName) {
		List<string> artworks = GetArtworkList(ArtworkType);

		foreach(string artwork in artworks) { 
			if(artwork == ArtworkName) { return artworks.IndexOf(artwork); }
		}

		return 0;
	}

	public int ReturnCharacterStateIndex(string CharacterName, string StateName) {
		List<string> characterStates = GetCharacterStates(CharacterName);

		int index = 0;

		foreach (string state in characterStates) {
			if (state == StateName) { index = characterStates.IndexOf(state); }
		}

		return index;
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