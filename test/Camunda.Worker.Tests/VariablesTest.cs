using System;
using Xunit;

namespace Camunda.Worker
{
    public class VariablesTest
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestAsBoolean(bool value)
        {
            var variable = new Variable(value, VariableType.Boolean);

            Assert.Equal(value, variable.AsBoolean());
        }

        [Theory]
        [InlineData((long) short.MinValue)]
        [InlineData(0L)]
        [InlineData((long) short.MaxValue)]
        public void TestAsShort(long value)
        {
            var variable = new Variable(value, VariableType.Short);

            Assert.Equal(value, variable.AsShort());
        }

        [Theory]
        [InlineData((long) int.MinValue)]
        [InlineData(0L)]
        [InlineData((long) int.MaxValue)]
        public void TestAsInteger(long value)
        {
            var variable = new Variable(value, VariableType.Integer);

            Assert.Equal(value, variable.AsInteger());
        }

        [Theory]
        [InlineData(long.MinValue)]
        [InlineData(0L)]
        [InlineData(long.MaxValue)]
        public void TestAsLong(long value)
        {
            var variable = new Variable(value, VariableType.Long);

            Assert.Equal(value, variable.AsLong());
        }

        [Fact]
        public void TestAsDouble()
        {
            var value = 0.0;

            var variable = new Variable(value, VariableType.Double);

            Assert.Equal(value, variable.AsDouble());
        }

        [Fact]
        public void TestAsString()
        {
            var value = "MyString";

            var variable = new Variable(value, VariableType.String);

            Assert.Equal(value, variable.AsString());
        }

        [Fact]
        public void TestAsBytes()
        {
            var value = new byte[]{ 127 };

            var variable = new Variable(Convert.ToBase64String(value), VariableType.Bytes);

            Assert.Equal(value, variable.AsBytes());
        }
    }
}
