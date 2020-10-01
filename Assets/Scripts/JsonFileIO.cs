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
	public GameObject CharacterPrefab;

	[Space(15)]
	public SentencesData sentenceData;

	private string loadedFile = "";

	public void SaveFile() {
		if(loadedFile == "") { Debug.LogError("No file Loaded!"); return; }

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

			titleText.text = "Currently loaded file: </b>" + Path + "/Sentences.json" +  "</b>";

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
		} catch {

		}

		return paths;
	}

	public void OpenFileLoad() {
		List<string> projectNames = GetFilePathNames();

		LoadFileMenu.SetActive(true);

		foreach(string project in projectNames) {
			GameObject tmp = Instantiate(LoadProjectFilePrefab, LoadFileMenuContent.transform);
			tmp.GetComponent<ProjectFile>().FilePath = project;
			tmp.name = project;
		}
	}

	public void UpdateMenus(SentencesData data) {
		foreach(Transform obj in ChapterMenu.ItemList) {
			obj.TryGetComponent(out Chapter chapter);
			if(chapter == null) { continue; }
			chapter.RemoveItem();
		}

		foreach(ChapterData chapter in data.chapters) {
			Debug.Log(chapter.ChapterName);
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

			chapterObject.GetComponent<Chapter>().UpdateMenuNames(ChapterMenu.MenuName);

			foreach (SentenceData sentence in chapter.sentences) {
				GameObject sentencesObject = Instantiate(SentencePrefab, chapterObject.GetComponent<Chapter>().GetSentencesMenu().ItemList);
				sentencesObject.name = chapter.ChapterName + sentencesObject.transform.GetSiblingIndex();

				sentencesObject.TryGetComponent(out Sentence tempSentence);

				tempSentence.SentenceText.text = sentence.Text;
				tempSentence.ArtworkTypeDropDown.value = sentence.ArtworkType;
				if(sentence.ArtworkType == 1)
					tempSentence.ArtworkNameDropDown.value = sentence.BG_ID;
				else if (sentence.ArtworkType == 2)
					tempSentence.ArtworkNameDropDown.value = sentence.CG_ID;
				tempSentence.ChoiceToggle.isOn = sentence.Choice;
				tempSentence.VoicedToggle.isOn = sentence.Voiced;
				if(sentence.Voiced)
					tempSentence.VAClipInput.text = sentence.VoiceClip;
			}
		}

		foreach (Transform obj in ChapterMenu.ItemList) {
			obj.TryGetComponent(out Chapter chapter);
			if (chapter == null) { continue; }
			chapter.SetValues();
		}
	}

	public void UpdateNewFileList() {
		List<string> projectNames = GetFilePathNames();

		foreach(Transform project in NewFileMenuContent.transform) {
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
		foreach(Transform project in LoadFileMenuContent.transform) {
			Destroy(project.gameObject);
		}
		LoadFileMenu.SetActive(false);
	}

	public void OpenFileCreate() {
		List<string> projectNames = GetFilePathNames();

		NewFileMenu.SetActive(true);

		foreach (string project in projectNames) {
			if(project.Contains(NewFileInput.text)) {
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
		if(input.text == "") { return; }
		NewFile(input.text);
		CloseFileCreate();
	}

	public SentencesData GetData() {
		SentencesData data = new SentencesData();
		List<ChapterData> chapters = new List<ChapterData>();

		foreach(Transform ch in ChapterMenu.ItemList) {
			ch.TryGetComponent(out Chapter chapter);

			if(chapter != null) {
				ChapterData tempChapter = new ChapterData();

				List<SentenceData> sentences = new List<SentenceData>();

				tempChapter.ChapterName = chapter.ChapterName;
				tempChapter.NextChapter = chapter.NextChapter;

				foreach(Transform sen in chapter.GetSentencesMenu().ItemList) {
					sen.TryGetComponent(out Sentence sentence);

					if(sentence != null) {
						SentenceData tempSentence = new SentenceData();

						tempSentence.Name = sentence.CharacterName;
						tempSentence.Text = sentence.Text;
						tempSentence.ArtworkType = sentence.ArtworkType;
						if(sentence.ArtworkType == 1) {
							tempSentence.BG_ID = sentence.ArtworkName;
						} else if (sentence.ArtworkType == 2) {
							tempSentence.CG_ID = sentence.ArtworkName;
						}
						tempSentence.Choice = sentence.Choice;
						tempSentence.Voiced = sentence.voiced;
						if(sentence.voiced) {
							tempSentence.VoiceClip = sentence.VAClipPath;
						}

						sentences.Add(tempSentence);
					}
				}
				tempChapter.sentences = sentences.ToArray();

				chapters.Add(tempChapter);
			}
		}

		data.chapters = chapters.ToArray();

		return data;
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

	public string Delay;
}

[Serializable]
public class CharacterData {
	public string characterName;

	public CharacterStateData[] characterStates;

	public bool useSeperateColors;

	public string colorHex;
	public string nameColorHex;
	public string textColorHex;

	public int gradientType;

	public bool useSeperateGradientColors;

	public string colorGradientHex;
	public string nameColorGradientHex;
	public string textColorGradientHex;
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