using System.Collections.Generic;
using UnityEngine;
using CryingOnion.OhMy.WeatherSystem.Utility;
using CryingOnion.OhMy.WeatherSystem.Core;
using CryingOnion.OhMy.WeatherSystem.Data;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CryingOnion.OhMy.WeatherSystem.Module
{
    [System.Serializable]
    public class OMWSThunderManager : OMWSFXModule
    {
        public Vector2 thunderTime = new Vector2();
        public GameObject thunderPrefab = null;
        private float thunderTimer = 0;
        public bool active = false;
        public List<OMWSThunderFX> thunderFX = new List<OMWSThunderFX>();
        List<OMWSThunderFX> activeThunderFX = new List<OMWSThunderFX>();
        public OMWSThunderFX defaultFX = null;

        public override void OnFXEnable()
        {
            thunderTimer = Random.Range(thunderTime.x, thunderTime.y);
            defaultFX = (OMWSThunderFX)Resources.Load("Default Thunder");
        }

        public override void SetupFXParent()
        {
            if (vfx.parent == null)
                return;

            parent = new GameObject().transform;
            parent.parent = vfx.parent;
            parent.localPosition = Vector3.zero;
            parent.localScale = Vector3.one;
            parent.name = "Thunder FX";
            parent.gameObject.AddComponent<OMWSFXParent>();
        }

        public override void OnFXUpdate()
        {

            if (!isEnabled)
                return;

            if (vfx == null)
                vfx = OMWSWeather.instance.GetModule<OMWSVFXModule>();

            if (parent == null)
                SetupFXParent();
            else if (parent.parent == null)
                parent.parent = vfx.parent;

            parent.transform.localPosition = Vector3.zero;

            UpdateThunderTimes();

            if (active)
            {
                thunderTimer -= Time.deltaTime;

                if (thunderTimer <= 0)
                    Strike();
                if (thunderTimer > thunderTime.y)
                    thunderTimer = thunderTime.y;
            }
        }

        public void UpdateThunderTimes()
        {
            OMWSThunderFX j = defaultFX;
            activeThunderFX.Clear();
            float total = 0;

            foreach (OMWSThunderFX i in thunderFX)
            {
                if (i.weight > 0)
                {
                    activeThunderFX.Add(i);
                    total += i.weight;
                }
            }

            if (activeThunderFX.Count == 0)
            {
                active = false;
                thunderTime = new Vector2(61, 61);
                return;
            }

            j.weight = Mathf.Clamp01(1 - total);

            if (j.weight > 0)
            {
                activeThunderFX.Add(j);
                total += j.weight;
            }

            thunderTime = j.timeBetweenStrikes;

            foreach (OMWSThunderFX i in activeThunderFX)
                thunderTime = new Vector2(Mathf.Lerp(thunderTime.x, i.timeBetweenStrikes.x, i.weight),
                    Mathf.Lerp(thunderTime.y, i.timeBetweenStrikes.y, i.weight));

            if (thunderTime == new Vector2(61, 61)) { active = false; return; }

            active = true;
        }

        public void Strike()
        {
            Transform i = MonoBehaviour.Instantiate(thunderPrefab, parent).transform;
            i.eulerAngles = Vector3.up * Random.value * 360;

            thunderTimer = Random.Range(thunderTime.x, thunderTime.y);
        }

        public override void OnFXDisable()
        {
            if (parent)
                MonoBehaviour.DestroyImmediate(parent.gameObject);
        }
    }


#if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(OMWSThunderManager))]
    public class OMWSThunderManagerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Rect pos = position;

            Rect tabPos = new Rect(pos.x + 35, pos.y, pos.width - 41, pos.height);
            Rect togglePos = new Rect(5, pos.y, 30, pos.height);

            property.FindPropertyRelative("_OpenTab").boolValue = EditorGUI.BeginFoldoutHeaderGroup(tabPos, property.FindPropertyRelative("_OpenTab").boolValue, new GUIContent("    Thunder FX", "Thunder FX control the rate at which lightning strikes during your weather profiles."), EditorUtilities.FoldoutStyle());

            bool toggle = EditorGUI.Toggle(togglePos, GUIContent.none, property.FindPropertyRelative("_IsEnabled").boolValue);

            if (property.FindPropertyRelative("_IsEnabled").boolValue != toggle)
            {
                property.FindPropertyRelative("_IsEnabled").boolValue = toggle;

                if (toggle == true)
                    (property.serializedObject.targetObject as OMWSVFXModule).thunderManager.OnFXEnable();
                else
                    (property.serializedObject.targetObject as OMWSVFXModule).thunderManager.OnFXDisable();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            if (property.FindPropertyRelative("_OpenTab").boolValue)
            {
                using (new EditorGUI.DisabledScope(!property.FindPropertyRelative("_IsEnabled").boolValue))
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("thunderPrefab"));
                    EditorGUI.indentLevel++;

                    if (property.FindPropertyRelative("thunderPrefab").objectReferenceValue)
                        Editor.CreateEditor((property.FindPropertyRelative("thunderPrefab").objectReferenceValue as GameObject).GetComponent<OMWSThunder>()).OnInspectorGUI();

                    EditorGUI.indentLevel--;
                    EditorGUI.indentLevel--;
                }
            }

            EditorGUI.EndProperty();
        }
    }
#endif
}