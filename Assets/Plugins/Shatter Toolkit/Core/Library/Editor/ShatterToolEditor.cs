// Shatter Toolkit
// Copyright 2015 Gustav Olsson
using UnityEditor;

namespace ShatterToolkit
{
	[CustomEditor(typeof(ShatterTool))]
	public class ShatterToolEditor : Editor
	{
		private ShatterTool _self;

		private void OnEnable()
		{
			_self = (ShatterTool) target;
		}

		public override void OnInspectorGUI()
		{
			EditorGUILayout.LabelField("Generation", _self.generation.ToString());
			base.OnInspectorGUI();
		}
	}
}