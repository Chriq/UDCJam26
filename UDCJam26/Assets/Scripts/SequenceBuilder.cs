using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SequenceBuilder : MonoBehaviour {
	public int numBars;
	public int subdivisions = 4;
	public bool shortenList = false;

	[HideInInspector] public RhythmBlockType[] beats;

	public void OnValidate() {
		if(numBars * subdivisions > beats.Length || shortenList) {
			RhythmBlockType[] arr = new RhythmBlockType[numBars * subdivisions];
			for(int i = 0; i < Mathf.Min(beats.Length, arr.Length); i++) {
				arr[i] = beats[i];
			}

			if(beats.Length != arr.Length) {
				shortenList = false;
			}

			beats = arr;
		}

		numBars = beats.Length / subdivisions;
	}
}

public enum RhythmBlockType {
    EMPTY,
    HIT,
    NO_HIT
}

[CustomEditor(typeof(SequenceBuilder))]
public class ListUpdaterEditor : Editor {
	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		EditorGUILayout.Space();

		SequenceBuilder listUpdater = (SequenceBuilder) target;

		EditorGUI.BeginChangeCheck();
		if(EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(listUpdater, "List Size Change");
			listUpdater.OnValidate();
		}

		EditorGUILayout.Space();

		listUpdater.beats = DrawListElements(listUpdater.beats, listUpdater.subdivisions);
	}

	private RhythmBlockType[] DrawListElements(RhythmBlockType[] listElements, int subdivisions) {
		EditorGUILayout.LabelField("Beats");
		for(int i = 0; i < listElements.Length; i++) {
			if(i % subdivisions == 0)
				DrawSeparator((i / subdivisions) + 1);

			listElements[i] = (RhythmBlockType)EditorGUILayout.EnumPopup($"Beat {i+1}", listElements[i]);
		}

		return listElements;
	}

	private void DrawSeparator(int bar) {
		GUILayout.Space(8);

		GUIStyle separatorStyle = new GUIStyle(GUI.skin.label);
		separatorStyle.fontStyle = FontStyle.Bold;
		separatorStyle.alignment = TextAnchor.UpperLeft;

		GUILayout.Label("Bar " + bar, separatorStyle);

		GUILayout.Box(GUIContent.none, GUILayout.ExpandWidth(true), GUILayout.Height(2));
		//Rect rect = GUILayoutUtility.GetLastRect();
		//EditorGUI.DrawRect(new Rect(rect.x, rect.y + rect.height, rect.width, 1), Color.black);
	}
}