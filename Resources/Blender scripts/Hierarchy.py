import bpy

sCollection = bpy.context.collection


def parentCol(_colParent, _objParent):
    for col in _colParent.children:
        newObj = bpy.data.objects.new("empty", None)
        bpy.context.scene.collection.objects.link(newObj)
        newObj.name = col.name
        newObj.parent = _objParent

        if len(col.objects) > 0:
            objs = col.objects
            for obj in objs:
                obj.parent = newObj
        else:
            parentCol(col, newObj)


root = bpy.data.objects.new("empty", None)
bpy.context.scene.collection.objects.link(root)
root.name = sCollection.name

parentCol(sCollection, root)
        
        
        