using System;
using Leap;
using Leap.Unity;
using UnityEditor;
using UnityEngine;

namespace PXL.Interaction.Detection.Editor {

	/// <summary>
	/// Custom editor script which exposes the necessary variables to the Inspector and 
	/// provides an easier to use interface than the default one.
	/// </summary>
	[CustomEditor(typeof(DetectHandPose))]
	public class DetectHandPoseEditor : UnityEditor.Editor {

		public override void OnInspectorGUI() {
			var t = (DetectHandPose)target;

			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Observed Hands: ", new GUIStyle(EditorStyles.largeLabel) { fontStyle = FontStyle.Bold }, GUILayout.MaxWidth(150));

			for (var i = 0; i < t.HandModels.Count; i++) {
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(t.HandModels[i] == null ? "Missing" : t.HandModels[i].gameObject.name, GUILayout.MinWidth(120), GUILayout.MaxWidth(120));
				t.HandModels[i] =
					(RigidHand)
						EditorGUILayout.ObjectField("", t.HandModels[i], typeof(RigidHand), true,
							GUILayout.MaxWidth(150));

				if (GUILayout.Button(new GUIContent("X", "Removes this item from the list"), GUILayout.MaxWidth(20)))
					t.HandModels.RemoveAt(i);
				EditorGUILayout.EndHorizontal();
			}
			if (t.HandModels.Count == 0)
				EditorGUILayout.LabelField("None",
					new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.BoldAndItalic },
					GUILayout.MaxWidth(50));

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("New Hand: ", EditorStyles.boldLabel, GUILayout.MaxWidth(75));
			t.NewHand = (RigidHand)EditorGUILayout.ObjectField("", t.NewHand, typeof(RigidHand), true, GUILayout.MaxWidth(150));

			EditorGUI.BeginDisabledGroup(t.NewHand == null);
			if (GUILayout.Button(new GUIContent("Add", "Adds the given hand to the list"), GUILayout.MaxWidth(75)))
				t.AddHand();
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();

			t.DetectFingerPoses = EditorGUILayout.BeginToggleGroup("Detect Finger Poses", t.DetectFingerPoses);

			CreateAllFingerLimitToggle("All fingers extracted", ref t.AllExtended, t.AllContracted);
			CreateAllFingerLimitToggle("All fingers contracted", ref t.AllContracted, t.AllExtended);

			EditorGUI.BeginDisabledGroup(t.AllExtended || t.AllContracted);

			EditorGUILayout.LabelField("Individual Finger Poses", EditorStyles.boldLabel, GUILayout.MaxWidth(150));
			for (var i = 0; i < t.FingerPoses.Count; i++)
				CreatePoseEntry(t, i);
			EditorGUILayout.Space();

			if (t.FingerPoses.Count < Enum.GetNames(typeof(Finger.FingerType)).Length - 1)
				CreateNewArea(t);
			else
				CreateAllFingersUsedMessage();

			EditorGUI.EndDisabledGroup();

			EditorGUILayout.EndToggleGroup();

			t.DetectGrabStrength = EditorGUILayout.BeginToggleGroup("Detect Grab Strength", t.DetectGrabStrength);

			CreateGrabStrengthLimit("Minimum Grab strength", ref t.LimitMinGrabStrength, ref t.MinGrabStrength, 0,
				t.LimitMaxGrabStrength ? t.MaxGrabStrength : 1);
			CreateGrabStrengthLimit("Maximum Grab strength", ref t.LimitMaxGrabStrength, ref t.MaxGrabStrength,
				t.LimitMinGrabStrength ? t.MinGrabStrength : 0, 1);

			EditorGUILayout.EndToggleGroup();
		}

		private static void CreateAllFingersUsedMessage() {
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			EditorGUILayout.LabelField("All fingers set up",
				new GUIStyle(EditorStyles.helpBox) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.BoldAndItalic },
				GUILayout.MaxWidth(110));
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}

		private static void CreateNewArea(DetectHandPose t) {
			EditorGUILayout.BeginVertical();
			EditorGUILayout.LabelField("New Pose: ", EditorStyles.boldLabel, GUILayout.MaxWidth(75));
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Finger ", GUILayout.MaxWidth(50));
			t.NewPose.FingerType =
				(Finger.FingerType)EditorGUILayout.EnumPopup("", t.NewPose.FingerType, GUILayout.MaxWidth(100));
			EditorGUILayout.LabelField("extended ", GUILayout.MaxWidth(60));
			t.NewPose.Extended = EditorGUILayout.Toggle("", t.NewPose.Extended, GUILayout.MaxWidth(20));

			if (GUILayout.Button(new GUIContent("Add", "Adds the given fingerpose to the list"), GUILayout.MaxWidth(75)))
				t.AddFingerPose();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
		}

		private static void CreateAllFingerLimitToggle(string title, ref bool limitFlag, bool displayDisabledFlag) {
			EditorGUI.BeginDisabledGroup(displayDisabledFlag);
			EditorGUILayout.BeginHorizontal();
			limitFlag = EditorGUILayout.Toggle(new GUIContent("", ""), limitFlag, GUILayout.MaxWidth(15));
			EditorGUILayout.LabelField(title, GUILayout.MaxWidth(200));
			EditorGUILayout.EndHorizontal();
			EditorGUI.EndDisabledGroup();
		}

		private static void CreatePoseEntry(DetectHandPose t, int i) {
			EditorGUILayout.BeginHorizontal();
			var p = t.FingerPoses[i];
			EditorGUILayout.LabelField("Finger ", GUILayout.MaxWidth(50));
			p.FingerType =
				(Finger.FingerType)EditorGUILayout.EnumPopup("", p.FingerType, GUILayout.MaxWidth(75));
			EditorGUILayout.LabelField("extended", GUILayout.MaxWidth(75));
			p.Extended = EditorGUILayout.Toggle(p.Extended, GUILayout.MaxWidth(25));
			t.FingerPoses[i] = p;

			if (GUILayout.Button(new GUIContent("X", "Removes this item from the list"), GUILayout.MaxWidth(20),
				GUILayout.MaxHeight(20))) {
				t.FingerPoses.RemoveAt(i);
			}

			EditorGUILayout.EndHorizontal();
		}

		private static void CreateGrabStrengthLimit(string title, ref bool limitFlag, ref float limit, float min, float max) {
			EditorGUILayout.BeginHorizontal();
			limitFlag = EditorGUILayout.Toggle("", limitFlag, GUILayout.MaxWidth(15));
			EditorGUILayout.LabelField(title, EditorStyles.boldLabel, GUILayout.MaxWidth(160));
			EditorGUI.BeginDisabledGroup(!limitFlag);
			limit = Mathf.Clamp(
				EditorGUILayout.FloatField(limit, GUILayout.MaxWidth(50)),
				min,
				max);
			EditorGUILayout.EndHorizontal();
			EditorGUI.EndDisabledGroup();
		}

	}

}