using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OuterWildsShipLogVisualizer;

public static class Extensions
{
    public static string[] GetAllFiles(this DirAccess dir)
    {
        if (dir == null)
        {
            return Array.Empty<string>();
        }

        var files = new List<string>();
        foreach (var subDirName in dir.GetDirectories() ?? Array.Empty<string>())
        {
            var path = System.IO.Path.Combine(dir.GetCurrentDir(), subDirName);
            using var subDir = DirAccess.Open(path);
            files = files.Concat(subDir.GetAllFiles().Select(x => System.IO.Path.Combine(subDirName, x))).ToList();
        }
        files = files.Concat(dir.GetFiles()).ToList();

        return files.ToArray();
    }
}
