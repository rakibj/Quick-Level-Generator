using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Rakib
{
    public class TextureToLevelWindow : EditorWindow
    {
        [MenuItem("Rakib/TextureToLevel")]
        static void OpenWindow()
        {
            TextureToLevelWindow window = (TextureToLevelWindow) GetWindow(typeof(TextureToLevelWindow));
            window.minSize = new Vector2(300, 300);
            window.Show();
        }
    }
}