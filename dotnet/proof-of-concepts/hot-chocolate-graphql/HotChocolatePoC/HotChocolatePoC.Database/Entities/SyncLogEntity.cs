namespace HotChocolatePoC.Database.Entities
{
    public sealed class SyncLogEntity
    {
        public int Id { get; set; }

        public ApiType Type { get; set; }

        public DateTime LastSyncedAt { get; set; }
    }
}