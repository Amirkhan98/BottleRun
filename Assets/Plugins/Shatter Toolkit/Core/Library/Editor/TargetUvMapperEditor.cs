namespace ShatterToolkit
{
	using UnityEngine;
	using UnityEditor;
	
	[CustomEditor(typeof(TargetUvMapper))]
	public class TargetUvMapperEditor : Editor
	{
		private TargetUvMapper _self;
	
		#region Unity Calls
	
		private void OnEnable()
		{
			_self = (TargetUvMapper) target;
		}
	
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			if (GUILayout.Button("Open UV Editor"))
			{
				ShatterToolUVEditor.Open(_self);
			}
		}
	
		#endregion
	}
}