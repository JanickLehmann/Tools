using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Tools.Extensions;

namespace Tools.UnitTests.Cryptography
{
    public class BigIntegerExtensionsTests
    {
        [SetUp]
        public void Setup()
        {
        }

        private static readonly IEnumerable<int> _randomBigIntCases = Enumerable.Range(0, 256).Select(i => Enumerable.Range(0, 100).Select(j => i)).SelectMany(n => n);

        [TestCaseSource(nameof(_randomBigIntCases))]
        public void RandomBigIntegerTest(int numBits)
        {
            using var random = RandomNumberGenerator.Create();
            var result = BigIntegerExtensions.Random(numBits, random);

            Assert.That(result, Is.LessThanOrEqualTo(BigInteger.Pow(2, numBits) - 1));
        }

        public void RandomBigIntegerPrimeTest(int bitLength)
        {
            Assert.Fail();
        }
    }
}
