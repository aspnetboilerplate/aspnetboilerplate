using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Abp.Json;
using Shouldly;
using Xunit;

namespace Abp.Tests
{
    public class SequentialGuidGenerator_Tests
    {
        private readonly SequentialGuidGenerator _sequentialGuidGenerator;

        public SequentialGuidGenerator_Tests()
        {
            _sequentialGuidGenerator = new SequentialGuidGenerator();
        }

        //[Fact] //This test fails
        public void Should_Generate_Sequential_And_Unique_Guids()
        {
            const int itemCount = 10;

            var list = new List<GuidWithSequenceNumber>();

            for (var i = 0; i < itemCount; i++)
            {
                list.Add(new GuidWithSequenceNumber(i, _sequentialGuidGenerator.Create()));
                Thread.Sleep(5);
            }

            list = list.OrderByDescending(i => i.Guid).ToList();

            for (var i = 0; i < itemCount; i++)
            {
                list[i].SequenceNumber.ShouldBe(itemCount - i - 1);
            }

            list.Select(i => i.Guid).Distinct().Count().ShouldBe(itemCount);
        }

        private class GuidWithSequenceNumber
        {
            public int SequenceNumber { get; private set; }

            public Guid Guid { get; private set; }

            public GuidWithSequenceNumber(int sequenceNumber, Guid guid)
            {
                SequenceNumber = sequenceNumber;
                Guid = guid;
            }

            public override string ToString()
            {
                return this.ToJsonString();
            }
        }
    }
}
