using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Rakib
{
    [CustomEditor(typeof(MeshToLevel))]
    public class MeshToLevelEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var meshToLevel = (MeshToLevel) target;
            if (GUILayout.Button("Generate"))
            {
                meshToLevel.Generate();
            }
        }
    }
}