
Make sure that "Fog" in the "Lighting" tab is disabled when updating any tree!
Otherwise textures might get corrupted.


1. Optimized Textures and Materials
--------------------------------------------
Please note that all tree prefabs share the same materials and textures in order to save texture memory and texture load at runtime.

Usually unity automatically creates materials and textures for each tree.
I have just deleted all those texture folders except for the first one, then assigned the materials "Optimized Bark Material" and "Optimized Leaf Material" both located in the Prefab "xyz__donnotdelete" to each tree by dragging them from the project tab to the proper slot of each tree in the inspector tab.

Changing any parameter on any tree will unity force to recalculate the textures so the additional folders will be recreated. In this case just delete the new folders and reassign the original materials like described above.

Those materials use the textures you find in the folder "xyz__donnotdelete_Textures".


2. PineTrees Doublebarkcolor
-------------------------------------------
In order to achieve a realistic looking bark – which is dark grey-brown on the lower trunk, and thin, flaky and orange on the upper trunk – the package ships with a customized bark shader providing a lively diffuse color mapping and highly detailed bump mapping.
All prefabs located in the folder "pineTrees Doublebarkcolor" use the material "pinebarkDoubleColor" and the customized shader.
Of course you can assign this material also to the prefabs located in the folder "pineTrees Singlebarkcolor" [or vise versa] but please make sure that you assign the material to all branches. Otherwise Unity might divide the optimized textures into three chunks what will significantly lower the texture quality.


3. Highres Materials
-------------------------------------------
By default Unity creates combined textures at a resolution of 1024x1024px which is not that much.

In case you want high res textures for e.g. albedo and normal you first will have to create the combined textures manually.
Please have a look at the .psd files "diffuse_highres" and "normal_specular_highres" which contain guide lines to help you place and scale the texture properly. In order to get nice tiling the bark texture needs some padding. Add this padding by simply copying pixels from the left to the right and vise versa like shown in: "Texture_Gide.png" 

Next you have to create new "optimized" materials. Do so by using the wizard script located under: "Assets/Create Highres Optimized Tree Material".
If "Leaf Material" is unchecked the created material will use the "Hidden/Nature/Tree Creator Bark Optimized" shader, if checked it will use the "Hidden/Nature/Tree Creator Leaves Optimized".
Hit "Create" and save your material.

Then assign your high res textures to the new materials.

Finally assign the high res materials by dragging them onto the proper slots of the "Mesh Renderer" component of your tree prefab.

