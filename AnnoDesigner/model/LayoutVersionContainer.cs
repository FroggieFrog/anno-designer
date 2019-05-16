﻿using System.Runtime.Serialization;

namespace AnnoDesigner.model
{
    /// <summary>
    /// Container with just the file version number
    /// </summary>
    [DataContract]
    public class LayoutVersionContainer
    {
        [DataMember(Order = 0)]
        public int FileVersion { get; set; }
    }
}