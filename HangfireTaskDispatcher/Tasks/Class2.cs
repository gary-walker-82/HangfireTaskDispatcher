using Hangfire.Extension.TaskDispatcher.Interfaces;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Hangfire.Extension.TaskDispatcher.Attributes;
using Hangfire.Extension.TaskDispatcher.Implementations;

namespace Tasks
{
    public enum Mytype
    {
        option1,
        option2
    }

    [DisplayName("Task Two")]
    [Description("This is a description")]
    public class T2TaskParameters : BaseTaskParameters
    {
        public override string Queue => "newqueue";
        public string Name => "MyName";
        [DisplayName("option Set")]
        [Description("select this if you want something to happen")]
        public bool? Option { get; set; }
        [TaskFormIgnore]
        public bool Option2 { get; set; }

        [Description("should have default value selected")]
        public bool Option3 { get; set; }
        public DateTime? Date { get; set; }
        public string SomethingElse { get; set; }
        public Mytype Myenum { get; set; }
        public long? LongNumber { get; set; }
        public Guid? GuidId { get; set; }
        public decimal? DecimalValue { get; set; }
        public double? DoubleValue { get; set; }
    }

    public class TestTaskParameters : BaseTaskParameters
    {
        public string MyName { get; set; }
    }

    [DisplayName("second task")]
    [Description("this is some details on the description")]
    public class Test1TaskParameters : BaseTaskParameters
    {
        public int MyNumber { get; set; }
    }

    public class MyGenericType<T> : BaseTaskParameters<T>
    {
        public string Somedata { get; set; }
    }


    public class Type1
    {
    }

    public class Type2
    {
    }


    public class MyGenericTypeHandler<T> : ITaskHandler<MyGenericType<T>>
    {
        public void Process(MyGenericType<T> taskParameters)
        {
            var name = typeof(T).Name;
            throw new NotImplementedException();
        }
    }

    public class Test1TaskHandler : ITaskHandler<Test1TaskParameters>
    {
        public void Process(Test1TaskParameters taskParameters)
        {
            throw new NotImplementedException();
        }
    }

    public class T2TaskHandler : ITaskHandler<T2TaskParameters>
    {
        public void Process(T2TaskParameters taskParameters)
        {
            throw new NotImplementedException();
        }
    }

    public class TestTaskHandler : ITaskHandler<TestTaskParameters>
    {
        public void Process(TestTaskParameters taskParameters)
        {
            throw new NotImplementedException();
        }
    }
}