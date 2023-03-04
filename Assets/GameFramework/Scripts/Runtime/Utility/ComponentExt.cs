using System;
using UnityEngine;

namespace UnityGameFramework.Runtime
{
    public static class ComponentExt
    {
        public static T AddComponent<T>(this Component component) where T : Component =>
            component.gameObject.AddComponent<T>();

        public static T GetComponent<T>(this Component component, string name) where T : Component
        {
            if (string.IsNullOrEmpty(name))
                return component.GetComponent<T>();
            Transform transform = component.transform.Find(name);
            return (UnityEngine.Object)transform == (UnityEngine.Object)null ? default(T) : transform.GetComponent<T>();
        }

        public static T GetComponent<T>(this Component component, int index) where T : Component
        {
            if (index < 0 || index >= component.transform.childCount)
                return component.GetComponent<T>();
            Transform child = component.transform.GetChild(index);
            return (UnityEngine.Object)child == (UnityEngine.Object)null ? default(T) : child.GetComponent<T>();
        }

        public static T GetComponent2<T>(this Component component) where T : Component
        {
            T component2 = component.GetComponent<T>();
            if ((UnityEngine.Object)component2 == (UnityEngine.Object)null)
                component2 = component.GetComponentInChildren<T>(true);
            return component2;
        }

        public static T GetComponent2<T>(this Component component, string name) where T : Component
        {
            if (string.IsNullOrEmpty(name))
                return component.GetComponent2<T>();
            Transform transform = component.transform.Find(name);
            if ((UnityEngine.Object)transform == (UnityEngine.Object)null)
            {
                transform = component.transform.Find2(name);
                if ((UnityEngine.Object)transform == (UnityEngine.Object)null)
                    return default(T);
            }

            return transform.GetComponent<T>();
        }

        public static T GetComponentOrAdd<T>(this Component component) where T : Component
        {
            T componentOrAdd = component.GetComponent<T>();
            if ((UnityEngine.Object)componentOrAdd == (UnityEngine.Object)null)
                componentOrAdd = component.gameObject.AddComponent<T>();
            return componentOrAdd;
        }

        public static T GetComponentOrAdd<T>(this Component component, string name) where T : Component
        {
            if (string.IsNullOrEmpty(name))
                return component.GetComponentOrAdd<T>();
            Transform component1 = component.transform.Find(name);
            if ((UnityEngine.Object)component1 == (UnityEngine.Object)null)
            {
                component1 = component.transform.Find2(name);
                if ((UnityEngine.Object)component1 == (UnityEngine.Object)null)
                    return default(T);
            }

            return component1.GetComponentOrAdd<T>();
        }

        public static T GetComponentOrAdd<T>(this Component component, int index) where T : Component
        {
            if (index < 0 || index >= component.transform.childCount)
                return component.GetComponentOrAdd<T>();
            Transform child = component.transform.GetChild(index);
            return (UnityEngine.Object)child == (UnityEngine.Object)null ? default(T) : child.GetComponentOrAdd<T>();
        }

        public static void SetFather(this Component component, Transform parent) => component.SetFather(parent, true);

        public static void SetFather(this Component component, Transform parent, bool reset)
        {
            if ((UnityEngine.Object)component.transform.parent != (UnityEngine.Object)parent)
                component.transform.SetParent(parent);
            if (!reset)
                return;
            RectTransform transform = component.transform as RectTransform;
            if ((UnityEngine.Object)transform != (UnityEngine.Object)null)
            {
                transform.anchoredPosition = Vector2.zero;
                transform.sizeDelta = Vector2.zero;
            }
            else
            {
                component.transform.localPosition = Vector3.zero;
                component.transform.localRotation = Quaternion.identity;
                component.transform.localScale = Vector3.one;
            }
        }

        public static void SetFather(this Component component, Transform parent, string name)
        {
            if ((UnityEngine.Object)parent == (UnityEngine.Object)null || string.IsNullOrEmpty(name))
            {
                component.SetFather(parent, true);
            }
            else
            {
                Transform parent1 = parent.Find2(name);
                component.SetFather(parent1, true);
            }
        }

        public static void SetLayerBy(this Component component, int index)
        {
            int num = 1 << index;
            if (component.gameObject.layer == num)
                return;
            component.gameObject.layer = num;
        }

        public static void SetLayer(this Component component, int layer)
        {
            if (component.gameObject.layer == layer)
                return;
            component.gameObject.layer = layer;
        }

        public static void SetLayer(this Component component, string name)
        {
            int layer = LayerMask.NameToLayer(name);
            if (component.gameObject.layer == layer)
                return;
            component.gameObject.layer = layer;
        }

        public static void SetActive(this Component component, bool active)
        {
            if (component.gameObject.activeSelf == active)
                return;
            component.gameObject.SetActive(active);
        }

        public static void SafeDestroy(this Component component) => ComponentExt.SafeDestroy(component, true);

        public static void SafeDestroy(this Component component, bool includeAsset)
        {
            if ((UnityEngine.Object)component == (UnityEngine.Object)null)
                return;
            try
            {
                UnityEngine.Object.Destroy((UnityEngine.Object)component);
                if (!includeAsset)
                    return;
                UnityEngine.Object.DestroyImmediate((UnityEngine.Object)component.gameObject, true);
            }
            catch (MissingComponentException ex)
            {
                Log.Fatal((Exception)ex);
            }
            catch (MissingReferenceException ex)
            {
                Log.Fatal((Exception)ex);
            }
            catch (NullReferenceException ex)
            {
                Log.Fatal((Exception)ex);
            }
            catch (UnassignedReferenceException ex)
            {
                Log.Fatal((Exception)ex);
            }
        }

        public static Component AddComponent(this Component component, System.Type type) =>
            component.gameObject.AddComponent(type);

        public static Component GetComponent(this Component component, System.Type type, string name)
        {
            if (string.IsNullOrEmpty(name))
                return component.GetComponent(type);
            Transform transform = component.transform.Find(name);
            return (UnityEngine.Object)transform == (UnityEngine.Object)null
                ? (Component)null
                : transform.GetComponent(type);
        }

        public static Component GetComponent(this Component component, System.Type type, int index)
        {
            if (index < 0 || index >= component.transform.childCount)
                return component.GetComponent(type);
            Transform child = component.transform.GetChild(index);
            return (UnityEngine.Object)child == (UnityEngine.Object)null ? (Component)null : child.GetComponent(type);
        }

        public static Component GetComponent2(this Component component, System.Type type)
        {
            Component component2 = component.GetComponent(type);
            if ((UnityEngine.Object)component2 == (UnityEngine.Object)null)
                component2 = component.GetComponentInChildren(type, true);
            return component2;
        }

        public static Component GetComponent2(
            this Component component,
            System.Type type,
            string name)
        {
            if (string.IsNullOrEmpty(name))
                return component.GetComponent2(type);
            Transform transform = component.transform.Find(name);
            if ((UnityEngine.Object)transform == (UnityEngine.Object)null)
            {
                transform = component.transform.Find2(name);
                if ((UnityEngine.Object)transform == (UnityEngine.Object)null)
                    return (Component)null;
            }

            return transform.GetComponent(type);
        }

        public static Component GetComponentOrAdd(this Component component, System.Type type)
        {
            Component componentOrAdd = component.GetComponent(type);
            if ((UnityEngine.Object)componentOrAdd == (UnityEngine.Object)null)
                componentOrAdd = component.gameObject.AddComponent(type);
            return componentOrAdd;
        }

        public static Component GetComponentOrAdd(
            this Component component,
            System.Type type,
            string name)
        {
            if (string.IsNullOrEmpty(name))
                return component.GetComponentOrAdd(type);
            Transform component1 = component.transform.Find(name);
            if ((UnityEngine.Object)component1 == (UnityEngine.Object)null)
            {
                component1 = component.transform.Find2(name);
                if ((UnityEngine.Object)component1 == (UnityEngine.Object)null)
                    return (Component)null;
            }

            return component1.GetComponentOrAdd(type);
        }

        public static Component GetComponentOrAdd(
            this Component component,
            System.Type type,
            int index)
        {
            if (index < 0 || index >= component.transform.childCount)
                return component.GetComponentOrAdd(type);
            Transform child = component.transform.GetChild(index);
            return (UnityEngine.Object)child == (UnityEngine.Object)null
                ? (Component)null
                : child.GetComponentOrAdd(type);
        }
    }
}