using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Rakib
{
    [CustomEditor(typeof(TextureToLevel))]
    public class TextureToLevelEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var textureToLevel = (TextureToLevel) target;
            if (GUILayout.Button("Generate"))
            {
                textureToLevel.GenerateLevel();
            }
        }
    }
}