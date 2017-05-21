using System;

namespace NAlex.APE.Interfaces
{
    public interface IPortId: IEquatable<IPortId>
    {
        IPortId NextValue();
        IPortId StartValue();
        string Value { get; }
    }
}