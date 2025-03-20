namespace NetFramework48Benchmark.Services
{
    public class ComplexService : IComplexService
    {
        private readonly ITestService _testService;

        public ComplexService(ITestService testService)
        {
            _testService = testService;
        }

        public string ProcessData(string input)
        {
            return $"Processed: {_testService.GetData()} + {input}";
        }
    }
}