using UnityEngine;

public static class Constants
{
    public static class Shaders
    {
        public static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
        public static readonly int MainTex = Shader.PropertyToID("_MainTex");
    }
    public static class Layers
    {
        public const string Player = "Player";
        public const string Enemy = "Enemy";
        public const string Ground = "Ground";
    }
}
