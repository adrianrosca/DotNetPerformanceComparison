namespace NetFramework48Benchmark.Services
{
    public class DataProcessor : IDataProcessor
    {
        private readonly IComplexService _complexService;

        public DataProcessor(IComplexService complexService)
        {
            _complexService = complexService;
        }

        public int CalculateValue(int input)
        {
            return _complexService.ProcessData(input.ToString()).Length;
        }
    }
}