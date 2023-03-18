= Adjust Pivot =

Online documentation available at: https://github.com/yasirkula/UnityAdjustPivot
E-mail: yasirkula@gmail.com

1. ABOUT
This tool helps you change the pivot point of an object without having to create an empty parent object as the pivot point. There are two types of pivot adjustments:

a. If the object does not have a mesh (MeshFilter, to be precise), then the script simply changes the positions and rotations of child objects accordingly
b. If the object does have a mesh, then the script first creates an instance of the mesh, adjusts the mesh's pivot point by altering its vertices, normals and tangents, and finally changes the positions and rotations of child objects accordingly

2. HOW TO
To change an object's pivot point, create an empty child GameObject and move it to the desired pivot position. Then, open the Adjust Pivot window via the Window-Adjust Pivot menu and press the "Move X's pivot here" button to move the parent object's pivot there. It is safe to delete the empty child object afterwards.

Note that if the object has a mesh (option b), to apply the changes to the prefab, you have to save the instantiated mesh to your project. Otherwise, the asset will be serialized in the scene and won't be available to the prefab. You have two options there:

- save the mesh as asset (.asset)
- save the mesh as OBJ (.obj)

Afterwards, you can safely apply your changes to the prefab.