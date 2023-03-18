import bpy
from pathlib import Path

destination_folder = "E:/AnatomyModels/Descriptions"
df = Path(destination_folder)

#if doesn't exist
if not df.exists():
    df.mkdir(parents=True, exist_ok=True)

original_ext = ''
new_ext = '.txt'

for text in bpy.data.texts:
    p = df / text.name
    if p.suffix == original_ext:
        p = df / (text.name + new_ext)      
        p.write_text(text.as_string())
        
        
        