namespace BenchmarkGrainInterfaces.GrainStorage
{
    [GenerateSerializer]
    public class Report
    {
        [Id(1)]
        public bool Success { get; set; }

        [Id(2)]
        public TimeSpan Elapsed { get; set; }
    }

    public interface IPersistentGrain : IGrainWithGuidKey
    {
        Task Init(int payloadSize);
        Task<Report> TrySet(int index);
    }

    public interface IPersistentGrain2DifferentPayloadSize : IGrainWithGuidKey
    {
        Task Init(int payloadSize);
        Task<Report> TrySet(int index);
    }

    public interface IPersistentGrain3WithStringKey : IGrainWithStringKey
    {
        Task Init(int payloadSize);
        Task<Report> TrySet(int index);
    }
}
