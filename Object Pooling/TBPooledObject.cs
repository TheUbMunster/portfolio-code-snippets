using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif
public abstract class TBPooledObject : MonoBehaviour
{
    #region Template Getter
    /// <summary>
    /// Used internally to access data from the main data asset.
    /// </summary>
    private static class Templates
    {
        /// <summary>
        /// Used internally. The main data asset for all the settings.
        /// </summary>
        private static TBPOData DataAsset { get => TBPOData.CurrentDataAsset; }
        /// <summary>
        /// Used internally. Gets a template from the template dictionary using the given key.
        /// </summary>
        /// <param name="key">The key to access the template (TemplateName).</param>
        /// <returns>The associated Template.</returns>
        public static TBPooledObject GetTemplate(string key) => DataAsset.runTimeTemplates[key];
        private static bool ss;
        /// <summary>
        /// Used internally. Adds a template to the template dictionary. 
        /// </summary>
        /// <param name="newT">The new template to add to the dictionary.</param>
        /// <returns>True if the template was added successfully.</returns>
        public static bool AddTemplate(TBPooledObject newT)
        {
            ss = true;
            try
            {
                DataAsset.runTimeTemplates.Add(newT.TemplateName, newT);
            }
            catch
            {
                ss = false;
            }
            return ss;
        }
        ///// <summary>
        ///// Used internally. Adds a template to the template dictionary. 
        ///// </summary>
        ///// <param name="newT">The new template to add to the dictionary.</param>
        ///// <returns>True if the template was added successfully.</returns>
        //public static bool AddTemplate(GameObject newT) => AddTemplate(newT.GetComponent<TBPooledObject>());
        /// <summary>
        /// Used internally. Gets the current template info.
        /// </summary>
        /// <returns>A string containing the template info.</returns>
        public static string GetTemplateInfo()
        {
            System.Text.StringBuilder s = new System.Text.StringBuilder(string.Empty);
            foreach (KeyValuePair<string, TBPooledObject> p in DataAsset.runTimeTemplates)
            {
                s.Append(p.Value.gameObject.name + ", ");
            }
            s = s.Remove(s.Length - 2, 2);
            s.Append(".");
            return s.ToString();
        }
        /// <summary>
        /// Used internally. Gets a pool cap value.
        /// </summary>
        /// <param name="objectName">The key to access the associated templates pool cap value.</param>
        /// <returns>The pool cap.</returns>
        public static int GetPoolCap(string key) => DataAsset.runTimePoolCaps[key];
        /// <summary>
        /// Used internally. Sets a new pool cap value.
        /// </summary>
        /// <param name="key">The key to associate with the new pool cap value.</param>
        /// <param name="value">The pool cap value.</param>
        public static void SetPoolCap(string key, int value) => DataAsset.runTimePoolCaps.Add(key, value);
    }
    #endregion

    #region Editor
#if UNITY_EDITOR
    [CustomEditor(typeof(TBPooledObject), true)]
    [Serializable]
    public class TBPooledObjectEditor : Editor
    {
        private TBPooledObject po;
        private void Awake()
        {
            po = (TBPooledObject)target;
        }
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField(new GUIContent("TBPooledObject Class Settings", "The inspector settings for this TBPooledObject. These settings only matter if you're starting the game with this TBPooledObject in one or more scenes. (If this TBPooledObject was spawned in runtime, these settings won't show up in the inspector)."), EditorStyles.boldLabel);
            if (po.WasCreated)
            {
                EditorGUILayout.LabelField(new GUIContent("No settings needed.", "No settings needed because this TBPooledObject was initially created through script. (Spawn() or Create())."));
            }
            else
            {
                int ind = po.TemplateName != null || po.TemplateName != "" ? 0 : Array.IndexOf<string>(TBPOData.CurrentDataAsset.noneAndCTTNames, po.TemplateName);
                ind = ind != -1 ? ind : 0;
                ind = EditorGUILayout.Popup(new GUIContent("Assigned Pool", "The pool to assign this TBPooledObject upon the start of the game. (The reason this is needed is because the logic for what pool a TBPooledObject is assigned to is done when you call a creation function, but since you're starting the game with this TBPooledObject in the scene, it doesn't know what pool it belongs to). (IF you forget to assign this, it will try to use the name of this object as the key instead, which should work as long as you didn't change the name)."), ind, TBPOData.CurrentDataAsset.noneAndCTTNames);
                if (ind == 0)
                {
                    po.TemplateName = null;
                }
                else
                {
                    po.TemplateName = TBPOData.CurrentDataAsset.noneAndCTTNames[ind];
                }
            }
            EditorGUILayout.LabelField(new GUIContent("Derived Class Settings", "The inspector settings for your derived class."), EditorStyles.boldLabel);
            base.OnInspectorGUI();
        }
    }
#endif
    #endregion

    #region Variables
    /// <summary>
    /// The dictionary that holds all of the TBPooledObject members.
    /// </summary>
    private static Dictionary<string, List<TBPooledObject>> members = new Dictionary<string, List<TBPooledObject>>();
    /// <summary>
    /// A temporary holder variable to use in various functions.
    /// </summary>
    private static TBPooledObject temp = null;
    /// <summary>
    /// Do not edit directly. Use SetDynRen() instead.
    /// </summary>
    private static bool DynamicRenaming { get; set; } = false;
    /// <summary>
    /// DO NOT EDIT MANUALLY!
    /// </summary>    
    public string TemplateName { get; private set; }
    /// <summary>
    /// Do not edit directly. Use KillMe() instead.
    /// </summary>
    private bool alive = true;
    /// <summary>
    /// Do not edit directly. Use KillMe() instead.
    /// </summary>
    private bool Alive
    {
        get => alive;
        set
        {
            if (value == alive)
            {
                return;
            }
            alive = value;
            //OnValueChanged stuff below.
            if (alive)
            {
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
    /// <summary>
    /// Is set to true if this TBPooledObject was created in any sense. (This was not one that pre-existed in the scene).
    /// </summary>
    private bool WasCreated { get; set; } = false;
    /// <summary>
    /// All subscribers are notified when an attempt is made to kill this TBPooledObject.
    /// </summary>
    public event Action onBeforeKillAttempt;
    /// <summary>
    /// All subscribers are notified when this TBPooledObject is killed.
    /// </summary>
    public event Action onAfterKilled;
    /// <summary>
    /// All subscribers are notified when this TBPooledObject is reset.
    /// </summary>
    public event Action onReset;
    #endregion

    #region Initialization
    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }
    #endregion

    #region Spawn
    /// <summary>
    /// Use this function to "Instantiate" a new TBPooledObject of template objectName. This function will find and reset a dead TBPooledObject to reuse, or if it can't find one, it will instantiate a new one.
    /// </summary>
    /// <param name="objectName">The template name of the object you want to spawn.</param>
    /// <returns>Returns the spawned TBPooledObject.</returns>
    public static TBPooledObject Spawn(string objectName)
    {
        temp = null;
        int i = 1;
        members[objectName].ForEach((x) =>
        {
            if (temp == null && !x.Alive)
            {
                temp = x;
                if (DynamicRenaming)
                {
                    x.gameObject.name = "Pooled - " + objectName + " No. " + i;
                }
            }
            i++;
        });
        if (temp != null)
        {
            temp.Alive = true;
            temp.SelfReset();
            return temp;
        }
        else
        {
            if (Templates.GetPoolCap(objectName) != -1 && members[objectName].Count > Templates.GetPoolCap(objectName) + 1)
            {
                Debug.Log("An attempt was made to increase the \"" + objectName + "\" pool beyond it's maximum capacity. Nothing happened.");
                return null;
            }
            try
            {
                temp = Instantiate(Templates.GetTemplate(objectName).gameObject).GetComponent<TBPooledObject>();
                if (!temp.gameObject.activeSelf)
                {
                    temp.gameObject.SetActive(true);
                }
            }
            catch
            {
                throw new System.Exception("Error: Unable to Instantiate a new TBPooledObject. Did you spell the objectName paramiter right? Are you sure the template prefab has a TBPooledObject derived script on it? Here's the string I was given: \"" + objectName + "\".");
            }
            temp.TemplateName = objectName;
            members[objectName].Add(temp);
            temp.gameObject.name = "Pooled - " + objectName + " No. " + members[objectName].Count;
            temp.WasCreated = true;
            return temp;
        }
    }
    /// <summary>
    /// Use this function to "Instantiate" a new TBPooledObject of template objectName. This function will find and reset a dead TBPooledObject to reuse, or if it can't find one, it will instantiate a new one.
    /// </summary>
    /// <param name="objectName">The template name of the object you want to spawn.</param>
    /// <param name="position">The position to assign to the spawned TBPooledObject.</param>
    /// <returns>Returns the spawned TBPooledObject.</returns>
    public static TBPooledObject Spawn(string objectName, Vector3 position)
    {
        temp = Spawn(objectName);
        temp.gameObject.transform.position = position;
        return temp;
    }
    /// <summary>
    /// Use this function to "Instantiate" a new TBPooledObject of template objectName. This function will find and reset a dead TBPooledObject to reuse, or if it can't find one, it will instantiate a new one.
    /// </summary>
    /// <param name="objectName">The template name of the object you want to spawn.</param>
    /// <param name="position">The position to assign to the spawned TBPooledObject.</param>
    /// <param name="rotation">The rotation to assign to the spawned TBPooledObject.</param>
    /// <returns>Returns the spawned TBPooledObject.</returns>
    public static TBPooledObject Spawn(string objectName, Vector3 position, Vector3 rotation)
    {
        temp = Spawn(objectName);
        temp.gameObject.transform.position = position;
        temp.gameObject.transform.rotation = Quaternion.Euler(rotation);
        return temp;
    }
    /// <summary>
    /// Use this function to "Instantiate" a new TBPooledObject of template objectName. This function will find and reset a dead TBPooledObject to reuse, or if it can't find one, it will instantiate a new one.
    /// </summary>
    /// <param name="objectName">The template name of the object you want to spawn.</param>
    /// <param name="position">The position to assign to the spawned TBPooledObject.</param>
    /// <param name="rotation">The rotation to assign to the spawned TBPooledObject.</param>
    /// <returns>Returns the spawned TBPooledObject.</returns>
    public static TBPooledObject Spawn(string objectName, Vector3 position, Quaternion rotation)
    {
        temp = Spawn(objectName);
        temp.gameObject.transform.position = position;
        temp.gameObject.transform.rotation = rotation;
        return temp;
    }
    /// <summary>
    /// Use this function to "Instantiate" a new TBPooledObject of template objectName. This function will find and reset a dead TBPooledObject to reuse, or if it can't find one, it will instantiate a new one.
    /// </summary>
    /// <param name="objectName">The template name of the object you want to spawn.</param>
    /// <param name="position">The position to assign to the spawned TBPooledObject.</param>
    /// <param name="rotation">The rotation to assign to the spawned TBPooledObject.</param>
    /// <param name="scale">The scale to assign to the spawned TBPooledObject. (Assigned to localScale).</param>
    /// <returns>Returns the spawned TBPooledObject.</returns>
    public static TBPooledObject Spawn(string objectName, Vector3 position, Vector3 rotation, Vector3 scale)
    {
        temp = Spawn(objectName);
        temp.gameObject.transform.position = position;
        temp.gameObject.transform.rotation = Quaternion.Euler(rotation);
        temp.gameObject.transform.localScale = scale;
        return temp;
    }
    /// <summary>
    /// Use this function to "Instantiate" a new TBPooledObject of template objectName. This function will find and reset a dead TBPooledObject to reuse, or if it can't find one, it will instantiate a new one.
    /// </summary>
    /// <param name="objectName">The template name of the object you want to spawn.</param>
    /// <param name="position">The position to assign to the spawned TBPooledObject.</param>
    /// <param name="rotation">The rotation to assign to the spawned TBPooledObject.</param>
    /// <param name="scale">The scale to assign to the spawned TBPooledObject. (Assigned to localScale).</param>
    /// <returns>Returns the spawned TBPooledObject.</returns>
    public static TBPooledObject Spawn(string objectName, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        temp = Spawn(objectName);
        temp.gameObject.transform.position = position;
        temp.gameObject.transform.rotation = rotation;
        temp.gameObject.transform.localScale = scale;
        return temp;
    }
    #endregion

    #region Create
    /// <summary>
    /// This function works similarly to Spawn(), except that Create() will forcefully instantiate a new TBPooledObject even if there are some available in the pool.
    /// </summary>
    /// <param name="objectName">The template name of the object you want to spawn.</param>
    /// <returns>Returns the spawned TBPooledObject.</returns>
    public static TBPooledObject Create(string objectName)
    {
        temp = null;
        if (Templates.GetPoolCap(objectName) != -1 && members[objectName].Count > Templates.GetPoolCap(objectName) + 1)
        {
            Debug.Log("An attempt was made to increase the \"" + objectName + "\" pool beyond it's maximum capacity. Nothing happened.");
            return null;
        }
        try
        {
            temp = Instantiate(Templates.GetTemplate(objectName).gameObject).GetComponent<TBPooledObject>();
        }
        catch
        {
            throw new System.Exception("Error: Unable to Instantiate a new PooledObject. Did you spell the objectName paramiter right? Are you sure the template prefab has a PooledObject derived script on it? Here's the string Create() was given: \"" + objectName + "\".");
        }
        temp.TemplateName = objectName;
        members[objectName].Add(temp);
        temp.gameObject.name = "Pooled - " + objectName + " No. " + members[objectName].Count;
        temp.WasCreated = true;
        return temp;
    }
    /// <summary>
    /// This function works similarly to Spawn(), except that Create() will forcefully instantiate a new TBPooledObject even if there are some available in the pool.
    /// </summary>
    /// <param name="objectName">The template name of the object you want to spawn.</param>
    /// <param name="position">The position to assign to the created TBPooledObject.</param>
    /// <returns>Returns the spawned TBPooledObject.</returns>
    public static TBPooledObject Create(string objectName, Vector3 position)
    {
        if (temp == null)
        {
            return null;
        }
        temp = Create(objectName);
        temp.gameObject.transform.position = position;
        return temp;
    }
    /// <summary>
    /// This function works similarly to Spawn(), except that Create() will forcefully instantiate a new TBPooledObject even if there are some available in the pool.
    /// </summary>
    /// <param name="objectName">The template name of the object you want to spawn.</param>
    /// <param name="position">The position to assign to the created TBPooledObject.</param>
    /// <param name="rotation">The rotation to assign to the created TBPooledObject.</param>    
    /// <returns>Returns the spawned TBPooledObject.</returns>
    public static TBPooledObject Create(string objectName, Vector3 position, Vector3 rotation)
    {
        if (temp == null)
        {
            return null;
        }
        temp = Create(objectName);
        temp.gameObject.transform.position = position;
        temp.gameObject.transform.rotation = Quaternion.Euler(rotation);
        return temp;
    }
    /// <summary>
    /// This function works similarly to Spawn(), except that Create() will forcefully instantiate a new TBPooledObject even if there are some available in the pool.
    /// </summary>
    /// <param name="objectName">The template name of the object you want to spawn.</param>
    /// <param name="position">The position to assign to the created TBPooledObject.</param>
    /// <param name="rotation">The rotation to assign to the created TBPooledObject.</param>    
    /// <returns>Returns the spawned TBPooledObject.</returns>
    public static TBPooledObject Create(string objectName, Vector3 position, Quaternion rotation)
    {
        if (temp == null)
        {
            return null;
        }
        temp = Create(objectName);
        temp.gameObject.transform.position = position;
        temp.gameObject.transform.rotation = rotation;
        return temp;
    }
    /// <summary>
    /// This function works similarly to Spawn(), except that Create() will forcefully instantiate a new TBPooledObject even if there are some available in the pool.
    /// </summary>
    /// <param name="objectName">The template name of the object you want to spawn.</param>
    /// <param name="position">The position to assign to the created TBPooledObject.</param>
    /// <param name="rotation">The rotation to assign to the created TBPooledObject.</param>
    /// <param name="scale">The scale to assign to the created TBPooledObject. (Assigned to localScale).</param>
    /// <returns>Returns the spawned TBPooledObject.</returns>
    public static TBPooledObject Create(string objectName, Vector3 position, Vector3 rotation, Vector3 scale)
    {
        if (temp == null)
        {
            return null;
        }
        temp = Create(objectName);
        temp.gameObject.transform.position = position;
        temp.gameObject.transform.rotation = Quaternion.Euler(rotation);
        temp.gameObject.transform.localScale = scale;
        return temp;
    }
    /// <summary>
    /// This function works similarly to Spawn(), except that Create() will forcefully instantiate a new TBPooledObject even if there are some available in the pool.
    /// </summary>
    /// <param name="objectName">The template name of the object you want to spawn.</param>
    /// <param name="position">The position to assign to the created TBPooledObject.</param>
    /// <param name="rotation">The rotation to assign to the created TBPooledObject.</param>
    /// <param name="scale">The scale to assign to the created TBPooledObject. (Assigned to localScale).</param>
    /// <returns>Returns the spawned TBPooledObject.</returns>
    public static TBPooledObject Create(string objectName, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        if (temp == null)
        {
            return null;
        }
        temp = Create(objectName);
        temp.gameObject.transform.position = position;
        temp.gameObject.transform.rotation = rotation;
        temp.gameObject.transform.localScale = scale;
        return temp;
    }
    #endregion

    #region Launch
    /// <summary>
    /// This function works similarly to Spawn(), except that if Launch() doesn't find a member to reset and reuse, it returns null. 
    /// </summary>
    /// <param name="objectName">The template name of the object you want to spawn.</param>
    /// <returns>Returns the launched TBPooledObject or null.</returns>
    public static TBPooledObject Launch(string objectName)
    {
        temp = null;
        int i = 1;
        members[objectName].ForEach((x) =>
        {
            if (DynamicRenaming)
            {
                x.gameObject.name = "Pooled - " + objectName + " No. " + i;
            }
            if (temp == null && !x.Alive)
            {
                temp = x;
            }
            i++;
        });
        if (temp != null)
        {
            temp.SelfReset();
            temp.Alive = true;
            return temp;
        }
        else
        {
            return null;
        }
    }
    /// <summary>
    /// This function works similarly to Spawn(), except that if Launch() doesn't find a member to reset and reuse, it returns null. 
    /// </summary>
    /// <param name="objectName">The template name of the object you want to spawn.</param>
    /// <param name="position">The position to assign to the launched TBPooledObject.</param>
    /// <returns>Returns the launched TBPooledObject or null.</returns>   
    public static TBPooledObject Launch(string objectName, Vector3 position)
    {
        temp = Launch(objectName);
        if (temp == null)
        {
            return null;
        }
        temp.gameObject.transform.position = position;
        return temp;
    }
    /// <summary>
    /// This function works similarly to Spawn(), except that if Launch() doesn't find a member to reset and reuse, it returns null. 
    /// </summary>
    /// <param name="objectName">The template name of the object you want to spawn.</param>
    /// <param name="position">The position to assign to the launched TBPooledObject.</param>
    /// <param name="rotation">The rotation to assign to the launched TBPooledObject.</param>    
    /// <returns>Returns the launched TBPooledObject or null.</returns>
    public static TBPooledObject Launch(string objectName, Vector3 position, Vector3 rotation)
    {
        temp = Launch(objectName);
        if (temp == null)
        {
            return null;
        }
        temp.gameObject.transform.position = position;
        temp.gameObject.transform.rotation = Quaternion.Euler(rotation);
        return temp;
    }
    /// <summary>
    /// This function works similarly to Spawn(), except that if Launch() doesn't find a member to reset and reuse, it returns null. 
    /// </summary>
    /// <param name="objectName">The template name of the object you want to spawn.</param>
    /// <param name="position">The position to assign to the launched TBPooledObject.</param>
    /// <param name="rotation">The rotation to assign to the launched TBPooledObject.</param>  
    /// <returns>Returns the launched TBPooledObject or null.</returns>
    public static TBPooledObject Launch(string objectName, Vector3 position, Quaternion rotation)
    {
        temp = Launch(objectName);
        if (temp == null)
        {
            return null;
        }
        temp.gameObject.transform.position = position;
        temp.gameObject.transform.rotation = rotation;
        return temp;
    }
    /// <summary>
    /// This function works similarly to Spawn(), except that if Launch() doesn't find a member to reset and reuse, it returns null. 
    /// </summary>
    /// <param name="objectName">The template name of the object you want to spawn.</param>
    /// <param name="position">The position to assign to the launched TBPooledObject.</param>
    /// <param name="rotation">The rotation to assign to the launched TBPooledObject.</param>  
    /// <param name="scale">The scale to assign to the launched TBPooledObject. (Assigned to localScale).</param>
    /// <returns>Returns the launched TBPooledObject or null.</returns>
    public static TBPooledObject Launch(string objectName, Vector3 position, Vector3 rotation, Vector3 scale)
    {
        temp = Launch(objectName);
        if (temp == null)
        {
            return null;
        }
        temp.gameObject.transform.position = position;
        temp.gameObject.transform.rotation = Quaternion.Euler(rotation);
        temp.gameObject.transform.localScale = scale;
        return temp;
    }
    /// <summary>
    /// This function works similarly to Spawn(), except that if Launch() doesn't find a member to reset and reuse, it returns null. 
    /// </summary>
    /// <param name="objectName">The template name of the object you want to spawn.</param>
    /// <param name="position">The position to assign to the launched TBPooledObject.</param>
    /// <param name="rotation">The rotation to assign to the launched TBPooledObject.</param>  
    /// <param name="scale">The scale to assign to the launched TBPooledObject. (Assigned to localScale).</param>
    /// <returns>Returns the launched TBPooledObject or null.</returns>
    public static TBPooledObject Launch(string objectName, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        temp = Launch(objectName);
        if (temp == null)
        {
            return null;
        }
        temp.gameObject.transform.position = position;
        temp.gameObject.transform.rotation = rotation;
        temp.gameObject.transform.localScale = scale;
        return temp;
    }
    #endregion

    #region Kill
    /// <summary>
    /// Use this function to "Destroy" a TBPooledObject.
    /// </summary>
    /// <returns>Returns true if the TBPooledObject was successfully killed. (Returns false if it was already dead).</returns>
    public bool KillMe()
    {
        new WaitForEndOfFrame();
        onBeforeKillAttempt?.Invoke();
        if (Alive)
        {
            try
            {
                Alive = false;
            }
            catch
            {
                throw new System.Exception("You should never see this message unless something went really wrong.");
            }
            onAfterKilled?.Invoke();
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// Use this function to "Destroy" a TBPooledObject.
    /// </summary>
    /// <param name="time">Time (in seconds) of a delay before the TBPooledObject is killed.</param>
    public void KillMe(float time)
    {
        Invoke("KillMe", time);
    }
    /// <summary>
    /// Use this function to "Destroy" a TBPooledObject.
    /// </summary>
    /// <param name="target">The target TBPooledObject to kill.</param>
    public static bool Kill(TBPooledObject target)
    {
        return target.KillMe();
    }
    /// <summary>
    /// Use this function to "Destroy" a TBPooledObject.
    /// </summary>
    /// <param name="target">The target TBPooledObject to kill.</param>
    /// <param name="time">Time (in seconds) of a delay before the TBPooledObject is killed.</param>
    public static void Kill(TBPooledObject target, float time)
    {
        target.KillMe(time);
    }
    #endregion

    #region Discard
    /// <summary>
    /// Use this function to remove a TBPooledObject's TBPooledObject derived script safely. (This object will no longer behave like a TBPooledObject).
    /// </summary>
    /// <returns>Returns true if DiscardSelf() found at least one TBPooledObject script on this object and destroyed it.</returns>
    public bool DiscardSelf(bool makeRedeemable = false)
    {
        if (makeRedeemable)
        {
            new WaitForEndOfFrame();
            this.enabled = false;
            return true;
        }
        else
        {
            new WaitForEndOfFrame();
            Destroy(this);
            return true;
        }
    }
    /// <summary>
    /// Use this function to remove a TBPooledObject's TBPooledObject derived script safely. (This object will no longer behave like a TBPooledObject).
    /// </summary>
    /// <param name="time">Time (in seconds) of a delay before the TBPooledObject is discarded.</param>
    public void DiscardSelf(float time)
    {
        Invoke("DiscardSelf", time);
    }
    /// <summary>
    /// Use this function to remove a TBPooledObject's TBPooledObject derived script safely. (This object will no longer behave like a TBPooledObject).
    /// </summary>
    /// <param name="target">The target TBPooledObject to discard.</param>
    /// <returns>Returns true if DiscardSelf() found at least one TBPooledObject script on this object and destroyed it.</returns>
    public static bool Discard(TBPooledObject target, bool makeRedeemable = false)
    {
        return target.DiscardSelf(makeRedeemable);
    }
    /// <summary>
    /// Use this function to remove a TBPooledObject's TBPooledObject derived script safely. (This object will no longer behave like a TBPooledObject).
    /// </summary>
    /// <param name="target">The target TBPooledObject to discard.</param>
    /// <param name="time">Time (in seconds) of a delay before the TBPooledObject is discarded.</param>
    public static void Discard(TBPooledObject target, float time)
    {
        target.DiscardSelf(time);
    }
    #endregion

    #region Redeem
    public TBPooledObject Reedem(GameObject target)
    {
        temp = target.GetComponent<TBPooledObject>();
        temp.enabled = true;
        return temp;
    }
    #endregion

    #region Annihilate
    /// <summary>
    /// Use this function to literally DESTROY a TBPooledObject.
    /// </summary>
    /// <returns>Returns true if object was successfully annihilated.</returns>
    public bool AnnihilateSelf()
    {
        new WaitForEndOfFrame();
        bool b = DiscardSelf();
        if (!b)
        {
            return false;
        }
        Destroy(gameObject);
        return true;
    }
    /// <summary>
    /// Use this funtion to literally DESTROY a TBPooledObject.
    /// </summary>
    /// <param name="time">Time (in seconds) of a delay before the TBPooledObject is annhiliated.</param>
    public void AnnihilateSelf(float time)
    {
        Invoke("AnnihilateSelf", time);
    }
    /// <summary>
    /// Use this function to literally DESTROY a TBPooledObject.
    /// </summary>
    /// <param name="target">The target TBPooledObject to annhiliate.</param>
    /// <returns>Returns true if object was successfully annhiliated.</returns>
    public static bool Annihilate(TBPooledObject target)
    {
        return target.AnnihilateSelf();
    }
    /// <summary>
    /// Use this function to literally DESTROY a TBPooledObject.
    /// </summary>
    /// <param name="target">The target TBPooledObject to annhiliate.</param>
    /// <param name="time">Time (in seconds) of a delay before the TBPooledObject is annhiliated.</param>    
    public static void Annihilate(TBPooledObject target, float time)
    {
        target.Invoke("AnnihilateSelf", time);
    }
    #endregion

    #region SelfReset
    private void SelfReset()
    {
        transform.position = Templates.GetTemplate(TemplateName).transform.position;
        transform.rotation = Templates.GetTemplate(TemplateName).transform.rotation;
        transform.localScale = Templates.GetTemplate(TemplateName).transform.localScale;
        onReset?.Invoke();
        OnReset();
    }
    protected virtual void OnReset()
    {

    }
    #endregion

    #region Init
    /// <summary>
    /// Use this function to make a new template in runtime.
    /// </summary>
    /// <typeparam name="T">The TBPooledObject derived script to attach to your newly Init'd TBPooledObject.</typeparam>
    /// <param name="target">The target TBPooledObject to init.</param>
    /// <param name="preserveOriginal">Set this to true if you'd like to preserve the original object. If this is true, it will also transform it into the first pooled member.</param>
    /// <param name="poolCap">The pool cap to associate with the new template.</param>
    /// <returns>Returns the Init'd TBPooledObject.</returns>
    public static TBPooledObject Init<T>(GameObject target, bool preserveOriginal, int poolCap = -1) where T : TBPooledObject
    {
        TBPooledObject[] ps = target.GetComponents<TBPooledObject>();
        if (ps.Length != 0)
        {
            return null;
        }
        else
        {
            string key = target.name;
            members.Add(key, new List<TBPooledObject>());
            Templates.SetPoolCap(key, poolCap);
            temp = target.AddComponent<T>();
            temp.TemplateName = key;
            if (preserveOriginal)
            {
                temp.gameObject.name = "Pooled - " + key + " No. 1";
                members[key].Add(temp);
                temp = Instantiate(temp.gameObject).GetComponent<T>();
                temp.gameObject.name = key;
                temp.TemplateName = key;
                temp.gameObject.SetActive(false);
            }
            else
            {
                temp.gameObject.SetActive(false);
            }
            Templates.AddTemplate(temp);
            return temp;
        }
    }
    /// <summary>
    /// Use this function to make a new template in runtime. The default attached script is TBPOTemplate.
    /// </summary>
    /// <param name="target">The target TBPooledObject to init.</param>
    /// <param name="preserveOriginal">Set this to true if you'd like to preserve the original object. If this is true, it will also transform it into the first pooled member.</param>
    /// <param name="poolCap">The pool cap to associate with the new template.</param>
    /// <returns>Returns the Init'd TBPooledObject.</returns>
    public static TBPooledObject Init(GameObject target, bool preserveOriginal, int poolCap = -1)
    {
        return Init<TBPOTemplate>(target, preserveOriginal, poolCap);
    }
    #endregion

    #region Closest/Farthest
    /// <summary>
    /// Use this function to check for the closest TBPooledObject to checkpoint.
    /// </summary>
    /// <param name="checkPoint">The point to check from.</param>
    /// <returns>The closest TBPooledObject to checkpoint. (Checks among ALL TBPooledObjects). (Will return null if the ONLY TBPooledObject(s) found were in the exact same spot as checkPoint).</returns>
    public static TBPooledObject Closest(Vector3 checkPoint)
    {
        return Closest(checkPoint, members.Keys.ToArray(), out _);
    }
    /// <summary>
    /// Use this function to check for the closest TBPooledObject to checkpoint.
    /// </summary>
    /// <param name="checkPoint">The point to check from.</param>
    /// <param name="checkTemplates">The templates to check.</param>
    /// <returns>The closest TBPooledObject to checkpoint. (Checks among only the TBPooledObjects of templates checkTemplates). (Will return null if the ONLY TBPooledObject(s) found were in the exact same spot as checkPoint).</returns>
    public static TBPooledObject Closest(Vector3 checkPoint, string[] checkTemplates)
    {
        return Closest(checkPoint, checkTemplates, out _);
    }
    /// <summary>
    /// Use this function to check for the closest TBPooledObject to checkpoint.
    /// </summary>
    /// <param name="checkPoint">The point to check from.</param>
    /// <param name="checkTemplates">The templates to check.</param>
    /// <param name="distance">The distance between checkPoint and the closest TBPooledObject.</param>
    /// <returns>The closest TBPooledObject to checkpoint. (Checks among only the TBPooledObjects of templates checkTemplates). (Will return null if the ONLY TBPooledObject(s) found were in the exact same spot as checkPoint).</returns>
    public static TBPooledObject Closest(Vector3 checkPoint, string[] checkTemplates, out float distance)
    {
        temp = null;
        float tempTwo = Mathf.Infinity;
        float tempThree = Mathf.Infinity;
        members.ForEachPooledObject(checkTemplates, x =>
        {
            tempThree = Vector3.Distance(checkPoint, x.transform.position);
            if (x.Alive && tempThree < tempTwo && tempThree != 0f)
            {
                tempTwo = tempThree;
                temp = x;
            }
        });
        distance = tempTwo;
        return temp;
    }
    /// <summary>
    /// Use this function to check for the furthest TBPooledObject to checkpoint.
    /// </summary>
    /// <param name="checkPoint">The point to check from.</param>
    /// <returns>The furthest TBPooledObject to checkpoint. (Checks among ALL TBPooledObjects). (Will return null if the ONLY TBPooledObject(s) found were in the exact same spot as checkPoint).</returns>
    public static TBPooledObject Farthest(Vector3 checkPoint)
    {
        return Farthest(checkPoint, members.Keys.ToArray(), out _);
    }
    /// <summary>
    /// Use this function to check for the furthest TBPooledObject to checkpoint.
    /// </summary>
    /// <param name="checkPoint">The point to check from.</param>
    /// <param name="checkTemplates">The templates to check.</param>
    /// <returns>The furthest TBPooledObject to checkpoint. (Checks among only the TBPooledObjects of templates checkTemplates). (Will return null if the ONLY TBPooledObject(s) found were in the exact same spot as checkPoint).</returns>
    public static TBPooledObject Farthest(Vector3 checkPoint, string[] checkTemplates)
    {
        return Farthest(checkPoint, checkTemplates, out _);
    }
    /// <summary>
    /// Use this function to check for the furthest TBPooledObject to checkpoint.
    /// </summary>
    /// <param name="checkPoint">The point to check from.</param>
    /// <param name="checkTemplates">The templates to check.</param>
    /// <param name="distance">The distance between checkPoint and the furthest TBPooledObject.</param>
    /// <returns>The furthest TBPooledObject to checkpoint. (Checks among only the TBPooledObjects of templates checkTemplates). (Will return null if the ONLY TBPooledObject(s) found were in the exact same spot as checkPoint).</returns>
    public static TBPooledObject Farthest(Vector3 checkPoint, string[] checkTemplates, out float distance)
    {
        temp = null;
        float tempTwo = Mathf.NegativeInfinity;
        float tempThree = Mathf.NegativeInfinity;
        members.ForEachPooledObject(checkTemplates, x =>
        {
            tempThree = Vector3.Distance(checkPoint, x.transform.position);
            if (x.Alive && tempThree > tempTwo && tempThree != 0f)
            {
                tempTwo = tempThree;
                temp = x;
            }
        });
        distance = tempTwo;
        return temp;
    }
    #endregion

    #region Wrangle
    /// <summary>
    /// Use this function to gather references to groups of TBPooledObjects.
    /// </summary>
    /// <param name="searchOfTemplates">The object lists to search through.</param>
    /// <returns>An array of the TBPooledObjects found.</returns>
    public static TBPooledObject[] Wrangle(string[] searchOfTemplates)
    {
        List<TBPooledObject> list = new List<TBPooledObject>();
        members.ForEachPooledObject(searchOfTemplates, x => { list.Add(x); });
        return list.ToArray();
    }
    #endregion

    #region OnSceneChange Destruction Exemption
    private void OnSceneLoaded(Scene scene, LoadSceneMode lsm)
    {
        foreach (KeyValuePair<string, List<TBPooledObject>> ps in members)
        {
            ps.Value.ForEach((x) =>
            {
                SceneManager.MoveGameObjectToScene(x.gameObject, scene);
            });
        }
    }
    private void OnSceneUnloaded(Scene scene)
    {
        if (Templates.GetTemplate(TemplateName) == this)
        {
            new WaitForEndOfFrame();
            Destroy(Templates.GetTemplate(TemplateName));
            members.Remove(TemplateName);
        }
    }
    /// <summary>
    /// Safely changes the scene while preserving all TBPooledObjects of saveTemplates.
    /// </summary>
    /// <param name="saveTemplates">An array of the TBPooledObject of templates to save.</param>
    /// <param name="sceneBuildIndex">The scene build index to change to.</param>
    /// <param name="saveLiving">Do we preserve all currently used TBPooledObjects as well?</param>
    public static void SafeSceneChange(string[] saveTemplates, int sceneBuildIndex, bool saveLiving = false)
    {
        for (int i = 0; i < saveTemplates.Length; i++)
        {
            members[saveTemplates[i]].ForEach((x) =>
            {
                if (saveLiving)
                {
                    x.Alive = false;
                }
                DontDestroyOnLoad(x.gameObject);
            });
        }
        SceneManager.LoadScene(sceneBuildIndex, LoadSceneMode.Single);
    }
    /// <summary>
    /// Safely changes the scene while preserving all TBPooledObjects of saveTemplates.
    /// </summary>
    /// <param name="saveTemplates">An array of the TBPooledObject of templates to save.</param>
    /// <param name="sceneName">The scene name to change to.</param>
    /// <param name="saveLiving">Do we preserve all currently used TBPooledObjects as well?</param>
    public static void SafeSceneChange(string[] saveTemplates, string sceneName, bool saveLiving = false)
    {
        SafeSceneChange(saveTemplates, SceneManager.GetSceneByName(sceneName).buildIndex, saveLiving);
    }
    #endregion

    #region Editor Settings
    /// <summary>
    /// Use this function to turn on or off Dynamic Renaming.
    /// </summary>
    /// <param name="value">The value to assign to Dynamic Renaming.</param>
    public static void SetDynRen(bool value)
    {
        DynamicRenaming = value;
    }
    #endregion

    #region OnSignificantEvents
    protected virtual void OnDestroy()
    {
        if (members.ContainsKey(TemplateName))
        {
            members[TemplateName].Remove(this);
        }
    }
    private void OnApplicationQuit()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }
    #endregion

    #region QOL
    /// <summary>
    /// Use this function to get a list of all of the template objects names.
    /// </summary>
    /// <returns>Returns a list of the template objects, seperated by ", " and capped off by ".".</returns>
    public static string GetTemplateInfo()
    {
        return Templates.GetTemplateInfo();
    }
    /// <summary>
    /// Use this function to get a list of all of the member objects names.
    /// </summary>
    /// <returns>Returns a list of the member objects, seperated by ", " and capped off by ".".</returns>
    public static string GetMembersInfo()
    {
        System.Text.StringBuilder s = new System.Text.StringBuilder(string.Empty);
        foreach (KeyValuePair<string, List<TBPooledObject>> ps in members)
        {
            ps.Value.ForEach((x) =>
            {
                s.Append(x.gameObject.name + ", ");
            });
        }
        s = s.Remove(s.Length - 2, 2);
        s.Append(".");
        return s.ToString();
    }
    #endregion

    #region Extension Methods
    /// <summary>
    /// Extension method used to operate on the list of TBPooledObjects.
    /// </summary>
    /// <param name="ofTemplates">What template lists to iterate through.</param>
    /// <param name="action">Delegate of a TBPooledObject function (or lambda expression).</param>
    public static void ForEachPooledObject(string[] ofTemplates, Action<TBPooledObject> action)
    {
        members.ForEachPooledObject(ofTemplates, action);
    }
    #endregion
}
#region Extension Methods
public static class TBPOExtensionMethods
{
    /// <summary>
    /// Extension method exclusivly used internally for the members variable.
    /// </summary>
    /// <param name="collection">The collection to iterate through.</param>
    /// <param name="ofTemplates">What template lists to iterate through.</param>
    /// <param name="action">Delegate of a TBPooledObject function (or lambda expression).</param>
    public static void ForEachPooledObject(this Dictionary<string, List<TBPooledObject>> collection, string[] ofTemplates, Action<TBPooledObject> action)
    {
        foreach (string s in ofTemplates)
        {
            collection[s].ForEach(action);
        }
    }
}
#endregion