namespace ShatterToolkit
{
	using UnityEngine;
	using UnityEditor;

	public class ShatterToolUVEditor : EditorWindow
	{
		public float zoomLevel = 1.0f;
		public TargetUvMapper mapper;

		public static void Open(TargetUvMapper aMapper)
		{
			ShatterToolUVEditor window = (ShatterToolUVEditor) EditorWindow.GetWindow(typeof(ShatterToolUVEditor));
			window.titleContent = new GUIContent("UV Editor");

			window.mapper = aMapper;
			window.zoomLevel = 1.0f;
			window.Show();
		}

		private void OnGUI()
		{
			if (mapper != null && mapper.material != null && mapper.material.mainTexture != null)
			{
				Texture shadowTex = mapper.material.mainTexture;
				// Texture shadowTex = mapper.material.GetTexture("_DetailAlbedoMap");

				EditorGUILayout.LabelField("Texture UV:");
				EditorGUILayout.HelpBox("Tip: Hold ALT/OPTION and drag labels X/Y/W/H for precise adjusting. Hold SHIFT to adjust values in bigger steps.", MessageType.Info);
				mapper.uvRect = EditorGUILayout.RectField(mapper.uvRect);

				Rect rectClamped = mapper.uvRect;

				rectClamped.width = Mathf.Clamp(mapper.uvRect.width, 0.0f, 1.0f);
				rectClamped.height = Mathf.Clamp(mapper.uvRect.height, 0.0f, 1.0f);

				mapper.uvRect = rectClamped;
				mapper.targetStart = new Vector2(rectClamped.x, rectClamped.y);
				mapper.targetSize = new Vector2(rectClamped.width, rectClamped.height);

				EditorGUILayout.LabelField("Zoom: " + (int) (zoomLevel * 100) + "%");
				zoomLevel = EditorGUILayout.Slider(zoomLevel, 0.0f, 2.0f);

				Vector2 imagePos = new Vector2(20.0f, 150.0f);
				Rect imageRect = new Rect(imagePos.x, imagePos.y, shadowTex.width * zoomLevel, shadowTex.height * zoomLevel);

				EditorGUI.DrawRect(imageRect, new Color(0.7f, 0.7f, 0.7f, 0.7f));
				EditorGUI.DrawPreviewTexture(imageRect, shadowTex, mapper.material, ScaleMode.ScaleToFit);
				DrawZones(shadowTex, new Rect(mapper.uvRect.x, 1.0f - mapper.uvRect.y, mapper.uvRect.width, mapper.uvRect.height), imagePos);

				if (GUI.changed)
				{
					EditorUtility.SetDirty(mapper);
				}
			}
		}

		private void DrawZones(Texture aTex, Rect aRect, Vector2 aOffset)
		{
			float x1 = aRect.x;
			float x2 = aRect.x + aRect.width;

			float y1 = aRect.y;
			float y2 = aRect.y - aRect.height;

			if ((Mathf.Floor(x1) == Mathf.Floor(x2)) && (Mathf.Floor(y1) == Mathf.Floor(y2)))
			{
				Rect newRect = new Rect(aRect);
				x1 = x1 - Mathf.Floor(x1);
				y1 = y1 - Mathf.Floor(y1);
				newRect.x = x1;
				newRect.y = y1;

				DrawRect(aTex, newRect, aOffset);
			}
			else
			{
				if ((Mathf.Floor(x1) != Mathf.Floor(x2)) && (Mathf.Floor(y1) != Mathf.Floor(y2)))
				{
					float newRectY1 = Mathf.Min(y1 - Mathf.Floor(y1), y2 - Mathf.Floor(y2));
					float newRectY2 = Mathf.Max(y1 - Mathf.Floor(y1), y2 - Mathf.Floor(y2));

					float newRectX1 = Mathf.Min(x1 - Mathf.Floor(x1), x2 - Mathf.Floor(x2));
					float newRectX2 = Mathf.Max(x1 - Mathf.Floor(x1), x2 - Mathf.Floor(x2));
				
					Rect rect1 = new Rect(aRect);
					rect1.y = newRectY1;
					rect1.x = 0;
					rect1.height = newRectY1;
					rect1.width = newRectX1;
					
					DrawRect(aTex, rect1, aOffset);

					rect1.y = newRectY1;
					rect1.x = newRectX2;
					rect1.height = newRectY1;
					rect1.width = 1.0f - newRectX2;;
					
					DrawRect(aTex, rect1, aOffset);

					Rect rect2 = new Rect(aRect);
					rect2.y = 1.0f;
					rect2.x = newRectX2;
					rect2.height = 1.0f - newRectY2;
					rect2.width = 1.0f - newRectX2;
					
					DrawRect(aTex, rect2, aOffset);

					rect2.y = 1.0f;
					rect2.x = 0.0f;
					rect2.height = 1.0f - newRectY2;
					rect2.width = newRectX1;
					
					DrawRect(aTex, rect2, aOffset);
				}
				else
				{
					if (Mathf.Floor(x1) != Mathf.Floor(x2))
					{
						float newRectX1 = Mathf.Min(x1 - Mathf.Floor(x1), x2 - Mathf.Floor(x2));
						float newRectX2 = Mathf.Max(x1 - Mathf.Floor(x1), x2 - Mathf.Floor(x2));
						float newY = y1 - Mathf.Floor(y1);

						Rect rect1 = new Rect(aRect);
						rect1.x = 0;
						rect1.y = newY;
						rect1.width = newRectX1;

						DrawRect(aTex, rect1, aOffset);

						Rect rect2 = new Rect(aRect);
						rect2.x = newRectX2;
						rect2.y = newY;
						rect2.width = 1.0f - newRectX2;
						
						DrawRect(aTex, rect2, aOffset);
					} 

					if (Mathf.Floor(y1) != Mathf.Floor(y2))
					{
						float newRectY1 = Mathf.Min(y1 - Mathf.Floor(y1), y2 - Mathf.Floor(y2));
						float newRectY2 = Mathf.Max(y1 - Mathf.Floor(y1), y2 - Mathf.Floor(y2));
						float newX = x1 - Mathf.Floor(x1);

						Rect rect1 = new Rect(aRect);
						rect1.y = newRectY1;
						rect1.x = newX;
						rect1.height = newRectY1;
						
						DrawRect(aTex, rect1, aOffset);
						
						Rect rect2 = new Rect(aRect);
						rect2.y = 1.0f;
						rect2.x = newX;
						rect2.height = 1.0f - newRectY2;
						
						DrawRect(aTex, rect2, aOffset);
					}
				}
			}
		}

		private void DrawRect(Texture aTex, Rect aRect, Vector2 aOffset)
		{
			Rect clampedRect = new Rect(aRect);

			clampedRect.x = Mathf.Clamp(clampedRect.x, 0.0f, 1.0f);
			clampedRect.y = Mathf.Clamp(clampedRect.y, 0.0f, 1.0f);
			clampedRect.width = Mathf.Clamp(clampedRect.width, 0.0f, 1.0f);
			clampedRect.height = Mathf.Clamp(clampedRect.height, 0.0f, 1.0f);

			float endCoordX = clampedRect.x + clampedRect.width;
			if (endCoordX > 1.0f)
			{
				clampedRect.width = 1.0f - clampedRect.x;
			}

			float endCoordY = clampedRect.y - clampedRect.height;
			if (endCoordY < 0.0f)
			{
				clampedRect.height = clampedRect.y;
			}

			EditorGUI.DrawRect(
				new Rect(clampedRect.x * aTex.width * zoomLevel+ aOffset.x,
					clampedRect.y * aTex.height * zoomLevel + aOffset.y,
					clampedRect.width * aTex.width * zoomLevel, 
					1.0f), 
				new Color(0.5f, 0.9f, 0.0f)
			);


			EditorGUI.DrawRect(
				new Rect(clampedRect.x * aTex.width * zoomLevel + clampedRect.width * aTex.width * zoomLevel + aOffset.x,
					clampedRect.y * aTex.height * zoomLevel+ aOffset.y,
					1.0f,
					-clampedRect.height * aTex.height * zoomLevel),
				new Color(0.5f, 0.9f, 0.0f)
			);


			EditorGUI.DrawRect(
				new Rect(clampedRect.x * aTex.width * zoomLevel+ aOffset.x, 
					clampedRect.y * aTex.height * zoomLevel -  clampedRect.height * aTex.height * zoomLevel + aOffset.y,
					clampedRect.width * aTex.width * zoomLevel, 
					-1.0f), 
				new Color(0.5f, 0.9f, 0.0f)
			);

			EditorGUI.DrawRect(
				new Rect(clampedRect.x * aTex.width * zoomLevel+ aOffset.x,
					clampedRect.y * aTex.height * zoomLevel + aOffset.y,
					1.0f,
					-clampedRect.height * aTex.height * zoomLevel),
				new Color(0.5f, 0.9f, 0.0f)
			);
		}
	}
}