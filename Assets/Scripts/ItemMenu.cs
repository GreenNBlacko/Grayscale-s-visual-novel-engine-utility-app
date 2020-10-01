using TMPro;
using UnityEngine;

public class ItemMenu : MonoBehaviour {
	public TMP_Text title;
	public string MenuName;
	public Transform ItemList;

	private MenuSystem menuSystem;
	
	public string ParentMenu;

	// Start is called before the first frame update
	void Start() {
		menuSystem = FindObjectOfType<MenuSystem>();
	}

	// Update is called once per frame
	void Update() {

	}

	public void LoadParentMenu() {
		menuSystem.LoadMenu(ParentMenu);
	}
}
