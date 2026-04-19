namespace Code.Misc
{
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static System.Reflection.BindingFlags;

/// <summary> An interface for volume components that can be updated. </summary>
interface IUpdatableVolumeComponent
{
    /// <summary> Called every frame to update the volume component. </summary>
    void Update();
    
    /// <summary> Indicates whether the component should be updated in edit mode. </summary>
    bool ExecuteInEditMode => true;
}


}