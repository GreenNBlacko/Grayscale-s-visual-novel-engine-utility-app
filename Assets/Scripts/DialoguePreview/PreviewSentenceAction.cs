using TMPro;
using UnityEngine;

public class PreviewSentenceAction : MonoBehaviour {
	public TMP_Text ActionName;

	public void SetValues(string name) {
		ActionName.text = name;
	}
}
