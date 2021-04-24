using TMPro;
using UnityEngine;

public class PreviewSentence : MonoBehaviour {
	public TMP_Text CharacterName;
	public TMP_Text SentenceText;

	public Transform ItemList;

	public void SetValues(string name, string text, VertexGradient nameColor, VertexGradient textColor) {
		CharacterName.text = name;
		SentenceText.text = text;

		CharacterName.colorGradient = nameColor;
		SentenceText.colorGradient = textColor;
	}
}
