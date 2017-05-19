namespace NAlex.APE.Interfaces
{
    public interface ITerminal
    {
        void StartCall(IPortId portId);
        void EndCall();

        event CallEventHandler CallStarted;
        event CallEventHandler CallEnded;
    }
}