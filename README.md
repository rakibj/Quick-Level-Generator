# Quick-Level-Generator

Open TextureToLevel scene and select TextureToLevel gameobject or Create an
empty gameobject and add TextureToLevel.cs

TextureToLevel.cs


Mesh Renderer:​ The prefab that will represent a pixel


Texture 2D:​ The sprite from which level will be generated, should be a max size
of 32 on import settings


Target Material:​ The material which will be instantiated


GPU Instancing: ​To turn on GPU Instancing on the generated materials or not?
Generally keeping it on is good when same material is repeated


Create Asset In Project:​ Should the materials be saved in Project files? For the
final version it's recommended to keep it to true


Keep Prefab Connection:​ Should the prefab contain it’s connection or should it
generate an instance of the prefab


Material Save Destination


Alpha Threshold:​ The value above which alpha value pixel will generate a
prefab (out of 255)


Quality​: How many materials should represent the generated level? More quality
-> less performance


Final Parent Scale:​ Scale value of the generated level


Similar for MeshToLevel.cs
