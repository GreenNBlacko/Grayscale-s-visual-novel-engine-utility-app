using TMPro;
using UnityEngine;

public class Choice : MonoBehaviour {
	public string ChoiceText;
	public int ChoiceChapter;

	public TMP_InputField ChoiceTextInput;
	public TMP_Dropdown ChoiceChapterInput;

	public MenuSystem menuSystem;

	private void Start() {
		menuSystem = FindObjectOfType<MenuSystem>();
		SetValue(ChoiceTextInput);
		SetValue(ChoiceChapterInput);
	}

	public void SetValue(TMP_InputField input) {
		ChoiceText = input.text;
	}

	public void SetValue(TMP_Dropdown input) {
		ChoiceChapter = input.value;
	}

	public void RemoveItem() {
		Destroy(gameObject);
	}
}
