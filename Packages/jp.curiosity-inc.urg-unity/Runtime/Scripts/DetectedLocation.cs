﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Urg
{
    public class DetectedLocation : ICloneable
    {
        public int rawIndex;
        public float angle;
        public float distance;

        public DetectedLocation(int rawIndex, float angle, float distance)
        {
            this.rawIndex = rawIndex;
            this.angle = angle;
            this.distance = distance;
        }

        public Vector2 ToPosition2D()
        {
            var pos3d = ToPosition();
            return new Vector2(pos3d.x, pos3d.z);
        }

        public Vector3 ToPosition()
        {
            return ToPosition(Vector3.right, Vector3.up);
        }

        public Vector3 ToPosition(Vector3 forward, Vector3 normal)
        {
            return distance * (Quaternion.AngleAxis(-angle, normal) * forward);
        }

        public object Clone()
        {
            return new DetectedLocation(this.rawIndex, this.angle, this.distance);
        }
    }
}
