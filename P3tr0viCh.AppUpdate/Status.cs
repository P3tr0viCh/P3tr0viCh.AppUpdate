namespace P3tr0viCh.AppUpdate
{
    public partial class AppUpdate
    {
        public enum Status
        {
            Idle,
            Check,
            CheckLocal,
            CheckLatest,
            Download,
            ArchiveExtract,
            Update,
        }

        public class ProgramStatus : P3tr0viCh.Utils.ProgramStatus<Status>
        {
            public ProgramStatus() : base(AppUpdate.Status.Idle)
            {
            }
        }
    }
}