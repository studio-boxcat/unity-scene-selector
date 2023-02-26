using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;

namespace Boxcat.Tools.SceneSelector
{
    static class SceneListBuilder
    {
        [NotNull]
        public static List<(SceneGroup, List<SceneEntry>)> Build(bool sort)
        {
            var sceneGroups = SceneGroups.Instance;
            var sceneEntries = SceneEntries.Instance;

            if (sort)
            {
                Undo.IncrementCurrentGroup();
                Undo.RecordObject(sceneGroups, "");
                Undo.RecordObject(sceneEntries, "");

                sceneGroups.Sort();
                sceneEntries.Sort();

                EditorUtility.SetDirty(sceneGroups);
                EditorUtility.SetDirty(sceneEntries);
            }

            var result = new List<(SceneGroup, List<SceneEntry>)>();
            foreach (var sceneGroup in sceneGroups.List)
            {
                if (sceneGroup.Hidden) continue;
                var sceneEntriesForGroup = sceneEntries.List.Where(x => x.Group == sceneGroup.Key).ToList();
                result.Add((sceneGroup, sceneEntriesForGroup));
            }
            return result;
        }
    }
}