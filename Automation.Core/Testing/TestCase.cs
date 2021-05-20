using Automation.Core.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Automation.Core.Testing
{
    public abstract class TestCase
    {
        //fields
        private IDictionary<string, object> testParams;
        private int attempts;
        private ILogger logger;

        protected TestCase()
        {
            testParams = new Dictionary<string, object>();
            attempts = 1;
            logger = new TraceLogger();
        } 

        //components
        public abstract bool AutomationTest(IDictionary<string, object> testParams);

        public TestCase Execute()
        {
            Actual = AutomationTest(testParams);

            return this;
        }

        //propertis
        public bool Actual { get; set; }


        //configuration
        public TestCase WithTestParams(IDictionary<string, object> testParams)
        {
            this.testParams = testParams;
            return this;
        }

        public TestCase WithNumberOfAttempts(int numberOfAttemptes)
        {
            attempts = numberOfAttemptes;
            return this;
        }

        public TestCase WithLogger(ILogger logger)
        {
            this.logger = logger;
            return this;
        }

    }
}
