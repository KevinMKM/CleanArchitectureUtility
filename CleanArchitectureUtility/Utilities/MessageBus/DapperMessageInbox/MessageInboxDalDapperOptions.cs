namespace CleanArchitectureUtility.Utilities.MessageBus.DapperMessageInbox
{
    public class MessageInboxDalDapperOptions
    {
        public string ConnectionString { get; set; } = string.Empty;
        public bool AutoCreateSqlTable { get; set; } = true;
        public string TableName { get; set; } = "MessageInbox";
        public string SchemaName { get; set; } = "";
    }
}