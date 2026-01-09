using System;
using UnityEngine;

namespace CustomAttributes
{
    /// <summary>
    /// I developed this attribute during my studies. So, it's quite simple, but somtimes 
    /// useful in my situation.
    /// </summary>
    /// <remarks>Make sure that the field where you applied this attribute has defined type
    /// of <see cref="UnityEngine.Object"/>.</remarks>
    public class ForceInterfaceAttribute : PropertyAttribute
    {
        public readonly Type InterfaceType;

        public ForceInterfaceAttribute(Type interfaceType) => InterfaceType = interfaceType;
    }
}