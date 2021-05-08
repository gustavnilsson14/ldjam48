using System.Collections.Generic;

namespace UnityEngine
{

    public static class UnityEngineEx
    {

        public static T GetComponentInDirectChildren<T>(this Component parent)
        {
            return parent.GetComponentInDirectChildren<T>(false);
        }

        public static T GetComponentInDirectChildren<T>(this Component parent, bool includeInactive)
        {
            foreach (Transform transform in parent.transform)
            {
                if (includeInactive || transform.gameObject.activeInHierarchy)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                    {
                        return component;
                    }
                }
            }
            return default(T);
        }

        public static T[] GetComponentsInDirectChildren<T>(this Component parent)
        {
            return parent.GetComponentsInDirectChildren<T>(false);
        }

        public static T[] GetComponentsInDirectChildren<T>(this Component parent, bool includeInactive)
        {
            List<T> tmpList = new List<T>();
            foreach (Transform transform in parent.transform)
            {
                if (includeInactive || transform.gameObject.activeInHierarchy)
                {
                    tmpList.AddRange(transform.GetComponents<T>());
                }
            }
            return tmpList.ToArray();
        }

        public static T GetComponentInSiblings<T>(this Component sibling)
        {
            return sibling.GetComponentInSiblings<T>(false);
        }

        public static T GetComponentInSiblings<T>(this Component sibling, bool includeInactive)
        {
            Transform parent = sibling.transform.parent;
            if (parent == null) return default(T);
            foreach (Transform transform in parent)
            {
                if (includeInactive || transform.gameObject.activeInHierarchy)
                {
                    if (transform != sibling)
                    {
                        T component = transform.GetComponent<T>();
                        if (component != null)
                        {
                            return component;
                        }
                    }
                }
            }
            return default(T);
        }

        public static T[] GetComponentsInSiblings<T>(this Component sibling)
        {
            return sibling.GetComponentsInSiblings<T>(false);
        }

        public static T[] GetComponentsInSiblings<T>(this Component sibling, bool includeInactive)
        {
            Transform parent = sibling.transform.parent;
            if (parent == null) return null;
            List<T> tmpList = new List<T>();
            foreach (Transform transform in parent)
            {
                if (includeInactive || transform.gameObject.activeInHierarchy)
                {
                    if (transform != sibling)
                    {
                        tmpList.AddRange(transform.GetComponents<T>());
                    }
                }
            }
            return tmpList.ToArray();
        }

        public static T GetComponentInDirectParent<T>(this Component child)
        {
            Transform parent = child.transform.parent;
            if (parent == null) return default(T);
            return parent.GetComponent<T>();
        }

        public static T[] GetComponentsInDirectParent<T>(this Component child)
        {
            Transform parent = child.transform.parent;
            if (parent == null) return null;
            return parent.GetComponents<T>();
        }
    }
}