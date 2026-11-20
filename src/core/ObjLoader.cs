// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
// ObjLoader.cs
using System.Globalization;
using System.Runtime.Versioning;
using OpenTK.Mathematics;

namespace Unminal.Core.ObjLoader;

public class ObjModel {
    public required float[] Vertices { get; set; }
    public required uint[] Indices { get; set; }
}

[SupportedOSPlatform("windows")]
public static class ObjLoader {
    public static ObjModel Load(string path) {
        var rawPositions = new List<Vector3>();
        var rawNormals = new List<Vector3>();

        var finalVertices = new List<float>();
        var indices = new List<uint>();
        var vertexCache = new Dictionary<(int posIdx, int normIdx), uint>();

        string[] lines = File.ReadAllLines(path);

        foreach (string line in lines) {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                continue;

            string[] tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0) continue;

            string type = tokens[0];

            if (type == "v") {
                float x = float.Parse(tokens[1], CultureInfo.InvariantCulture);
                float y = float.Parse(tokens[2], CultureInfo.InvariantCulture);
                float z = float.Parse(tokens[3], CultureInfo.InvariantCulture);
                rawPositions.Add(new Vector3(x, y, z));
            }
            else if (type == "vn") {
                float nx = float.Parse(tokens[1], CultureInfo.InvariantCulture);
                float ny = float.Parse(tokens[2], CultureInfo.InvariantCulture);
                float nz = float.Parse(tokens[3], CultureInfo.InvariantCulture);
                var n = new Vector3(nx, ny, nz);
                rawNormals.Add(n.LengthSquared > 0 ? Vector3.Normalize(n) : Vector3.UnitY);
            }
            else if (type == "f") {
                int count = tokens.Length - 1;
                var polyIndices = new List<uint>(count);
                var polyPositions = new List<Vector3>(count);

                for (int i = 1; i < tokens.Length; i++) {
                    string[] parts = tokens[i].Split('/');

                    int pIdx = int.Parse(parts[0], CultureInfo.InvariantCulture);
                    pIdx = pIdx > 0 ? pIdx - 1 : rawPositions.Count + pIdx;

                    int nIdx = -1;
                    if (parts.Length >= 3 && !string.IsNullOrEmpty(parts[2])) {
                        nIdx = int.Parse(parts[2], CultureInfo.InvariantCulture);
                        nIdx = nIdx > 0 ? nIdx - 1 : rawNormals.Count + nIdx;
                    }

                    var key = (pIdx, nIdx);
                    if (!vertexCache.TryGetValue(key, out uint finalIdx)) {
                        finalIdx = (uint)(finalVertices.Count / 6);
                        Vector3 pos = rawPositions[pIdx];
                        Vector3 norm = (nIdx >= 0 && nIdx < rawNormals.Count) ? rawNormals[nIdx] : Vector3.Zero;

                        finalVertices.Add(pos.X);
                        finalVertices.Add(pos.Y);
                        finalVertices.Add(pos.Z);
                        finalVertices.Add(norm.X);
                        finalVertices.Add(norm.Y);
                        finalVertices.Add(norm.Z);

                        vertexCache[key] = finalIdx;
                    }

                    polyIndices.Add(finalIdx);
                    polyPositions.Add(rawPositions[pIdx]);
                }

                TriangulatePolygon(polyPositions, polyIndices, indices);
            }
        }

        if (rawNormals.Count == 0) {
            RecalculateMissingNormals(finalVertices, indices);
        }

        return new ObjModel {
            Vertices = finalVertices.ToArray(),
            Indices = indices.ToArray()
        };
    }

    private static void TriangulatePolygon(List<Vector3> pts, List<uint> vIndices, List<uint> outIndices) {
        int n = pts.Count;
        if (n < 3) return;
        if (n == 3) {
            outIndices.Add(vIndices[0]);
            outIndices.Add(vIndices[1]);
            outIndices.Add(vIndices[2]);
            return;
        }

        Vector3 normal = Vector3.Zero;
        for (int i = 0; i < n; i++) {
            Vector3 curr = pts[i];
            Vector3 next = pts[(i + 1) % n];
            normal.X += (curr.Y - next.Y) * (curr.Z + next.Z);
            normal.Y += (curr.Z - next.Z) * (curr.X + next.X);
            normal.Z += (curr.X - next.X) * (curr.Y + next.Y);
        }

        float ax = MathF.Abs(normal.X);
        float ay = MathF.Abs(normal.Y);
        float az = MathF.Abs(normal.Z);

        var pts2D = new Vector2[n];
        for (int i = 0; i < n; i++) {
            if (ax >= ay && ax >= az)
                pts2D[i] = new Vector2(pts[i].Y, pts[i].Z);
            else if (ay >= ax && ay >= az)
                pts2D[i] = new Vector2(pts[i].X, pts[i].Z);
            else
                pts2D[i] = new Vector2(pts[i].X, pts[i].Y);
        }

        var rem = new List<int>(n);
        for (int i = 0; i < n; i++) rem.Add(i);

        float area = 0f;
        for (int i = 0; i < n; i++) {
            Vector2 p1 = pts2D[i];
            Vector2 p2 = pts2D[(i + 1) % n];
            area += (p1.X * p2.Y - p2.X * p1.Y);
        }
        bool ccw = area >= 0;

        int guard = 0;
        while (rem.Count > 3 && guard < n * n) {
            guard++;
            bool earFound = false;

            for (int i = 0; i < rem.Count; i++) {
                int prev = rem[(i + rem.Count - 1) % rem.Count];
                int curr = rem[i];
                int next = rem[(i + 1) % rem.Count];

                Vector2 a = pts2D[prev];
                Vector2 b = pts2D[curr];
                Vector2 c = pts2D[next];

                float cross = (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
                if (ccw ? cross <= 1e-6f : cross >= -1e-6f)
                    continue;

                bool containsOther = false;
                for (int j = 0; j < rem.Count; j++) {
                    int testIdx = rem[j];
                    if (testIdx == prev || testIdx == curr || testIdx == next) continue;
                    if (PointInTriangle(pts2D[testIdx], a, b, c)) {
                        containsOther = true;
                        break;
                    }
                }

                if (!containsOther) {
                    outIndices.Add(vIndices[prev]);
                    outIndices.Add(vIndices[curr]);
                    outIndices.Add(vIndices[next]);
                    rem.RemoveAt(i);
                    earFound = true;
                    break;
                }
            }

            if (!earFound) {
                outIndices.Add(vIndices[rem[0]]);
                outIndices.Add(vIndices[rem[1]]);
                outIndices.Add(vIndices[rem[2]]);
                rem.RemoveAt(1);
            }
        }

        if (rem.Count == 3) {
            outIndices.Add(vIndices[rem[0]]);
            outIndices.Add(vIndices[rem[1]]);
            outIndices.Add(vIndices[rem[2]]);
        }
    }

    private static bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c) {
        float d1 = (p.X - b.X) * (a.Y - b.Y) - (a.X - b.X) * (p.Y - b.Y);
        float d2 = (p.X - c.X) * (b.Y - c.Y) - (b.X - c.X) * (p.Y - c.Y);
        float d3 = (p.X - a.X) * (c.Y - a.Y) - (c.X - a.X) * (p.Y - a.Y);
        bool hasNeg = (d1 < 0) || (d2 < 0) || (d3 < 0);
        bool hasPos = (d1 > 0) || (d2 > 0) || (d3 > 0);
        return !(hasNeg && hasPos);
    }

    private static void RecalculateMissingNormals(List<float> verts, List<uint> indices) {
        int vCount = verts.Count / 6;
        var normals = new Vector3[vCount];

        for (int i = 0; i < indices.Count; i += 3) {
            int i0 = (int)indices[i];
            int i1 = (int)indices[i + 1];
            int i2 = (int)indices[i + 2];

            var p0 = new Vector3(verts[i0 * 6], verts[i0 * 6 + 1], verts[i0 * 6 + 2]);
            var p1 = new Vector3(verts[i1 * 6], verts[i1 * 6 + 1], verts[i1 * 6 + 2]);
            var p2 = new Vector3(verts[i2 * 6], verts[i2 * 6 + 1], verts[i2 * 6 + 2]);

            Vector3 n = Vector3.Cross(p1 - p0, p2 - p0);
            normals[i0] += n;
            normals[i1] += n;
            normals[i2] += n;
        }

        for (int i = 0; i < vCount; i++) {
            Vector3 n = normals[i].LengthSquared > 0 ? Vector3.Normalize(normals[i]) : Vector3.UnitY;
            verts[i * 6 + 3] = n.X;
            verts[i * 6 + 4] = n.Y;
            verts[i * 6 + 5] = n.Z;
        }
    }
}
