// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Runtime.InteropServices.Tests
{
    public class CharArrayTests
    {
        public static readonly object[][] ArrayData =
        {
            new object[] { new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j' } }
        };

        [Theory]
        [MemberData(nameof(ArrayData))]
        public void NullValueArguments_ThrowsArgumentNullException(char[] TestArray)
        {
            char[] array = null;
            AssertExtensions.Throws<ArgumentNullException>("destination", () => Marshal.Copy(TestArray, 0, IntPtr.Zero, 0));
            AssertExtensions.Throws<ArgumentNullException>("source", () => Marshal.Copy(array, 0, new IntPtr(1), 0));
            AssertExtensions.Throws<ArgumentNullException>("destination", () => Marshal.Copy(new IntPtr(1), array, 0, 0));
            AssertExtensions.Throws<ArgumentNullException>("source", () => Marshal.Copy(IntPtr.Zero, TestArray, 0, 0));
        }

        [Theory]
        [MemberData(nameof(ArrayData))]
        public void OutOfRangeArguments_ThrowsArgumentOutOfRangeException(char[] TestArray)
        {
            int sizeOfArray = sizeof(char) * TestArray.Length;
            IntPtr ptr = Marshal.AllocCoTaskMem(sizeOfArray);
            try
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => Marshal.Copy(TestArray, 0, ptr, TestArray.Length + 1));
                Assert.Throws<ArgumentOutOfRangeException>(() => Marshal.Copy(TestArray, TestArray.Length + 1, ptr, 1));
                Assert.Throws<ArgumentOutOfRangeException>(() => Marshal.Copy(TestArray, 2, ptr, TestArray.Length));
            }
            finally
            {
                Marshal.FreeCoTaskMem(ptr);
            }
        }

        [Theory]
        [MemberData(nameof(ArrayData))]
        public void CopyRoundTrip_MatchesOriginalArray(char[] TestArray)
        {
            int sizeOfArray = sizeof(char) * TestArray.Length;
            IntPtr ptr = Marshal.AllocCoTaskMem(sizeOfArray);
            try
            {
                Marshal.Copy(TestArray, 0, ptr, TestArray.Length);

                char[] array1 = new char[TestArray.Length];
                Marshal.Copy(ptr, array1, 0, TestArray.Length);
                Assert.Equal<char>(TestArray, array1);

                Marshal.Copy(TestArray, 2, ptr, TestArray.Length - 4);
                char[] array2 = new char[TestArray.Length];
                Marshal.Copy(ptr, array2, 2, TestArray.Length - 4);
                Assert.Equal<char>(TestArray.AsSpan(2, TestArray.Length - 4).ToArray(), array2.AsSpan(2, TestArray.Length - 4).ToArray());
            }
            finally
            {
                Marshal.FreeCoTaskMem(ptr);
            }
        }
    }
}
