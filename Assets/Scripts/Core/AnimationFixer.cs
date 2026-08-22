using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Test_AnimationMissing
{
    public class AnimationFixer : EditorWindow
    {
        private Animator m_Animator;
        private List<AnimationClip> m_MissingClips = new();
        private List<string> m_MissingPathList = new();
        private List<string> m_ReplaceList = new();
        private List<string> m_ExistsPathList = new();
        private Vector2 m_ScrollPosition;
        private Texture2D m_SuccessIcon;
        private Texture2D m_FailedIcon;

        [MenuItem("EditorWindow/AnimationFixer")]
        private static void Init()
        {
            var window = GetWindow(typeof(AnimationFixer));
            window.titleContent = new GUIContent("AnimationFixer");
            window.Show();
            window.minSize = new Vector2(330, 400);
        }

        private void OnEnable()
        {
            m_SuccessIcon = (Texture2D)EditorGUIUtility.Load("d_greenLight");
            m_FailedIcon = (Texture2D)EditorGUIUtility.Load("d_redLight");
        }

        private void OnGUI()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                m_Animator = EditorGUILayout.ObjectField("animator", m_Animator, typeof(Animator), true) as Animator;

                if (check.changed)
                {
                    Reload();
                }
            }

            if (m_Animator == null)
            {
                EditorGUILayout.HelpBox("animator を設定してください", MessageType.Info);
                return;
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("リロード", GUILayout.Width(80)))
                {
                    Reload();
                }
            }

            using (var scrollview = new EditorGUILayout.ScrollViewScope(m_ScrollPosition))
            {
                m_ScrollPosition = scrollview.scrollPosition;
                if (m_MissingClips.Count == 0)
                {
                    EditorGUILayout.HelpBox("Missing がありません", MessageType.Info);
                    return;
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label("■ Missing パス");
                    GUILayout.Space(10);
                    GUILayout.Label("■ 置換後 パス");
                }
                using (new EditorGUILayout.VerticalScope("box"))
                {
                    for (int i = 0; i < m_MissingPathList.Count; i++)
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.SelectableLabel(m_MissingPathList[i], GUILayout.Height(15));
                            GUILayout.Label("->", GUILayout.Width(20));
                            m_ReplaceList[i] = EditorGUILayout.TextField(m_ReplaceList[i]);
                            var icon = m_ExistsPathList.Contains(m_ReplaceList[i]) ? m_SuccessIcon : m_FailedIcon;
                            if (icon != null)
                            {
                                GUILayout.Label(new GUIContent(icon), GUILayout.Height(20), GUILayout.Width(20));
                            }
                        }
                    }
                }

                GUILayout.Space(15);

                GUILayout.Label("■ 修正対象のAnimationClip");
                foreach (var clip in m_MissingClips)
                {
                    EditorGUILayout.ObjectField(clip, typeof(AnimationClip), false);
                }

                if (GUILayout.Button("Apply"))
                {
                    for (int i = 0; i < m_MissingPathList.Count; i++)
                    {
                        var missingPath = m_MissingPathList[i];
                        var replacePath = m_ReplaceList[i];
                        Undo.RecordObjects(m_MissingClips.ToArray(), "replace animationclip paths");
                        ReplacePath(m_MissingClips.ToArray(), missingPath, replacePath);
                    }

                    Reload();
                }
            }
        }


        private void Reload()
        {
            GUI.FocusControl(null);
            m_ReplaceList.Clear();
            m_MissingClips.Clear();
            m_MissingPathList.Clear();

            var result = GetMissingInfo(m_Animator);
            m_MissingClips.AddRange(result.Item2);
            m_MissingPathList.AddRange(result.Item1);
            m_ReplaceList.AddRange(m_MissingPathList);
            m_ExistsPathList = GetAllPaths(m_Animator.transform);
        }

        // List<missing path>, List<missing clip>
        private static (List<string>, List<AnimationClip>) GetMissingInfo(Animator m_Animator)
        {
            var result = (new List<string>(), new List<AnimationClip>());
            var controller = m_Animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
            var clips = controller.animationClips.Distinct().ToList();

            foreach (var clip in clips)
            {
                var hasMissing = false;
                var bindings = AnimationUtility.GetCurveBindings(clip);
                foreach (var binding in bindings)
                {
                    if (m_Animator.transform.Find(binding.path) == null)
                    {
                        result.Item1.Add(binding.path);
                        hasMissing = true;
                    }
                }

                if (hasMissing)
                {
                    result.Item2.Add(clip);
                }
            }

            result.Item1 = result.Item1.Distinct().ToList();
            result.Item2 = result.Item2.Distinct().ToList();

            return result;
        }

        private static void ReplacePath(AnimationClip[] clips, string oldPath, string newPath)
        {
            foreach (var clip in clips)
            {
                var bindings = AnimationUtility.GetCurveBindings(clip);
                var removeBindings = bindings.Where(c => c.path.Contains(oldPath));

                foreach (var binding in removeBindings)
                {
                    var curve = AnimationUtility.GetEditorCurve(clip, binding);
                    var newBinding = binding;
                    newBinding.path = newBinding.path.Replace(oldPath, newPath);
                    AnimationUtility.SetEditorCurve(clip, binding, null);
                    AnimationUtility.SetEditorCurve(clip, newBinding, curve);
                }
            }
        }

        private static List<string> GetAllPaths(Transform root)
        {
            List<string> paths = new List<string>();
            // Bindings の path に root は含まれない
            foreach (Transform child in root)
            {
                AddPathsRecursive(child, null, paths);
            }
            return paths;
        }

        private static void AddPathsRecursive(Transform current, string path, List<string> paths)
        {
            path = string.IsNullOrEmpty(path) ? current.name : path + "/" + current.name;
            paths.Add(path);
            foreach (Transform child in current)
            {
                AddPathsRecursive(child, path, paths);
            }
        }
    }
}