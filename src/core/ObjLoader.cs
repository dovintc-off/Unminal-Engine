// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
// ObjLoader.cs
namespace Unminal.Core.ObjLoader;

public class ObjModel {
    public required float[] Vertices { get; set; }
    public required uint[] Indices { get; set; }
}

[SupportedOSPlatform("windows")]
public static class ObjLoader {
    public static ObjModel Load(string path) {
        var positions = new List<Vector3>();
        var indices = new List<uint>();

        string[] lines = File.ReadAllLines(path);

        foreach (string line in lines) {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                continue;

            string[] tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            if (tokens.Length == 0) continue;

            string type = tokens[0];

            if (type == "v") {
                float x = float.Parse(tokens[1], System.Globalization.CultureInfo.InvariantCulture);
                float y = float.Parse(tokens[2], System.Globalization.CultureInfo.InvariantCulture);
                float z = float.Parse(tokens[3], System.Globalization.CultureInfo.InvariantCulture);
                
                positions.Add(new Vector3(x, y, z));
            } else if (type == "f") {
                
                int[] faceIndices = new int[tokens.Length - 1];
                
                for (int i = 1; i < tokens.Length; i++)
                {
                    string token = tokens[i];
                    string vertexIndexStr = token.Split('/')[0];
                    
                    int index = int.Parse(vertexIndexStr, System.Globalization.CultureInfo.InvariantCulture);
                    
                    if (index > 0)
                        faceIndices[i - 1] = index - 1;
                    else
                        faceIndices[i - 1] = positions.Count + index;
                }

                for (int i = 1; i < faceIndices.Length - 1; i++)
                {
                    indices.Add((uint)faceIndices[0]);
                    indices.Add((uint)faceIndices[i]);
                    indices.Add((uint)faceIndices[i + 1]);
                }
            }
        }

        Vector3[] calculateNormals = new Vector3[positions.Count];
        for (int i = 0; i < indices.Count; i += 3) {
            int idx0 = (int)indices[i];
            int idx1 = (int)indices[i + 1];
            int idx2 = (int)indices[i + 2];

            Vector3 p0 = positions[idx0];
            Vector3 p1 = positions[idx1];
            Vector3 p2 = positions[idx2];

            Vector3 edge1 = p1 - p0;
            Vector3 edge2 = p2 - p0;

            Vector3 normal = Vector3.Cross(edge1, edge2);

            calculateNormals[idx0] += normal;
            calculateNormals[idx1] += normal;
            calculateNormals[idx2] += normal;
        }

        for (int i = 0; i < calculateNormals.Length; i++) {
            if (calculateNormals[i].Length > 0) calculateNormals[i] = Vector3.Normalize(calculateNormals[i]);
        }

        float[] verticesArray = new float[positions.Count * 6];

        for (int i = 0; i < positions.Count; i++) {
            verticesArray[i * 6]     = positions[i].X;
            verticesArray[i * 6 + 1] = positions[i].Y;
            verticesArray[i * 6 + 2] = positions[i].Z;

            verticesArray[i * 6 + 3] = calculateNormals[i].X;
            verticesArray[i * 6 + 4] = calculateNormals[i].Y;
            verticesArray[i * 6 + 5] = calculateNormals[i].Z;
        }

        return new ObjModel {
            Vertices = verticesArray,
            Indices = indices.ToArray()
        };
    }
}
