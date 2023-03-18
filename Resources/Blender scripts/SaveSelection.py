import bpy
from pathlib import Path

selection = bpy.context.selected_objects

#---------DESTINATON FOLDER----------#
destination_folder = r"C:/Z-Anatomy/Selections"
#------------FILE NAME----------------#
file_name = "FILE NAME.txt"

destination_folder = destination_folder.replace("\\","/")

df = Path(destination_folder)

if not df.exists():
    df.mkdir(parents=True, exist_ok=True)

result = ""

for sel in selection:
    result += "%s\n" % sel.name

p = df / file_name
p.write_text(result)