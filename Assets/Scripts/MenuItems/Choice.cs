using TMPro;
using UnityEngine;

public class Choice : MonoBehaviour {
	public string ChoiceText;
	public int ChoiceChapter;

	public TMP_InputField ChoiceTextInput;
	public TMP_Dropdown ChoiceChapterInput;

	private void Start() {
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
		Destroy(this.gameObject);
	}
}
