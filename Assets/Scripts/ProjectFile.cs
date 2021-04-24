using TMPro;
using UnityEngine;

public class ProjectFile : MonoBehaviour {
	public TMP_Text ProjectName;

	public string FilePath;

	public bool ConvertModeOn;

	private JsonFileIO jsonFileIO;

	// Start is called before the first frame update
	void Start() {
		jsonFileIO = FindObjectOfType<JsonFileIO>();
		ProjectName.text = FilePath;
	}

	public void LoadFile() {
		if(ConvertModeOn) {
			jsonFileIO.LoadFileV1(FilePath);
		} else {
			jsonFileIO.LoadFile(FilePath);
		}
		jsonFileIO.CloseFileLoad();
	}

	public void SaveFile() {
		jsonFileIO.NewFile(FilePath);
		ProjectName.text = "";
		jsonFileIO.CloseFileCreate();
	}
}
