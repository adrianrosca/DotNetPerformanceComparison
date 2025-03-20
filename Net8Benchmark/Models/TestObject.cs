using System;
using System.Collections.Generic;

namespace Net8Benchmark.Models
{
    public class TestObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<TestItem> Items { get; set; }
    }
}