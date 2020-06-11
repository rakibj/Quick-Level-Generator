using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Rakib
{
    public class TextureToLevel : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Texture2D texture2D;
        [SerializeField] private Material targetMaterial;
        [SerializeField] private bool gpuInstancing = true;
        [SerializeField] private bool createAssetInProject = false;
        [SerializeField] private bool keepPrefabConnection = true;
        [SerializeField] private string materialSaveDestination = "Assets/_Project/Art/3D/Materials/Pieces/";
        [SerializeField] [Range(0f, 1f)] private float alphaThreshold = .2f;
        [SerializeField] [Range(0f, 0.99f)] private float quality = 0.5f;
        [SerializeField] private float finalParentScale = 0.25f;

        private Dictionary<Color32, Material> m_colorMats = new Dictionary<Color32, Material>();

        public void GenerateLevel()
        {
            m_colorMats.Clear();
            var x = 0;
            var y = 0;
            var amount = 0;
            var materialIndex = 0;
            var parent = new GameObject("Grp_" + texture2D.name);

            if (createAssetInProject)
            {
                var directory = materialSaveDestination + "/" + texture2D.name;
                if (Directory.Exists(directory))
                    FileUtil.DeleteFileOrDirectory(directory);

                Directory.CreateDirectory(directory);
            }

            foreach (Color32 color in texture2D.GetPixels32())
            {
                if (x >= texture2D.width)
                {
                    x = 0;
                    y++;
                }

                var alpha = alphaThreshold * 256;
                if (color.a > alpha)
                {
                    amount++;

                    MeshRenderer mesh;
                    if (keepPrefabConnection)
                    {
                        mesh = PrefabUtility.InstantiatePrefab(meshRenderer) as MeshRenderer;
                        mesh.transform.position = parent.transform.position + parent.transform.right * x +
                                                  parent.transform.up * y;
                        mesh.transform.parent = parent.transform;
                    }
                    else
                    {
                        mesh = Instantiate(meshRenderer,
                        parent.transform.position + parent.transform.right * x + parent.transform.up * y,
                        Quaternion.identity,
                        parent.transform);
                    }

                    //Assign material to mesh
                    var fetchFromDictionary = false;
                    foreach (var colorMat in m_colorMats)
                    {
                        var diffR = Mathf.Abs(color.r - colorMat.Key.r);
                        var diffG = Mathf.Abs(color.g - colorMat.Key.g);
                        var diffB = Mathf.Abs(color.b - colorMat.Key.b);

                        var totalDifference = diffR + diffG + diffB;
                        var totalAmount = 256 * 3f;
                        var colorDifference = totalDifference / totalAmount;

                        var allowed = 1f - quality;
                        if (colorDifference < allowed)
                        {
                            mesh.sharedMaterial = colorMat.Value;
                            fetchFromDictionary = true;
                            break;
                        }
                        
                    }
                    if (!fetchFromDictionary)
                    {
                        var material = Instantiate(targetMaterial);
                        material.name = "M_" + texture2D.name + "_" + materialIndex;
                        material.color = color;
                        material.enableInstancing = gpuInstancing;
                        m_colorMats.Add(color, material);
                        mesh.sharedMaterial = material;
                        
                        //Save material to Assets
                        if (createAssetInProject)
                        {
                            AssetDatabase.CreateAsset(material,
                                materialSaveDestination + "/" + texture2D.name + "/" + material.name + ".mat");
                        }

                        materialIndex++;
                    }
                }

                x++;
            }

            parent.transform.localScale = Vector3.one * finalParentScale;
            Debug.Log("Material count: " + m_colorMats.Count);
        }
    }
}