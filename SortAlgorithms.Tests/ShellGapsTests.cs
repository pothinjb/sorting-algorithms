using Microsoft.VisualStudio.TestTools.UnitTesting;
using SortAlgorithms.Core;
using System;
using System.Linq;

namespace SortAlgorithms.Tests
{
    /// <summary>
    /// Checks that <see cref="Gaps.GenerateSequence"/> computed up to <see cref="int.MaxValue"/>
    /// matches the precomputed gap tables in <see cref="Gaps"/>.
    /// </summary>
    [TestClass]
    public class ShellGapsTests
    {
        /// <summary>The generated Hibbard sequence matches <see cref="Gaps.Hibbard"/>.</summary>
        [TestMethod]
        public void Test_Hibbard_MaxValue()
        {
            int[] result = Gaps.GenerateSequence(int.MaxValue, ShellSequence.Hibbard);
            CollectionAssert.AreEqual(Gaps.Hibbard, result);
        }

        /// <summary>The generated Knuth sequence matches <see cref="Gaps.Knuth"/>.</summary>
        [TestMethod]
        public void Test_Knuth_MaxValue()
        {
            int[] result = Gaps.GenerateSequence(int.MaxValue, ShellSequence.Knuth);
            CollectionAssert.AreEqual(Gaps.Knuth, result);
        }

        /// <summary>The generated Lee sequence matches <see cref="Gaps.Lee"/>.</summary>
        [TestMethod]
        public void Test_Lee_MaxValue()
        {
            int[] result = Gaps.GenerateSequence(int.MaxValue, ShellSequence.Lee);
            CollectionAssert.AreEqual(Gaps.Lee, result);
        }

        /// <summary>The generated PapernovStasevich sequence matches <see cref="Gaps.PapernovStasevich"/>.</summary>
        [TestMethod]
        public void Test_PapernovStasevich_MaxValue()
        {
            int[] result = Gaps.GenerateSequence(int.MaxValue, ShellSequence.PapernovStasevich);
            CollectionAssert.AreEqual(Gaps.PapernovStasevich, result);
        }

        /// <summary>The generated Sedgewick sequence matches <see cref="Gaps.Sedgewick"/>.</summary>
        [TestMethod]
        public void Test_Sedgewick_MaxValue()
        {
            int[] result = Gaps.GenerateSequence(int.MaxValue, ShellSequence.Sedgewick);
            CollectionAssert.AreEqual(Gaps.Sedgewick, result);
        }

        /// <summary>The Incerpi-Sedgewick sequence (which has no generator) comes from <see cref="Gaps.IncerpiSedgewick"/>.</summary>
        [TestMethod]
        public void Test_IncerpiSedgewick()
        {
            CollectionAssert.AreEqual(Gaps.IncerpiSedgewick, Gaps.GenerateSequence(int.MaxValue, ShellSequence.IncerpiSedgewick));
            CollectionAssert.AreEqual(new[] { 112, 48, 21, 7, 3, 1 }, Gaps.GenerateSequence(200, ShellSequence.IncerpiSedgewick));
        }

        /// <summary>
        /// Every sequence that <see cref="ShellSequence"/> lists can be generated, ends with a gap of 1
        /// and never exceeds the array size.
        /// </summary>
        [TestMethod]
        public void Test_AllSequences_AreGenerated()
        {
            const int n = 1000;
            foreach (ShellSequence seq in Enum.GetValues(typeof(ShellSequence)))
            {
                int[] gaps = Gaps.GenerateSequence(n, seq);
                Assert.IsNotNull(gaps, $"{seq}");
                Assert.AreEqual(1, gaps[gaps.Length - 1], $"{seq}: the last gap must be 1");
                Assert.IsTrue(gaps.All(h => h <= n), $"{seq}: a gap is larger than n");
            }
        }
    }
}
