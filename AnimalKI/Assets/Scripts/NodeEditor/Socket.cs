using System;
using System.Collections.Generic;
using UnityEngine;

namespace NodeSystem
{
    public abstract class Socket : ScriptableObject
    {
        [NonSerialized]
        private object value = null;
        private Type valueType;

        //public Rect rect;
        public Node parentNode;
        public TypeData typeData;

        public Rect rect;

        public List<Connection> connections = new List<Connection>();

        public void ValidateConnections()
        {
            for (int i = connections.Count-1; i >= 0; i-=1)
            {
                if (connections[i].startSocket == null || connections[i].endSocket == null)
                    connections.Remove(connections[i]);
            }
        }

        public abstract Socket Create(Node node, TypeData type);

        protected internal virtual void DrawSocket(Rect socketRect)
        {
            //Draw socket

            //Draw outgoing Connections
        }

        public bool IsValueNull { get { return value == null; } }

        public void SetValue<T>(T Value)
        {
            if (valueType == null)
                valueType = typeData.Type;
            if (valueType == typeof(T))
                value = Value;
            else
                Debug.LogError("Trying to SetValue<" + typeof(T).FullName + "> for Output Type: " + valueType.FullName);
        }

        public T GetValue<T>()
        {
            if (valueType == null)
                valueType = typeData.Type;
            if (valueType == typeof(T))
            {
                if (value == null)
                    value = getDefault<T>();
                return (T)value;
            }
            Debug.LogError("Trying to GetValue<" + typeof(T).FullName + "> for Output Type: " + valueType.FullName);
            return getDefault<T>();
        }
        

        public void ResetValue()
        {
            value = null;
        }
        

        public static T getDefault<T>()
        {
            if (typeof(T).GetConstructor(Type.EmptyTypes) != null) // Try to create using an empty constructor if existant
                return Activator.CreateInstance<T>();
            else // Else try to get default. Returns null only on reference types
                return default(T);
        }
    }        

    public class SocketIn : Socket
    {
        public override Socket Create(Node node, TypeData type)
        {
            parentNode = node;
            typeData = type;

            return this;
        }
    }

    public class SocketOut : Socket
    {
        public override Socket Create(Node node, TypeData type)
        {
            parentNode = node;
            typeData = type;

            return this;
        }
    }
}
