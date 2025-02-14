﻿using Sirenix.OdinInspector;
using UnityEngine;

namespace Amir.Level
{
    public class LevelSettings : MonoBehaviour
    {
        public Transform startLevelPoint;
        public Transform endLevelPoint;
        public bool stuckBySix;
        //public Transform flyStartPoint;

        public bool applyColorScheme = false;

        [ShowIf("applyColorScheme")] public ColorSchemeObject colorScheme;

        private void Start()
        {
            if (applyColorScheme)
                colorScheme.ApplyScheme();
        }

        private void Update()
        {
            if (StaticManager.instance.debugMode)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                    colorScheme.ApplyScheme();
            }
        }
    }
}