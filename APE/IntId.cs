using NAlex.APE.Interfaces;

namespace NAlex.APE
{
    public struct IntId: IPortId
    {
        public int Id { get; set; }

        public string Value
        {
            get { return Id.ToString(); }
        }
    }
}