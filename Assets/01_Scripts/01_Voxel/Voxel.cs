using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel 
{
    public byte ID;

    public bool isSolid
    { 
        get
        {
            return ID != 0;
        } 
    }


}
