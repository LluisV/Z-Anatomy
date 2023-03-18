using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualBasic.FileIO;

/// <summary>
/// This script creates a .txt for each layer (column) of the imported .csv. 
/// The name of the layer will be the name of the first element of the column.
/// </summary>

namespace Create_Layers
{
    class Program
    {
        const string SEPARATOR = ",";
        const string ORIGIN_PATH = @"C:\Users\Lluis\Desktop\Projectes\Unity\Z-Anatomy PC\Resources\Layers\Collections - ";
        const string DEST_PATH_LAYERS = @"C:\Users\Lluis\Desktop\Projectes\Unity\Z-Anatomy PC\Z-Anatomy PC\Assets\Models\Layers\";
        const string DEST_PATH_COLLECTIONS = @"C:\Users\Lluis\Desktop\Projectes\Unity\Z-Anatomy PC\Z-Anatomy PC\Assets\Models\Collections\";
        static string[] LayersNames = { "Arteries", "Bones", "Fasciae", "Ligaments", "Lymph", "Muscles", "Nerves", "Refs", "Skin", "Veins", "Viscera" };
        static string[] CollectionNames = { "Group-Muscles", "Group-Nerve", "BONUS" };

        static void Main(string[] args)
        {


            foreach (var layer in LayersNames)
            {
                using (TextFieldParser csvParser = new TextFieldParser(ORIGIN_PATH + layer + ".csv"))
                {
                    csvParser.SetDelimiters(new string[] { SEPARATOR });

                    // Skip the row with the column names
                    string[] fields = csvParser.ReadFields();
                    string[] layers = new string[fields.Length];

                    for (int i = 0; i < fields.Length; i++)
                    {
                        layers[i] += fields[i] + "\n";
                    }

                    while (!csvParser.EndOfData)
                    {
                        // Read current line fields, pointer moves to the next line.
                        fields = csvParser.ReadFields();
                        for (int i = 0; i < fields.Length; i++)
                        {
                            layers[i] += fields[i] + "\n";
                        }
                    }

                    if (Directory.Exists(DEST_PATH_LAYERS + layer))
                        Directory.Delete(DEST_PATH_LAYERS + layer, true);

                    Directory.CreateDirectory(DEST_PATH_LAYERS + layer);

                    for (int i = 0; i < layers.Length; i++)
                    {
                        string fileName = layers[i].Split('\n')[0] + ".txt";
                        File.WriteAllText(DEST_PATH_LAYERS + layer + @"\" + fileName, layers[i]);
                    }
                }
            }

            foreach (var collection in CollectionNames)
            {
                using (TextFieldParser csvParser = new TextFieldParser(ORIGIN_PATH + collection + ".csv"))
                {
                    csvParser.SetDelimiters(new string[] { SEPARATOR });

                    // Skip the row with the column names
                    string[] fields = csvParser.ReadFields();
                    string[] layers = new string[fields.Length];

                    for (int i = 0; i < fields.Length; i++)
                    {
                        layers[i] += fields[i] + "\n";
                    }

                    while (!csvParser.EndOfData)
                    {
                        // Read current line fields, pointer moves to the next line.
                        fields = csvParser.ReadFields();
                        for (int i = 0; i < fields.Length; i++)
                        {
                            layers[i] += fields[i] + "\n";
                        }
                    }

                    if (Directory.Exists(DEST_PATH_COLLECTIONS + collection))
                        Directory.Delete(DEST_PATH_COLLECTIONS + collection, true);

                    Directory.CreateDirectory(DEST_PATH_COLLECTIONS + collection);

                    for (int i = 0; i < layers.Length; i++)
                    {
                        string fileName = layers[i].Split('\n')[0] + ".txt";
                        File.WriteAllText(DEST_PATH_COLLECTIONS + collection + @"\" + fileName, layers[i]);
                    }
                }
            }

            
        }
    }
}
