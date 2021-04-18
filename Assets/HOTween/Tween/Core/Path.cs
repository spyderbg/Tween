using System;
using UnityEngine;

namespace Holoville.HOTween.Core {

internal class Path
{
    public float pathLength;
    public float[] waypointsLength;
    public float[] timesTable;
    private float[] lengthsTable;
    internal Vector3[] path;
    internal bool changed;
    private Vector3[] drawPs;
    private PathType pathType;

    public Path(PathType type, params Vector3[] path)
    {
        pathType = type;
        this.path = new Vector3[path.Length];
        Array.Copy(path, this.path, this.path.Length);
    }

    public Vector3 GetPoint(float t) =>
        GetPoint(t, out var _);

    internal Vector3 GetPoint(float t, out int outWaypointIndex)
    {
        if (pathType == PathType.Linear)
        {
            if (t <= 0.0)
            {
                outWaypointIndex = 1;
                return path[1];
            }

            var index1 = 0;
            var index2 = 0;
            var length = timesTable.Length;
            for (var index3 = 1; index3 < length; ++index3)
            {
                if (timesTable[index3] >= (double)t)
                {
                    index1 = index3 - 1;
                    index2 = index3;
                    break;
                }
            }

            var num1 = timesTable[index1];
            var num2 = timesTable[index2] - timesTable[index1];
            var maxLength = pathLength * (t - num1);
            var vector3_1 = path[index1];
            var vector3_2 = path[index2];
            outWaypointIndex = index2;
            return vector3_1 + Vector3.ClampMagnitude(vector3_2 - vector3_1, maxLength);
        }

        var num3 = path.Length - 3;
        var num4 = (int)Math.Floor(t * (double)num3);
        var index = num3 - 1;
        if (index > num4)
            index = num4;
        var num5 = t * num3 - index;
        var vector3_3 = path[index];
        var vector3_4 = path[index + 1];
        var vector3_5 = path[index + 2];
        var vector3_6 = path[index + 3];
        outWaypointIndex = -1;
        return 0.5f * ((-vector3_3 + 3f * vector3_4 - 3f * vector3_5 + vector3_6) * (num5 * num5 * num5) +
                       (2f * vector3_3 - 5f * vector3_4 + 4f * vector3_5 - vector3_6) * (num5 * num5) +
                       (-vector3_3 + vector3_5) * num5 + 2f * vector3_4);
    }

    public Vector3 Velocity(float t)
                                {
                                var num1 = path.Length - 3;
                                var num2 = (int)Math.Floor(t * (double)num1);
                                var index = num1 - 1;
                                if (index > num2)
                                index = num2;
                                var num3 = t * num1 - index;
                                var vector3_1 = path[index];
                                var vector3_2 = path[index + 1];
                                var vector3_3 = path[index + 2];
                                var vector3_4 = path[index + 3];
                                return 1.5f * (-vector3_1 + 3f * vector3_2 - 3f * vector3_3 + vector3_4) * (num3 * num3) +
                                (2f * vector3_1 - 5f * vector3_2 + 4f * vector3_3 - vector3_4) * num3 + 0.5f * vector3_3 -
                                0.5f * vector3_1;
                                }

    public void GizmoDraw() =>
        GizmoDraw(-1f, false);

    public void GizmoDraw(float t, bool drawTrig)
                                             {
                                             Gizmos.color = new Color(0.6f, 0.6f, 0.6f, 0.6f);
                                             if (changed || pathType == PathType.Curved && drawPs == null)
                                             {
                                             changed = false;
                                             if (pathType == PathType.Curved)
                                             {
                                             var num = path.Length * 10;
                                             drawPs = new Vector3[num + 1];
                                             for (var index = 0; index <= num; ++index)
                                             {
                                             var point = GetPoint(index / (float)num);
                                             drawPs[index] = point;
                                             }
                                             }
                                             }
                                             
                                             if (pathType == PathType.Linear)
                                             {
                                             var to = path[1];
                                             var length = path.Length;
                                             for (var index = 1; index < length - 1; ++index)
                                             {
                                             var from = path[index];
                                             Gizmos.DrawLine(from, to);
                                             to = from;
                                             }
                                             }
                                             else
                                             {
                                             var to = drawPs[0];
                                             var length = drawPs.Length;
                                             for (var index = 1; index < length; ++index)
                                             {
                                             var drawP = drawPs[index];
                                             Gizmos.DrawLine(drawP, to);
                                             to = drawP;
                                             }
                                             }
                                             
                                             Gizmos.color = Color.white;
                                             var num1 = path.Length - 1;
                                             for (var index = 1; index < num1; ++index)
                                             Gizmos.DrawSphere(path[index], 0.1f);
                                             if (!drawTrig || t == -1.0)
                                             return;
                                             var point1 = GetPoint(t);
                                             var vector3_1 = point1;
                                             var t1 = t + 0.0001f;
                                             Vector3 vector3_2;
                                             Vector3 vector3_3;
                                             if (t1 > 1.0)
                                             {
                                             vector3_2 = point1;
                                             vector3_1 = GetPoint(t - 0.0001f);
                                             vector3_3 = GetPoint(t - 0.0002f);
                                             }
                                             else
                                             {
                                             var t2 = t - 0.0001f;
                                             if (t2 < 0.0)
                                             {
                                             vector3_3 = point1;
                                             vector3_1 = GetPoint(t + 0.0001f);
                                             vector3_2 = GetPoint(t + 0.0002f);
                                             }
                                             else
                                             {
                                             vector3_3 = GetPoint(t2);
                                             vector3_2 = GetPoint(t1);
                                             }
                                             }
                                             
                                             var lhs = vector3_2 - vector3_1;
                                             lhs.Normalize();
                                             var rhs1 = vector3_1 - vector3_3;
                                             rhs1.Normalize();
                                             var rhs2 = Vector3.Cross(lhs, rhs1);
                                             rhs2.Normalize();
                                             var vector3_4 = Vector3.Cross(lhs, rhs2);
                                             vector3_4.Normalize();
                                             Gizmos.color = Color.black;
                                             Gizmos.DrawLine(point1, point1 + lhs);
                                             Gizmos.color = Color.blue;
                                             Gizmos.DrawLine(point1, point1 + rhs2);
                                             Gizmos.color = Color.red;
                                             Gizmos.DrawLine(point1, point1 + vector3_4);
                                             }

    internal Vector3 GetConstPoint(float t) =>
        pathType == PathType.Linear ? GetPoint(t) : GetPoint(GetConstPathPercFromTimePerc(t));

    internal Vector3 GetConstPoint(float t, out float outPathPerc, out int outWaypointIndex)
    {
        if (pathType == PathType.Linear)
        {
            outPathPerc = t;
            return GetPoint(t, out outWaypointIndex);
        }

        var percFromTimePerc = GetConstPathPercFromTimePerc(t);
        outPathPerc = percFromTimePerc;
        outWaypointIndex = -1;
        return GetPoint(percFromTimePerc);
    }

    internal void StoreTimeToLenTables(int subdivisions)
                                                    {
                                                    if (pathType == PathType.Linear)
                                                    {
                                                    pathLength = 0.0f;
                                                    var length = path.Length;
                                                    waypointsLength = new float[length];
                                                    var b = path[1];
                                                    for (var index = 1; index < length; ++index)
                                                    {
                                                    var a = path[index];
                                                    var num = Vector3.Distance(a, b);
                                                    if (index < length - 1)
                                                    pathLength += num;
                                                    b = a;
                                                    waypointsLength[index] = num;
                                                    }
                                                    
                                                    timesTable = new float[length];
                                                    var num1 = 0.0f;
                                                    for (var index = 2; index < length; ++index)
                                                    {
                                                    num1 += waypointsLength[index];
                                                    timesTable[index] = num1 / pathLength;
                                                    }
                                                    }
                                                    else
                                                    {
                                                    pathLength = 0.0f;
                                                    var num = 1f / subdivisions;
                                                    timesTable = new float[subdivisions];
                                                    lengthsTable = new float[subdivisions];
                                                    var b = GetPoint(0.0f);
                                                    for (var index = 1; index < subdivisions + 1; ++index)
                                                    {
                                                    var t = num * index;
                                                    var point = GetPoint(t);
                                                    pathLength += Vector3.Distance(point, b);
                                                    b = point;
                                                    timesTable[index - 1] = t;
                                                    lengthsTable[index - 1] = pathLength;
                                                    }
                                                    }
                                                    }

    internal void StoreWaypointsLengths(int subdivisions)
                                                     {
                                                     var length = this.path.Length - 2;
                                                     waypointsLength = new float[length];
                                                     waypointsLength[0] = 0.0f;
                                                     Path path = null;
                                                     for (var index1 = 2; index1 < length + 1; ++index1)
                                                     {
                                                     var vector3Array = new Vector3[4]
                                                     {
                                                     this.path[index1 - 2],
                                                     this.path[index1 - 1],
                                                     this.path[index1],
                                                     this.path[index1 + 1]
                                                     };
                                                     if (index1 == 2)
                                                     path = new Path(pathType, vector3Array);
                                                     else
                                                     path.path = vector3Array;
                                                     var num1 = 0.0f;
                                                     var num2 = 1f / subdivisions;
                                                     var b = path.GetPoint(0.0f);
                                                     for (var index2 = 1; index2 < subdivisions + 1; ++index2)
                                                     {
                                                     var t = num2 * index2;
                                                     var point = path.GetPoint(t);
                                                     num1 += Vector3.Distance(point, b);
                                                     b = point;
                                                     }
                                                     
                                                     waypointsLength[index1 - 1] = num1;
                                                     }
                                                     }

    private float GetConstPathPercFromTimePerc(float t)
    {
        if (t > 0.0 && t < 1.0)
        {
            var num1 = pathLength * t;
            var num2 = 0.0f;
            var num3 = 0.0f;
            var num4 = 0.0f;
            var num5 = 0.0f;
            var length = lengthsTable.Length;
            for (var index = 0; index < length; ++index)
            {
                if (lengthsTable[index] > (double)num1)
                {
                    num4 = timesTable[index];
                    num5 = lengthsTable[index];
                    if (index > 0)
                    {
                        num3 = lengthsTable[index - 1];
                        break;
                    }

                    break;
                }

                num2 = timesTable[index];
            }

            t = num2 + (float)((num1 - (double)num3) / (num5 - (double)num3) * (num4 - (double)num2));
        }

        if (t > 1.0)
            t = 1f;
        else if (t < 0.0)
            t = 0.0f;
        return t;
    }
}

}