﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CS4455.Utility;

public class CharacterCommon {



    public static bool CheckGroundNear(
        Vector3 charPos,       
        float jumpableGroundNormalMaxAngle, 
        float rayDepth, //how far down from charPos will we look for ground?
        float rayOriginOffset, //charPos near bottom of collider, so need a fudge factor up away from there
        out bool isJumpable,
        LayerMask whatisGround // Layermask for determining what layers are considered ground for the Raycast
    ) 
    {

        bool ret = false;
        bool _isJumpable = false;


        float totalRayLen = rayOriginOffset + rayDepth;

        Ray ray = new Ray(charPos + Vector3.up * rayOriginOffset, Vector3.down);

        RaycastHit[] hits = Physics.RaycastAll(ray, totalRayLen, whatisGround);

        RaycastHit groundHit = new RaycastHit();

        foreach(RaycastHit hit in hits)
        {

            ret = true;

            groundHit = hit;

            _isJumpable = Vector3.Angle(Vector3.up, hit.normal) < jumpableGroundNormalMaxAngle;

            break; //only need to find the ground once

        }

        Helper.DrawRay(ray, totalRayLen, hits.Length > 0, groundHit, Color.magenta, Color.green);

        isJumpable = _isJumpable;

        return ret;
    }
}
