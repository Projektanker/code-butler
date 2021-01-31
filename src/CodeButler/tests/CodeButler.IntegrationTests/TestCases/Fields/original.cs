using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeButler.IntegrationTests.TestCases.Fields
{
    class Test
    {
        private string _testWithOutAttribute;
        private int _test;
        public static int _testPublicStatic;
        // Comment
        public static int _testPublicStaticComment;
        [Obsolete]        
        private string _testWithAttribute;   
        private readonly int _testReadOnly;            
    }
}