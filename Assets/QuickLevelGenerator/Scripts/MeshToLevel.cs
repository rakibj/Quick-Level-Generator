using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MeshToLevel : MonoBehaviour
{
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer prefab;
    [SerializeField] private Material material;
    [SerializeField] private Color topColor;
    [SerializeField] private Color bottomColor;
    [SerializeField] private bool keepPrefabConnection = true;
    [SerializeField] private bool gpuInstancing = true;
    [SerializeField] private bool createAssetInProject = false;
    [SerializeField] private string materialSaveDestination = "Assets/_Project/Art/3D/Materials/Pieces/";
    
    [SerializeField] [Range(0f, 0.99f)] private float quality = 0.9f;
    private Dictionary<float, Material> m_heightMats = new Dictionary<float, Material>();

    private MeshRenderer[] m_meshRenderers;
    public void Generate()
    {
        m_heightMats.Clear();
        if (createAssetInProject)
        {
            var directory = materialSaveDestination + "/" + meshFilter.gameObject.name;
            if (Directory.Exists(directory))
                FileUtil.DeleteFileOrDirectory(directory);

            Directory.CreateDirectory(directory);
        }
        var materialIndex = 0;
        var vertices = meshFilter.sharedMesh.vertices;
        var parent = new GameObject("Grp_" + meshFilter.gameObject.name);
        
        var localToWorld = meshFilter.transform.localToWorldMatrix;

        var minX = Mathf.Infinity;
        var minY = Mathf.Infinity;
        var minZ = Mathf.Infinity;
        var maxX = -Mathf.Infinity;
        var maxY = -Mathf.Infinity;
        var maxZ = -Mathf.Infinity;
        
        m_meshRenderers = new MeshRenderer[vertices.Length];
        
        for (var i = 0; i < vertices.Length; i++)
        {
            var vertex = vertices[i];
            var worldPos = localToWorld.MultiplyPoint3x4(vertex);

            MeshRenderer meshRenderer;
            if (keepPrefabConnection)
            {
                meshRenderer = PrefabUtility.InstantiatePrefab(prefab) as MeshRenderer;
                meshRenderer.transform.position = worldPos;
                meshRenderer.transform.parent = parent.transform;
            }
            else
            {
                meshRenderer = Instantiate(prefab, worldPos, Quaternion.identity, parent.transform);
            }
            meshRenderer.material = material;
            m_meshRenderers[i] = meshRenderer;

            if (worldPos.x < minX) minX = worldPos.x;
            if (worldPos.y < minY) minY = worldPos.y;
            if (worldPos.z < minZ) minZ = worldPos.z;

            if (worldPos.x > maxX) maxX = worldPos.x;
            if (worldPos.y > maxY) maxY = worldPos.y;
            if (worldPos.z > maxZ) maxZ = worldPos.z;
        }
        
        for (var i = 0; i < m_meshRenderers.Length; i++)
        {
            var normalizedX = m_meshRenderers[i].transform.position.x / (maxX - minX);
            var normalizedY = m_meshRenderers[i].transform.position.y / (maxY - minY);
            var normalizedZ = m_meshRenderers[i].transform.position.z / (maxZ - minZ);

            var col = Color.Lerp(topColor, bottomColor, normalizedY);

            var fetchFromDictionary = false;
            foreach (var heightMat in m_heightMats)
            {
                var allowed = 1 - quality;
                var difference = Mathf.Abs(heightMat.Key - normalizedY);
                if (difference < allowed)
                {
                    m_meshRenderers[i].sharedMaterial = heightMat.Value;
                    fetchFromDictionary = true;
                }
            }

            if (!fetchFromDictionary)
            {
                var mat = Instantiate(material);
                mat.color = col;
                mat.name = "M_" + meshFilter.gameObject.name + "_" + materialIndex;
                mat.enableInstancing = gpuInstancing;
                m_meshRenderers[i].sharedMaterial = mat;
                m_heightMats.Add(normalizedY, mat);
                //Save material to Assets
                if (createAssetInProject)
                {
                    AssetDatabase.CreateAsset(mat,
                        materialSaveDestination + "/" + meshFilter.gameObject.name + "/" + mat.name + ".mat");
                }

                materialIndex++;
            }
        }

        Debug.Log("Materials Count: " + m_heightMats.Count);
    }
}
