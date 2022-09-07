using System.Numerics;
using System.Security.Cryptography;

namespace Tools.Extensions
{
    public static class BigIntegerExtensions
    {
        /// <summary>
        /// Constructs a randomly generated <see cref="BigInteger"/>, uniformly distributed over the range <c>0</c> to <c>(2^numBits - 1)</c>, inclusive.
        /// </summary>
        /// <param name="numBits">Maximum bitLength of the new <see cref="BigInteger"/>.</param>
        /// <param name="random">Source of randomness to be used in computing the new <see cref="BigInteger"/>.</param>
        /// <returns>A new random positive <see cref="BigInteger"/> within the given bounds.</returns>
        public static BigInteger Random(int numBits, RandomNumberGenerator random)
        {
            if (numBits < 0)
                throw new ArgumentOutOfRangeException(nameof(numBits), numBits, "No negative numbers allowed");
            if (numBits == 1)
                return BigInteger.Zero;

            // Calculate length
            int bytes = numBits / 8;
            int bits = numBits % 8;

            // Generates random bytes
            byte[] bs = new byte[bytes + 1];
            random.GetBytes(bs);

            // Mask out unnecessary bits
            byte mask = (byte)(0xFF >> (8 - bits));
            bs[^1] &= mask;

            return new BigInteger(bs);
        }

        /// <summary>
        /// Constructs a randomly generated positive <see cref="BigInteger"/> that is probably prime, with the specified bitLength.
        /// </summary>
        /// <param name="bitLength">Bit length of the returned <see cref="BigInteger"/>.</param>
        /// <param name="certainty">
        /// A measure of the uncertainty that the caller is willing to tolerate.
        /// The probability that the new <see cref="BigInteger"/> represents a prime number will exceed <c>(1 - (1/2^certainty))</c>.
        /// The execution time of the method is proportional to the value of this parameter.
        /// </param>
        /// <param name="random">Source of random bits used to select candidates to be tested for primality.</param>
        /// <returns>A new random positive <see cref="BigInteger"/> that is probably prime within the given bounds.</returns>
        public static BigInteger RandomPrime(int bitLength, int certainty, RandomNumberGenerator random)
        {
            if (bitLength < 2)
                throw new ArithmeticException();

            // Generate random numbers until a prime number has been found
            BigInteger bigInt;
            do
            {
                bigInt = Random(bitLength, random);
            } while (bigInt.IsProbablePrime(certainty));

            return bigInt;
        }

        /// <summary>
        /// Checks if a given <see cref="BigInteger"/> is probably prime.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="certainty">
        /// A measure of the uncertainty that the caller is willing to tolerate:
        /// if the call returns true the probability that this BigInteger is prime exceeds <c>(1 - (1/2^certainty))</c>.
        /// The execution time of this method is proportional to the value of this parameter
        /// </param>
        /// <returns>true if this BigInteger is probably prime, false if it is definitely composite.</returns>
        public static bool IsProbablePrime(this BigInteger source, int certainty)
        {
            if (source == 2 || source == 3)
                return true;
            if (source < 2 || source % 2 == 0)
                return false;

            BigInteger d = source - 1;
            int s = 0;

            while (d % 2 == 0)
            {
                d /= 2;
                s += 1;
            }

            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] bytes = new byte[source.ToByteArray().LongLength];
            BigInteger a;

            // Miller-Rabin test
            for (int i = 0; i < certainty; i++)
            {
                do
                {
                    rng.GetBytes(bytes);
                    a = new BigInteger(bytes);
                } while (a < 2 || a >= source - 2);

                BigInteger x = BigInteger.ModPow(a, d, source);
                if (x == 1 || x == source - 1)
                    continue;

                for (int r = 1; r < s; r++)
                {
                    x = BigInteger.ModPow(x, 2, source);
                    if (x == 1)
                        return false;
                    if (x == source - 1)
                        break;
                }

                if (x != source - 1)
                    return false;
            }

            return true;
        }
    }
}
