import bpy

for obj in bpy.context.selected_objects:
    # Store delta location of object
    obj_delta = obj.delta_location

    # Assign object location as delta location and Reset delta location to 0
    obj.location = obj_delta
    obj.delta_location = (0,0,0)