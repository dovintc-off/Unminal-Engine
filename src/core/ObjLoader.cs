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

        for (int lineNumber = 0; lineNumber < lines.Length; lineNumber++) {
            string line = lines[lineNumber];
            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                continue;

            string[] tokens = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            if (tokens.Length == 0) continue;

            string type = tokens[0];

            if (type == "v") {
                if (tokens.Length < 4)
                    throw new InvalidDataException($"OBJ line {lineNumber + 1}: vertex requires 3 coordinates.");

                if (!float.TryParse(tokens[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float x) ||
                    !float.TryParse(tokens[2], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float y) ||
                    !float.TryParse(tokens[3], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float z))
                    throw new InvalidDataException($"OBJ line {lineNumber + 1}: invalid vertex coordinates.");
                
                positions.Add(new Vector3(x, y, z));
            } else if (type == "f") {
                if (tokens.Length < 4)
                    throw new InvalidDataException($"OBJ line {lineNumber + 1}: face requires at least 3 vertices.");

                int[] faceIndices = new int[tokens.Length - 1];

                for (int i = 1; i < tokens.Length; i++)
                {
                    string token = tokens[i];
                    string vertexIndexStr = token.Split('/')[0];

                    if (!int.TryParse(vertexIndexStr, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out int index) || index == 0)
                        throw new InvalidDataException($"OBJ line {lineNumber + 1}: invalid vertex index '{vertexIndexStr}'.");

                    int resolvedIndex = index > 0 ? index - 1 : positions.Count + index;
                    if (resolvedIndex < 0 || resolvedIndex >= positions.Count)
                        throw new InvalidDataException($"OBJ line {lineNumber + 1}: vertex index {index} is out of range.");

                    faceIndices[i - 1] = resolvedIndex;
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
