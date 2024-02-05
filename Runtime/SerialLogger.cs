using UnityEngine;

namespace UitkForKsp2.Controls
{
    public static class SerialLogger
    {
        private static int _serialValue;

        public static void Log(object value)
        {
            Debug.Log($"{value} --- {_serialValue++}");
        }
    }
}