namespace CleanArchitectureUtility.Utilities.Databases.Hamster
{
    public class ChangeDataLogHamsterOptions
    {
        public List<string> PropertyForReject { get; set; } = new()
        {
            "CreatedByUserId",
            "CreatedDateTime",
            "ModifiedByUserId",
            "ModifiedDateTime"
        };

        public string? BusinessIdFieldName { get; set; } = "BusinessId";
    }
}