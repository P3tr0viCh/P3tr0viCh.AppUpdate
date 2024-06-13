namespace P3tr0viCh.AppUpdate
{
    public enum UpdateStatus
    {
        Idle,
        Check,
        CheckLocal,
        CheckLatest,
        Download,
        ArchiveExtract,
        Update,
    }

    public class ProgramStatus : P3tr0viCh.Utils.ProgramStatus<UpdateStatus>
    {
        public ProgramStatus() : base(UpdateStatus.Idle)
        {
        }
    }
}