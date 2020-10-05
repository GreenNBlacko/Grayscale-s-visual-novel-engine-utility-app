using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Choice : MonoBehaviour {
	public string ChoiceText;
	public int ChoiceChapter;

	public TMP_InputField ChoiceTextInput;
	public TMP_Dropdown ChoiceChapterInput;

	public MenuSystem menuSystem;
	private JsonFileIO fileIO;

	private void Start() {
		menuSystem = FindObjectOfType<MenuSystem>();
		fileIO = FindObjectOfType<JsonFileIO>();
		SetValue(ChoiceTextInput);
		SetValue(ChoiceChapterInput);
	}

	private void Update() {
		fileIO = FindObjectOfType<JsonFileIO>();
		List<string> options = fileIO.ReturnAllChapters();
		options.RemoveAt(0);

		int tmp = ChoiceChapter;

		if (options.Count != ChoiceChapterInput.options.Count) {
			ChoiceChapterInput.ClearOptions();
			ChoiceChapterInput.AddOptions(options);
		}

		for (int i = 0; i < ChoiceChapterInput.options.Count; i++) {
			if (i >= options.Count) {
				ChoiceChapterInput.ClearOptions();
				ChoiceChapterInput.AddOptions(options);
				break;
			}
			if (options[i] != ChoiceChapterInput.options[i].text) {
				ChoiceChapterInput.ClearOptions();
				ChoiceChapterInput.AddOptions(options);
			}
		}

		ChoiceChapterInput.value = tmp;

		if (ChoiceChapterInput.value >= ChoiceChapterInput.options.Count) {
			ChoiceChapterInput.value = 0;
		}
	}

	public void SetValue(TMP_InputField input) {
		ChoiceText = input.text;
	}

	public void SetValue(TMP_Dropdown input) {
		Update();
		ChoiceChapter = input.value;
	}

	public void RemoveItem() {
		Destroy(gameObject);
	}
}
