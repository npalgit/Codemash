﻿using System;

namespace Codemash.Server.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class EnumKeyAttribute : Attribute
    {
        public string AlternativeName { get; private set; }

        public EnumKeyAttribute(string alternativeName)
        {
            AlternativeName = alternativeName;
        }
    }
}
