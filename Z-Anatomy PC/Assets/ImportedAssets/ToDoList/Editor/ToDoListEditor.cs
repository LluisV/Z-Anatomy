using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(ToDoList))]
public class ToDoListEditor : Editor {
	[SerializeField] ToDoList script;
	
	void OnEnable(){
		script = (ToDoList)target;
	}

	// reset all X questions to standard
	void OnDisable(){
		refreshRequests ();
	}

	void refreshRequests(){
		for (int i = 0; i < script.textList.Count; i++) {
			script.textList[i].deleteQuestion = false;
		}
	}
	
	public override void OnInspectorGUI() {
		EditorUtility.SetDirty (target);

		GUI.color = Color.white;
		script.title = EditorGUILayout.TextField(script.title, getTitleStyle());

		EditorGUILayout.Space ();

		for (int i = 0; i < script.textList.Count; i++) {
			ToDoList.TextObject t = script.textList[i];

			EditorGUILayout.BeginHorizontal ();

			// the foldout text line
			GUI.color = getColor(t.state);
			string headline = t.s.Split("\n"[0])[0];
			t.show = Foldout(t.show, headline, true, getEditStyle(getColor(t.state)));

			if (!EditorApplication.isPlaying) {


				// the enum popup
				GUI.color = getColor(t.state);
				t.state = (ToDoList.TextObject.State) EditorGUILayout.EnumPopup(t.state, GUILayout.MaxWidth(66));

				GUI.color = Color.white;
				EditorGUI.BeginDisabledGroup (i < 1);
				if (GUILayout.Button ("˄", new GUIStyle(EditorStyles.miniButtonLeft), GUILayout.MaxWidth(25), GUILayout.MinWidth(25), GUILayout.MaxHeight(14), GUILayout.MinHeight(14))) {
					refreshRequests();
					script.MoveUp (i);
				}
				EditorGUI.EndDisabledGroup ();

				EditorGUI.BeginDisabledGroup (i > script.textList.Count - 2);
				if (GUILayout.Button ("˅", new GUIStyle(EditorStyles.miniButtonMid), GUILayout.MaxWidth(25), GUILayout.MinWidth(25), GUILayout.MaxHeight(14), GUILayout.MinHeight(14))) {
					refreshRequests();
					script.MoveDown (i);
				}
				EditorGUI.EndDisabledGroup ();

				// the delete button
				if (t.deleteQuestion){
					GUI.color = Color.red;
					if (GUILayout.Button ("YES?", new GUIStyle(EditorStyles.miniButtonRight), GUILayout.MaxWidth(45), GUILayout.MaxHeight(14))) {
						refreshRequests();
						script.DeleteText (i);
					}
				} else {
					GUI.color = (Color.red + Color.white) / 2f;
					if (GUILayout.Button ("X", new GUIStyle(EditorStyles.miniButtonRight), GUILayout.MaxWidth(45), GUILayout.MaxHeight(14))) {
						refreshRequests();
						script.RequestDelete (i);
					}
				}
			}
			EditorGUILayout.EndHorizontal ();

			// the foldout text area
			GUI.color = getColor(t.state);
			if (t.show){
				if (!EditorApplication.isPlaying){
					t.s = EditorGUILayout.TextArea(t.s);
				} else {
					EditorStyles.label.wordWrap = true;
					EditorGUILayout.LabelField(t.s/*, getPlayStyle(getColor(t.state))*/);
				}
			}

			EditorGUILayout.Space ();
		}

		if (!EditorApplication.isPlaying) {
			EditorGUILayout.BeginHorizontal ();
			// the ADD button
			GUI.color = new Color (0.54f, 0.68f, 0.95f);
			if (GUILayout.Button ("ADD ITEM", GUILayout.MaxWidth (100), GUILayout.MinWidth (100), GUILayout.MaxHeight (25), GUILayout.MinHeight (25))) {
				refreshRequests();
				script.CreateNewText ();
			}

			// the SORT button
			GUI.color = Color.white;
			if (GUILayout.Button ("SORT", GUILayout.MaxWidth (45), GUILayout.MinWidth (45), GUILayout.MaxHeight (25), GUILayout.MinHeight (25))) {
				refreshRequests();
				script.SortText ();
			}
			EditorGUILayout.EndHorizontal ();
		}
	}

	Color getColor(ToDoList.TextObject.State state){
		Color c = Color.white;
		if (state == ToDoList.TextObject.State.Active){
			c = new Color(1f, 0.85f, 0.4f);
		} else if (state == ToDoList.TextObject.State.Finished){
			c = new Color(0.70f, 1f, 0.50f);
		} else if (state == ToDoList.TextObject.State.Bug){
			c = new Color(1f, 0.5f, 0.8f);
		}
		return c;
	}

	GUIStyle getEditStyle(Color c){
		GUIStyle style = new GUIStyle(EditorStyles.foldout);
		style.fontStyle = FontStyle.Bold;

		Color myStyleColor ;
		if (c == Color.white) {
			myStyleColor = Color.black;
		} else {
			myStyleColor = (c * 2f + Color.black) / 3f;
		}

		style.normal.textColor = myStyleColor;
		style.onNormal.textColor = myStyleColor;
		style.hover.textColor = myStyleColor;
		style.onHover.textColor = myStyleColor;
		style.focused.textColor = myStyleColor;
		style.onFocused.textColor = myStyleColor;
		style.active.textColor = myStyleColor;
		style.onActive.textColor = myStyleColor;

		return style;
	}

	GUIStyle getPlayStyle(Color c){
		GUIStyle style = new GUIStyle(EditorStyles.label);
		
		Color myStyleColor ;
		if (c == Color.white) {
			myStyleColor = Color.black;
		} else {
			myStyleColor = (c * 2f + Color.black) / 3f;
		}
		
		style.normal.textColor = myStyleColor;
		style.onNormal.textColor = myStyleColor;
		style.hover.textColor = myStyleColor;
		style.onHover.textColor = myStyleColor;
		style.focused.textColor = myStyleColor;
		style.onFocused.textColor = myStyleColor;
		style.active.textColor = myStyleColor;
		style.onActive.textColor = myStyleColor;
		
		return style;
	}

	GUIStyle getTitleStyle(){
		GUIStyle style = new GUIStyle(EditorStyles.textField);
		style.fontStyle = FontStyle.Bold;
		
		return style;
	}

	public static bool Foldout(bool foldout, GUIContent content, bool toggleOnLabelClick, GUIStyle style)
	{
		Rect position = GUILayoutUtility.GetRect(40f, 40f, 16f, 16f, style);
		// EditorGUI.kNumberW == 40f but is internal
		return EditorGUI.Foldout(position, foldout, content, toggleOnLabelClick, style);
	}

	public static bool Foldout(bool foldout, string content, bool toggleOnLabelClick, GUIStyle style)
	{
		return Foldout(foldout, new GUIContent(content), toggleOnLabelClick, style);
	}
}
#endif