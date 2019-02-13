using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TyphoonTests
{
    public static class TestUtils
    {
        public static void TestArrayEquality<T>(T[] expected, T[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length, "Arrays are not the same length");
            for (int i = 0; i < expected.Length; i++)
                Assert.AreEqual(expected[i], actual[i], $"Error at index: {i}");
        }
    }
}
