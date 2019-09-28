using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;

public static class RenameBuild
{
    public const string BuildName = "AugTyr";

    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget _, string path)
    {
        string directory = Path.GetDirectoryName(path);
        string builtName = Path.GetFileNameWithoutExtension(path);

        File.Move(path, Path.Combine(directory, BuildName + ".exe"));

        Directory.Move(
            Path.Combine(directory, builtName + "_Data"),
            Path.Combine(directory, BuildName + "_Data"));
    }
}
