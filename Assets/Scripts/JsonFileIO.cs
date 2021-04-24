using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JsonFileIO : MonoBehaviour {
	[Header("Main Menus")]
	public TMP_Text titleText;
	public TMP_Text EditFileMenuTitleText;

	[Header("Load File Menu")]
	public GameObject LoadFileMenu;
	public GameObject LoadFileMenuContent;
	public GameObject LoadProjectFilePrefab;
	public Button LoadFileMenuConvertModeButton;

	[Header("New File Menu")]
	public TMP_InputField NewFileInput;
	public GameObject NewFileMenu;
	public GameObject NewFileMenuContent;
	public GameObject NewProjectPrefab;

	[Header("Menus")]
	public ItemMenu ChapterMenu;
	public ItemMenu CharacterMenu;
	public ItemMenu ArtworkMenu;
	public ItemMenu BGMMenu;

	[Header("Prefabs")]
	public GameObject ChapterPrefab;
	public GameObject SentencePrefab;
	public GameObject ChoicePrefab;
	public GameObject SentenceActionPrefab;
	public GameObject CharacterPrefab;
	public GameObject CharacterStatePrefab;
	public GameObject ArtworkPrefab;
	public GameObject BGMPrefab;

	[Space(15)]
	public SentencesDataV2 sentenceData;

	private bool convertModeOn = false;

	private string FilePathV1 = "DataPath" + "/sentenceData/" + "PathReplace" + "/Sentences.json";
	private string FilePathV2 = "DataPath" + "/sentenceData/" + "PathReplace" + "/SentencesV2.json";

	private string TempDataPath = "DataPath" + "/sentenceData/" + "PathReplace" + "/tempData.json";

	public enum ActionsV1 { AddCharacterToScene, MoveCharacter, ChangeCharacterState, RemoveCharacterFromScene, PlayBGM, Delay };
	public enum ActionsV2 { AddCharacterToScene, MoveCharacter, ShowTransition, ShowBG, ShowCG, RemoveCharacterFromScene, ChangeCharacterState, Delay, WaitUntilActionIsFinished, SetCharacterIndex, PlayBGM };

	private string loadedFile = "";

	private void Start() {
		FilePathV1 = FilePathV1.Replace("DataPath", Application.persistentDataPath);
		FilePathV2 = FilePathV2.Replace("DataPath", Application.persistentDataPath);
		TempDataPath = TempDataPath.Replace("DataPath", Application.persistentDataPath);

		if(PlayerPrefs.HasKey("OpenFile")) {
			LoadFile(PlayerPrefs.GetString("OpenFile", ""));
		}

		StartCoroutine(SaveFileTempCounter());
	}

	public void SaveFile() {
		string filePath = FilePathV2.Replace("PathReplace", loadedFile);

		if (loadedFile == "") { Debug.LogError("No file Loaded!"); return; }

		sentenceData = GetData();

		string jsonData = JsonUtility.ToJson(sentenceData, true);
		File.WriteAllText(filePath, jsonData);

		File.Delete(TempDataPath.Replace("PathReplace", loadedFile));
	}

	public void LoadFile(string Path) {
		if(Path == "") {
			return;
		}

		if(File.Exists(TempDataPath.Replace("PathReplace", Path))) {
			LoadFileTemp(Path);
			return;
		}

		string filePath = FilePathV2.Replace("PathReplace", Path);

		if (File.Exists(filePath)) {
			StreamReader reader = new StreamReader(filePath);

			string jsonData = reader.ReadToEnd();
			sentenceData = JsonUtility.FromJson<SentencesDataV2>(jsonData);

			loadedFile = Path;

			reader.Close();

			titleText.text = "Currently loaded file: </b>" + Path + "/SentencesV2.json" + "</b>";
			EditFileMenuTitleText.text = "Editing file: </b>" + Path + "/SentencesV2.json" + "</b>";

			UpdateMenus(JsonUtility.FromJson<SentencesDataV2>(jsonData));

			PlayerPrefs.SetString("OpenFile", Path);
		} else {
			string jsonData = JsonUtility.ToJson(sentenceData, true);

			File.WriteAllText(filePath, jsonData);

			LoadFile(Path);
		}
	}

	public IEnumerator SaveFileTempCounter() {
		while(true) {
			yield return new WaitForEndOfFrame();

			if (File.Exists(TempDataPath.Replace("PathReplace", loadedFile))) {
				SaveFileTemp();
			}
		}
	}

	public void SaveFileTemp() {
		if (loadedFile == "") { return; }

		string filePath = TempDataPath.Replace("PathReplace", loadedFile);

		sentenceData = GetData();

		if(!File.Exists(filePath)) {
			FileInfo file = new FileInfo(filePath);
			file.Directory.Create();
		}

		string jsonData = JsonUtility.ToJson(sentenceData, true);
		File.WriteAllText(filePath, jsonData);
	}

	public void LoadFileTemp(string Path) {
		string filePath = TempDataPath.Replace("PathReplace", Path);

		StreamReader reader = new StreamReader(filePath);

		string jsonData = reader.ReadToEnd();
		sentenceData = JsonUtility.FromJson<SentencesDataV2>(jsonData);

		loadedFile = Path;

		reader.Close();

		titleText.text = "Currently loaded file: </b>" + loadedFile + "/SentencesV2.json" + "</b>";
		EditFileMenuTitleText.text = "Editing file: </b>" + loadedFile + "/SentencesV2.json" + "</b>";

		UpdateMenus(sentenceData);
	}

	#region Conversion
	public void LoadFileV1(string Path) {
		string filePath = FilePathV1.Replace("PathReplace", Path);

		if (File.Exists(filePath)) {
			StreamReader reader = new StreamReader(filePath);

			string jsonData = reader.ReadToEnd();

			loadedFile = Path;

			ConvertV1ToV2(JsonUtility.FromJson<SentencesDataV1>(jsonData));

			reader.Close();

			File.Delete(filePath);
		}
	}

	public void ConvertV1ToV2(SentencesDataV1 dataV1) {
		SentencesDataV2 dataV2 = new SentencesDataV2();

		List<ChapterDataV2> chapterData = new List<ChapterDataV2>();
		List<CharacterDataV2> characterData = new List<CharacterDataV2>();
		List<ArtworkDataV2> artworkData = new List<ArtworkDataV2>();
		List<BGMDataV2> bgmData = new List<BGMDataV2>();

		foreach (ChapterDataV1 chapter in dataV1.chapters) {
			ChapterDataV2 tempChapter = new ChapterDataV2();

			tempChapter.ChapterName = chapter.ChapterName;
			tempChapter.NextChapter = chapter.NextChapter;

			List<SentenceDataV2> sentences = new List<SentenceDataV2>();

			foreach (SentenceDataV1 sentence in chapter.sentences) {
				SentenceDataV2 tempSentence = new SentenceDataV2();

				tempSentence.Name = sentence.Name;
				tempSentence.Text = sentence.Text;
				tempSentence.Choice = sentence.Choice;

				List<ChoiceOptionDataV2> choices = new List<ChoiceOptionDataV2>();

				foreach (ChoiceOptionDataV1 choice in sentence.choiceOptions) {
					ChoiceOptionDataV2 tempChoice = new ChoiceOptionDataV2();

					tempChoice.OptionID = choice.OptionID;
					tempChoice.OptionText = choice.OptionText;

					choices.Add(tempChoice);
				}

				tempSentence.choiceOptions = choices.ToArray();

				List<SentenceActionDataV2> sentenceActions = new List<SentenceActionDataV2>();

				switch (sentence.ArtworkType) {
					case 1: {
							SentenceActionDataV2 tempAction = new SentenceActionDataV2();

							tempAction.ActionType = ActionsV2.ShowBG;
							tempAction.BG = sentence.BG_ID;

							sentenceActions.Add(tempAction);
							break;
						}
					case 2: {
							SentenceActionDataV2 tempAction = new SentenceActionDataV2();

							tempAction.ActionType = ActionsV2.ShowCG;
							tempAction.CG = sentence.CG_ID;

							sentenceActions.Add(tempAction);
							break;
						}
				}

				tempSentence.Voiced = sentence.Voiced;
				tempSentence.VoiceClip = sentence.VoiceClip;

				foreach (SentenceActionDataV1 sentenceAction in sentence.sentenceActions) {
					sentenceActions.Add(ConvertV1SentenceActionToV2(sentenceAction));
				}

				tempSentence.sentenceActions = sentenceActions.ToArray();

				sentences.Add(tempSentence);
			}

			tempChapter.sentences = sentences.ToArray();

			chapterData.Add(tempChapter);
		}

		foreach (CharacterDataV1 character in dataV1.characterData) {
			CharacterDataV2 tempCharacter = new CharacterDataV2();

			tempCharacter.characterName = character.characterName;

			List<CharacterStateDataV2> characterStates = new List<CharacterStateDataV2>();

			foreach (CharacterStateDataV1 characterState in character.characterStates) {
				CharacterStateDataV2 tempCharacterState = new CharacterStateDataV2();

				tempCharacterState.StateName = characterState.StateName;
				tempCharacterState.StateType = characterState.StateType;
				tempCharacterState.BaseLayerImagePath = characterState.BaseLayerImagePath;
				tempCharacterState.ExpressionLayerImagePath = characterState.ExpressionLayerImagePath;

				characterStates.Add(tempCharacterState);
			}

			tempCharacter.characterStates = characterStates.ToArray();

			tempCharacter.useSeperateColors = character.useSeperateColors;

			tempCharacter.colorHex = character.colorHex;
			tempCharacter.colorRGB = character.colorRGB;
			tempCharacter.nameColorHex = character.nameColorHex;
			tempCharacter.nameColorRGB = character.nameColorRGB;
			tempCharacter.textColorHex = character.textColorHex;
			tempCharacter.textColorRGB = character.textColorRGB;

			tempCharacter.gradientType = character.gradientType;

			tempCharacter.useSeperateGradientColors = character.useSeperateGradientColors;

			tempCharacter.colorGradientHex = character.colorGradientHex;
			tempCharacter.colorGradientRGB = character.colorGradientRGB;
			tempCharacter.nameColorGradientHex = character.nameColorGradientHex;
			tempCharacter.nameColorGradientRGB = character.nameColorGradientRGB;
			tempCharacter.textColorGradientHex = character.textColorGradientHex;
			tempCharacter.textColorGradientRGB = character.textColorGradientRGB;

			characterData.Add(tempCharacter);
		}

		foreach (ArtworkDataV1 artwork in dataV1.artworkData) {
			ArtworkDataV2 tempArtwork = new ArtworkDataV2();

			tempArtwork.ArtworkName = artwork.ArtworkName;
			tempArtwork.ArtworkType = artwork.ArtworkType;
			tempArtwork.ArtworkPath = artwork.ArtworkPath;

			artworkData.Add(tempArtwork);
		}

		foreach (BGMDataV1 bgm in dataV1.bgmData) {
			BGMDataV2 tempBGM = new BGMDataV2();

			tempBGM.BGMName = bgm.BGMName;
			tempBGM.BGMPath = bgm.BGMPath;

			bgmData.Add(tempBGM);
		}

		dataV2.chapters = chapterData.ToArray();
		dataV2.characterData = characterData.ToArray();
		dataV2.artworkData = artworkData.ToArray();
		dataV2.bgmData = bgmData.ToArray();

		string filePath = FilePathV2.Replace("PathReplace", loadedFile);

		string jsonData = JsonUtility.ToJson(dataV2, true);
		File.WriteAllText(filePath, jsonData);

		LoadFile(loadedFile);
	}

	public SentenceActionDataV2 ConvertV1SentenceActionToV2(SentenceActionDataV1 sentenceActionDataV1) {
		SentenceActionDataV2 sentenceActionDataV2 = new SentenceActionDataV2();

		switch (sentenceActionDataV1.ActionType) {
			case (int)ActionsV1.AddCharacterToScene: {
					sentenceActionDataV2.ActionType = ActionsV2.AddCharacterToScene;
					break;
				}
			case (int)ActionsV1.MoveCharacter: {
					sentenceActionDataV2.ActionType = ActionsV2.MoveCharacter;
					break;
				}
			case (int)ActionsV1.ChangeCharacterState: {
					sentenceActionDataV2.ActionType = ActionsV2.ChangeCharacterState;
					break;
				}
			case (int)ActionsV1.RemoveCharacterFromScene: {
					sentenceActionDataV2.ActionType = ActionsV2.RemoveCharacterFromScene;
					break;
				}
			case (int)ActionsV1.PlayBGM: {
					sentenceActionDataV2.ActionType = ActionsV2.PlayBGM;
					break;
				}
			case (int)ActionsV1.Delay: {
					sentenceActionDataV2.ActionType = ActionsV2.Delay;
					break;
				}
		}

		sentenceActionDataV2.FadeIn = sentenceActionDataV1.FadeIn;
		sentenceActionDataV2.FadeOut = sentenceActionDataV1.FadeOut;
		sentenceActionDataV2.Transition = sentenceActionDataV1.Transition;

		sentenceActionDataV2.FadeSpeed = sentenceActionDataV1.FadeSpeed;
		sentenceActionDataV2.TransitionSpeed = sentenceActionDataV1.TransitionSpeed;

		sentenceActionDataV2.startingPosition = sentenceActionDataV1.startingPosition;
		sentenceActionDataV2.customStartingPosition = sentenceActionDataV1.customStartingPosition;

		sentenceActionDataV2.position = sentenceActionDataV1.position;

		sentenceActionDataV2.CharacterName = sentenceActionDataV1.CharacterName;
		sentenceActionDataV2.StateName = sentenceActionDataV1.StateName;

		sentenceActionDataV2.BGMName = sentenceActionDataV1.BGMName;

		sentenceActionDataV2.Delay = sentenceActionDataV1.Delay;

		return sentenceActionDataV2;
	}
	#endregion

	public void NewFile(string Path) {
		sentenceData = new SentencesDataV2();

		FileInfo file1 = new FileInfo(Application.persistentDataPath);
		file1.Directory.Create();

		FileInfo file2 = new FileInfo(Application.persistentDataPath + "/sentenceData/" + Path);
		file2.Directory.Create();

		FileInfo file3 = new FileInfo(Application.persistentDataPath + "/sentenceData/" + Path + "/SentencesV2.json");
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
				foreach (string file in System.IO.Directory.GetFiles(d)) {
					if (file.Contains("SentencesV2.json")) {
						paths.Add(Path.GetFileName(d));
					}
				}
			}
		}
		catch {

		}

		return paths;
	}

	public List<string> GetFilePathNamesV1() {
		List<string> paths = new List<string>();
		try {
			foreach (string d in Directory.GetDirectories(Application.persistentDataPath + "/sentenceData")) {
				foreach (string file in System.IO.Directory.GetFiles(d)) {
					if (file.Contains("Sentences.json")) {
						paths.Add(Path.GetFileName(d));
					}
				}
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

	public void ToggleConvertMode() {
		convertModeOn = !convertModeOn;

		List<string> projectNames = new List<string>();

		foreach (Transform project in LoadFileMenuContent.transform) {
			Destroy(project.gameObject);
		}

		if (convertModeOn) {
			projectNames = GetFilePathNamesV1();
			LoadFileMenuConvertModeButton.image.color = new Color32 { r = 0, g = 221, b = 119, a = 255 };
		} else {
			projectNames = GetFilePathNames();
			LoadFileMenuConvertModeButton.image.color = new Color32 { r = 255, g = 78, b = 78, a = 255 };
		}

		foreach (string project in projectNames) {
			GameObject tmp = Instantiate(LoadProjectFilePrefab, LoadFileMenuContent.transform);
			tmp.GetComponent<ProjectFile>().FilePath = project;
			tmp.name = project;
			tmp.GetComponent<ProjectFile>().ConvertModeOn = convertModeOn;
		}
	}

	public void UpdateMenus(SentencesDataV2 data) {
		for (int i = 7; i < FindObjectOfType<MenuSystem>().menus.Count; i++) {
			Destroy(FindObjectOfType<MenuSystem>().menus[i].MenuObject.gameObject);
		}
		FindObjectOfType<MenuSystem>().menus.RemoveRange(7, FindObjectOfType<MenuSystem>().menus.Count - 7);
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

		foreach (CharacterDataV2 character in data.characterData) {

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

			foreach (CharacterStateDataV2 characterState in character.characterStates) {
				GameObject characterStateObject = Instantiate(CharacterStatePrefab, characterObject.GetComponent<Character>().GetCharacterStatesMenu().ItemList);

				characterStateObject.GetComponent<CharacterState>().StateNameInput.text = characterState.StateName;
				characterStateObject.GetComponent<CharacterState>().StateTypeDropdown.value = characterState.StateType;
				characterStateObject.GetComponent<CharacterState>().BaseLayerInput.text = characterState.BaseLayerImagePath;
				characterStateObject.GetComponent<CharacterState>().ExpressionLayerInput.text = characterState.ExpressionLayerImagePath;

				characterStateObject.GetComponent<CharacterState>().SetValues();
			}

			characterObject.GetComponent<Character>().SetValues();
		}

		foreach (ArtworkDataV2 artwork in data.artworkData) {
			GameObject artworkObject = Instantiate(ArtworkPrefab, ArtworkMenu.ItemList);

			artworkObject.GetComponent<Artwork>().ArtworkNameInput.text = artwork.ArtworkName;
			artworkObject.GetComponent<Artwork>().ArtworkTypeDropdown.value = artwork.ArtworkType;
			artworkObject.GetComponent<Artwork>().ArtworkPathInput.text = artwork.ArtworkPath;

			artworkObject.GetComponent<Artwork>().SetValues();
		}

		foreach (BGMDataV2 bgm in data.bgmData) {
			GameObject bgmObject = Instantiate(BGMPrefab, BGMMenu.ItemList);

			bgmObject.GetComponent<BGM>().BGMNameInput.text = bgm.BGMName;
			bgmObject.GetComponent<BGM>().BGMPathInput.text = bgm.BGMPath;

			bgmObject.GetComponent<BGM>().SetValues();
		}

		foreach (ChapterDataV2 chapter in data.chapters) {
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

			foreach (SentenceDataV2 sentence in chapter.sentences) {
				GameObject sentencesObject = Instantiate(SentencePrefab, chapterObject.GetComponent<Chapter>().GetSentencesMenu().ItemList);

				sentencesObject.TryGetComponent(out Sentence tempSentence);

				List<string> characterNames = GetCharacterList();

				tempSentence.NameDropDown.options.Clear();
				foreach (string characterName in characterNames) {
					TMP_Dropdown.OptionData name = new TMP_Dropdown.OptionData() { text = characterName };
					tempSentence.NameDropDown.options.Add(name);
				}

				tempSentence.NameDropDown.value = ReturnCharacterIndex(sentence.Name);

				tempSentence.NameOverrideToggle.isOn = sentence.OverrideName;

				tempSentence.DisplayNameText.text = sentence.DisplayName;

				tempSentence.SentenceText.text = sentence.Text;

				tempSentence.ChoiceToggle.isOn = sentence.Choice;
				tempSentence.VoicedToggle.isOn = sentence.Voiced;
				if (sentence.Voiced)
					tempSentence.VAClipInput.text = sentence.VoiceClip;

				chapterObject.GetComponent<Chapter>().CreateSentenceSubmenus();

				tempSentence.UpdateLists();
				tempSentence.SetValues();

				foreach (ChoiceOptionDataV2 choice in sentence.choiceOptions) {
					GameObject choiceObject = Instantiate(ChoicePrefab, tempSentence.ChoicesMenu.GetComponent<ItemMenu>().ItemList);

					choiceObject.TryGetComponent(out Choice tempChoice);

					tempChoice.ChoiceChapter = choice.OptionID;

					tempChoice.UpdateLists();

					tempChoice.ChoiceTextInput.text = choice.OptionText;
				}

				foreach (SentenceActionDataV2 sentenceAction in sentence.sentenceActions) {
					GameObject sentenceActionsObject = Instantiate(SentenceActionPrefab, tempSentence.SentenceActionsMenu.GetComponent<ItemMenu>().ItemList);

					sentenceActionsObject.TryGetComponent(out SentenceAction tempSentenceAction);

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

					tempSentenceAction.ActionType = sentenceAction.ActionType;

					tempSentenceAction.CharacterName = sentenceAction.CharacterName;

					tempSentenceAction.StateName = sentenceAction.StateName;

					tempSentenceAction.BGMName = ReturnBGMIndex(sentenceAction.BGMName);

					tempSentenceAction.SetDropdownValues();

					tempSentenceAction.DelayInput.text = sentenceAction.Delay.ToString();

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
		convertModeOn = true;

		ToggleConvertMode();

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

	public SentencesDataV2 GetData() {
		SentencesDataV2 data = new SentencesDataV2();
		List<ChapterDataV2> chapters = new List<ChapterDataV2>();
		List<CharacterDataV2> characters = new List<CharacterDataV2>();
		List<ArtworkDataV2> artworks = new List<ArtworkDataV2>();
		List<BGMDataV2> BGMs = new List<BGMDataV2>();

		foreach (Transform ch in ChapterMenu.ItemList) {
			ch.TryGetComponent(out Chapter chapter);

			if (chapter != null) {
				chapter.SetValue(chapter.ChapterNameInput);
				chapter.SetValue(chapter.NextChapterList);
				ChapterDataV2 tempChapter = new ChapterDataV2();

				List<SentenceDataV2> sentences = new List<SentenceDataV2>();

				tempChapter.ChapterName = chapter.ChapterName;
				tempChapter.NextChapter = chapter.NextChapter;

				foreach (Transform sen in chapter.GetSentencesMenu().ItemList) {
					sen.TryGetComponent(out Sentence sentence);

					if (sentence != null) {
						sentence.SetValues();
						SentenceDataV2 tempSentence = new SentenceDataV2();

						List<ChoiceOptionDataV2> choices = new List<ChoiceOptionDataV2>();
						List<SentenceActionDataV2> sentenceActions = new List<SentenceActionDataV2>();

						tempSentence.Name = sentence.CharacterName;
						tempSentence.OverrideName = sentence.OverrideName;
						tempSentence.DisplayName = sentence.DisplayName;
						tempSentence.Text = sentence.Text;
						tempSentence.Choice = sentence.Choice;
						tempSentence.Voiced = sentence.voiced;
						if (sentence.voiced) {
							tempSentence.VoiceClip = sentence.VAClipPath;
						}

						foreach (Transform choic in sentence.ChoicesMenu.GetComponent<ItemMenu>().ItemList) {
							if (choic.TryGetComponent(out Choice choice)) {
								choice.SetValue(choice.ChoiceTextInput);
								choice.SetValue(choice.ChoiceChapterInput);
								ChoiceOptionDataV2 tempChoice = new ChoiceOptionDataV2();

								tempChoice.OptionID = choice.ChoiceChapter;
								tempChoice.OptionText = choice.ChoiceText;

								choices.Add(tempChoice);
							}
						}

						foreach (Transform sentAction in sentence.SentenceActionsMenu.GetComponent<ItemMenu>().ItemList) {
							if (sentAction.TryGetComponent(out SentenceAction sentenceAction)) {
								sentenceAction.SetValues();
								SentenceActionDataV2 tempSentenceAction = new SentenceActionDataV2();

								tempSentenceAction.ActionType = sentenceAction.ActionType;
								tempSentenceAction.CharacterName = sentenceAction.CharacterName;
								tempSentenceAction.StateName = sentenceAction.StateName;
								tempSentenceAction.Delay = sentenceAction.Delay;
								tempSentenceAction.FadeIn = sentenceAction.FadeIn;
								tempSentenceAction.FadeOut = sentenceAction.FadeOut;
								tempSentenceAction.Transition = sentenceAction.Transition;
								tempSentenceAction.FadeSpeed = sentenceAction.FadeSpeed;
								tempSentenceAction.BG = sentenceAction.BG;
								tempSentenceAction.CG = sentenceAction.CG;
								tempSentenceAction.CharacterIndex = sentenceAction.CharacterIndex;
								tempSentenceAction.RunOnlyOnce = sentenceAction.RunOnlyOnce;
								tempSentenceAction.EnterScene = sentenceAction.EnterScene;
								tempSentenceAction.ExitScene = sentenceAction.ExitScene;
								tempSentenceAction.startingPosition = sentenceAction.StartingPosition;
								tempSentenceAction.customStartingPosition = sentenceAction.StartPosition;
								tempSentenceAction.TransitionSpeed = sentenceAction.TransitionSpeed;
								tempSentenceAction.position = sentenceAction.Position;
								tempSentenceAction.BGMName = ReturnBGMName(sentenceAction.BGMName);

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

			CharacterDataV2 tempCharacter = new CharacterDataV2();

			List<CharacterStateDataV2> characterStates = new List<CharacterStateDataV2>();

			foreach (Transform characState in character.GetCharacterStatesMenu().ItemList) {
				if (characState.TryGetComponent(out CharacterState characterState)) {
					CharacterStateDataV2 tempCharacterState = new CharacterStateDataV2();

					tempCharacterState.StateName = characterState.StateName;
					tempCharacterState.StateType = characterState.StateType;
					tempCharacterState.BaseLayerImagePath = characterState.BaseLayerPath;
					tempCharacterState.ExpressionLayerImagePath = characterState.ExpressionLayerPath;
					tempCharacterState.Advanced = characterState.Advanced;
					tempCharacterState.Offset = characterState.Offset;

					characterStates.Add(tempCharacterState);
				}
			}

			tempCharacter.characterStates = characterStates.ToArray();

			tempCharacter.useSeperateColors = character.UseSeperateColors;
			tempCharacter.gradientType = character.GradientType;
			tempCharacter.useSeperateGradientColors = character.UseSeperateGradientColors;

			tempCharacter.characterName = character.CharacterName;

			tempCharacter.nameColorRGB = new byte[3] { character.NameColor.r, character.NameColor.g, character.NameColor.b };
			tempCharacter.nameColorHex = ColorUtility.ToHtmlStringRGB(character.NameColor);

			tempCharacter.textColorRGB = new byte[3] { character.TextColor.r, character.TextColor.g, character.TextColor.b };
			tempCharacter.textColorHex = ColorUtility.ToHtmlStringRGB(character.TextColor);

			tempCharacter.colorRGB = new byte[3] { character.Color.r, character.Color.g, character.Color.b };
			tempCharacter.colorHex = ColorUtility.ToHtmlStringRGB(character.Color);


			tempCharacter.nameColorGradientRGB = new byte[3] { character.NameGradientColor.r, character.NameGradientColor.g, character.NameGradientColor.b };
			tempCharacter.nameColorGradientHex = ColorUtility.ToHtmlStringRGB(character.NameGradientColor);

			tempCharacter.textColorGradientRGB = new byte[3] { character.TextGradientColor.r, character.TextGradientColor.g, character.TextGradientColor.b };
			tempCharacter.textColorGradientHex = ColorUtility.ToHtmlStringRGB(character.TextGradientColor);

			tempCharacter.colorGradientRGB = new byte[3] { character.GradientColor.r, character.GradientColor.g, character.GradientColor.b };
			tempCharacter.colorGradientHex = ColorUtility.ToHtmlStringRGB(character.GradientColor);

			characters.Add(tempCharacter);
		}

		foreach (Transform art in ArtworkMenu.ItemList) {
			if (art.TryGetComponent(out Artwork artwork)) {
				ArtworkDataV2 tempArtwork = new ArtworkDataV2();

				tempArtwork.ArtworkName = artwork.ArtworkName;
				tempArtwork.ArtworkType = artwork.ArtworkType;
				tempArtwork.ArtworkPath = artwork.ArtworkPath;

				artworks.Add(tempArtwork);
			}
		}

		foreach (Transform bgmusic in BGMMenu.ItemList) {
			if (bgmusic.TryGetComponent(out BGM bgm)) {
				BGMDataV2 tempBGM = new BGMDataV2();

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
			if (!child.TryGetComponent(out Chapter chapter)) { child.SetAsLastSibling(); continue; }
			chapterNames.Add(chapter.ChapterName);
		}

		return chapterNames;
	}

	public List<string> GetCharacterList() {
		List<string> characters = new List<string>();

		foreach (Transform temp in CharacterMenu.ItemList) {
			temp.TryGetComponent(out Character character);

			if (character == null) { temp.SetAsLastSibling(); continue; }
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

			if (artwork == null) { temp.SetAsLastSibling(); continue; }
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
				continue;
			}
			temp.SetAsLastSibling();
		}

		return BGMs;
	}

	public List<string> GetCharacterStates(string CharacterName) {
		List<string> characterStates = new List<string>();

		foreach (Transform obj in CharacterMenu.ItemList) {
			if (obj.TryGetComponent(out Character character)) {
				if (character.CharacterName == CharacterName) {
					foreach (Transform charState in character.GetCharacterStatesMenu().ItemList) {
						if (charState.TryGetComponent(out CharacterState characterState)) { characterStates.Add(characterState.StateName); continue; }
						charState.SetAsLastSibling();
					}
				}
			}
		}

		return characterStates;
	}

	public string ReturnBGMName(int Index) {
		List<string> bgms = GetBGMList();

		try {
			return bgms[Index];
		}
		catch {
			try {
				return bgms.Last();
			}
			catch {
				return "";
			}
		}
	}

	public int ReturnArtworkIndex(int ArtworkType, string ArtworkName) {
		List<string> artworks = GetArtworkList(ArtworkType);

		foreach (string artwork in artworks) {
			if (artwork == ArtworkName) { return artworks.IndexOf(artwork); }
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
public class SentencesDataV1 {
	public ChapterDataV1[] chapters;
	public CharacterDataV1[] characterData;
	public ArtworkDataV1[] artworkData;
	public BGMDataV1[] bgmData;
}

[Serializable]
public class ChapterDataV1 {
	public string ChapterName;
	public int NextChapter = -1;
	public SentenceDataV1[] sentences;
}

[Serializable]
public class SentenceDataV1 {
	public string Name;

	public string Text;

	public int ArtworkType;

	public int BG_ID;
	public int CG_ID;

	public bool Choice;

	public ChoiceOptionDataV1[] choiceOptions;

	public bool Voiced;
	public string VoiceClip;

	public SentenceActionDataV1[] sentenceActions;
}

[Serializable]
public class ChoiceOptionDataV1 {
	public int OptionID;
	public string OptionText;
}

[Serializable]
public class SentenceActionDataV1 {
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
public class CharacterDataV1 {
	public string characterName;

	public CharacterStateDataV1[] characterStates;

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
public class CharacterStateDataV1 {
	public string StateName;
	public int StateType;
	public string BaseLayerImagePath;
	public string ExpressionLayerImagePath;
}

[Serializable]
public class ArtworkDataV1 {
	public string ArtworkName;
	public int ArtworkType;
	public string ArtworkPath;
}

[Serializable]
public class BGMDataV1 {
	public string BGMName;
	public string BGMPath;
}

[Serializable]
public class SentencesDataV2 {
	public ChapterDataV2[] chapters;
	public CharacterDataV2[] characterData;
	public ArtworkDataV2[] artworkData;
	public BGMDataV2[] bgmData;
}

[Serializable]
public class ChapterDataV2 {
	public string ChapterName;
	public int NextChapter = -1;
	public SentenceDataV2[] sentences;
}

[Serializable]
public class SentenceDataV2 {
	public string Name;

	public bool OverrideName;

	public string DisplayName;

	public string Text;

	public bool Choice;

	public ChoiceOptionDataV2[] choiceOptions;

	public bool Voiced;
	public string VoiceClip;

	public SentenceActionDataV2[] sentenceActions;
}

[Serializable]
public class ChoiceOptionDataV2 {
	public int OptionID;
	public string OptionText;
}

[Serializable]
public class SentenceActionDataV2 {
	public JsonFileIO.ActionsV2 ActionType;

	public string CharacterName;
	public string StateName;

	public float Delay;

	public bool FadeIn;
	public bool FadeOut;
	public bool Transition;

	public float FadeSpeed;

	public int BG;
	public int CG;

	public int CharacterIndex;

	public bool RunOnlyOnce;

	public bool EnterScene;

	public bool ExitScene;

	public int startingPosition;
	public Vector2 customStartingPosition;

	public float TransitionSpeed;

	public Vector2 position;

	public string BGMName;
}

[Serializable]
public class CharacterDataV2 {
	public string characterName;

	public CharacterStateDataV2[] characterStates;

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
public class CharacterStateDataV2 {
	public string StateName;
	public int StateType;
	public string BaseLayerImagePath;
	public string ExpressionLayerImagePath;
	public bool Advanced;
	public Vector2 Offset;
}

[Serializable]
public class ArtworkDataV2 {
	public string ArtworkName;
	public int ArtworkType;
	public string ArtworkPath;
}

[Serializable]
public class BGMDataV2 {
	public string BGMName;
	public string BGMPath;
}