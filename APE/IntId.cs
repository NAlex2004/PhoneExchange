using System;
using System.CodeDom;
using NAlex.APE.Interfaces;

namespace NAlex.APE
{
    public struct IntId: IPortId
    {
        public int Id { get; set; }

        public void IncreaseValue()
        {
            Id++;
        }

        public void DecreaseValue()
        {
            Id--;
        }

        public IPortId StartValue()
        {
            return new IntId() {Id = 1};
        }

        public string Value
        {
            get { return Id.ToString(); }
        }

        public bool Equals(IPortId other)
        {
            return other != null && Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
        }
    }
}