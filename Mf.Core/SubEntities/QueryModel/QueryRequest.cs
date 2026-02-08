namespace Mf.Core.SubEntities.QueryModel
{
    public class QueryRequest
    {
        public string? ObjectType { get; set; }
        public List<Filter> Filters { get; set; } = new();
        public List<string> Includes { get; set; } = new();
        public int? Skip { get; set; }
        public int? Take { get; set; }

    }

    public class AlterRequest
    {
        public string? ObjectType { get; set; }

        public object? Model { get; set; }
    }
}
