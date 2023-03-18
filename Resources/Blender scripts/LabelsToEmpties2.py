import bpy

font_objs = [o for o in bpy.context.scene.objects if o.type == 'FONT']

for fo in font_objs:
    if fo.children:
        line = fo.children[0]
        tmp_line_inv = line.matrix_parent_inverse.copy()
        
    delta_scale = fo.delta_scale
    empty = bpy.data.objects.new("Empty", None)
    empty.location = fo.location
    empty.delta_location = fo.delta_location
    empty.scale = fo.scale
    empty.delta_scale = fo.delta_scale
    empty.rotation_mode = fo.rotation_mode
    empty.rotation_quaternion = fo.rotation_quaternion
    empty.delta_rotation_quaternion = fo.delta_rotation_quaternion
    empty.rotation_euler = fo.rotation_euler
    empty.delta_rotation_euler = fo.delta_rotation_euler
    tmp_mpi = fo.matrix_parent_inverse.copy()
    empty.parent = fo.parent
    empty.matrix_parent_inverse = tmp_mpi

    cols = fo.users_collection
    for c in cols:
        c.objects.link(empty)
        
#    if fo.children:
#        line = fo.children[0]
#        line.parent = empty
#        line.matrix_parent_inverse = tmp_line_inv
    for child in fo.children[:]:
        tmp_mpi = child.matrix_parent_inverse.copy()
        child.parent = empty
        child.matrix_parent_inverse = tmp_mpi
    
    name = fo.name
    bpy.data.objects.remove(fo, do_unlink=True)
    empty.name = name