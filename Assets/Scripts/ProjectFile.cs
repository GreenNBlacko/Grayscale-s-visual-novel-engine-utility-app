using TMPro;
using UnityEngine;

public class ProjectFile : MonoBehaviour {
	public TMP_Text ProjectName;

	public string FilePath;

	private JsonFileIO jsonFileIO;

	// Start is called before the first frame update
	void Start() {
		jsonFileIO = FindObjectOfType<JsonFileIO>();
		ProjectName.text = FilePath;
	}

	public void LoadFile() {
		jsonFileIO.LoadFile(FilePath);
		jsonFileIO.CloseFileLoad();
	}

	public void SaveFile() {
		jsonFileIO.NewFile(FilePath);
		ProjectName.text = "";
		jsonFileIO.CloseFileCreate();
	}
}
