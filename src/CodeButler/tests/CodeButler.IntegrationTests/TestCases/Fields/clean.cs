using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeButler.IntegrationTests.TestCases.Fields
{
    class Test
    {
        public static int _testPublicStatic;
        // Comment
        public static int _testPublicStaticComment;
        private readonly int _testReadOnly;
        private int _test;
        [Obsolete]
        private string _testWithAttribute;
        private string _testWithOutAttribute;
    }
}