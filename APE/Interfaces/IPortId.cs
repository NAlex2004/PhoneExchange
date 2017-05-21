using System;

namespace NAlex.APE.Interfaces
{
    public interface IPortId: IEquatable<IPortId>
    {
        void IncreaseValue();
        void DecreaseValue();
        IPortId StartValue();
        string Value { get; }
    }
}