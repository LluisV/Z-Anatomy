using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode] public class ToDoList : MonoBehaviour {	
	[Serializable] public class TextObject {
		public string s = "empty content item";
		public enum State {Default, Bug, Active, Finished};
		public State state;
		public bool show = true;
		public bool deleteQuestion = false;
	}
	
	public string title = "LIST TITLE";
	public List <TextObject> textList = new List<TextObject>();

	void Awake(){
		if (textList.Count == 0) {
			TextObject t = new TextObject ();
			textList.Add (t);
		}
	}
	
	public void CreateNewText(){
		textList.Add (new TextObject());
	}

	public void RequestDelete (int id){
		textList [id].deleteQuestion = true;
	}
	
	public void DeleteText (int id){
		textList.Remove (textList [id]);
	}

	public void SortText(){
		for (int x = 0; x < textList.Count - 1; x++) {
			for (int i = 0; i < textList.Count - 1; i++) {
				TextObject swap = null;
				if ((int)textList[i].state > (int)textList[i + 1].state){
					swap = textList[i];
					textList[i] = textList[i + 1];
					textList[i + 1] = swap;
				}
			}
		}
	}

	public void MoveUp (int id){
		TextObject swap = textList[id];
		textList[id] = textList[id - 1];
		textList[id - 1] = swap;
	}

	public void MoveDown (int id){
		TextObject swap = textList[id];
		textList[id] = textList[id + 1];
		textList[id + 1] = swap;
	}
}