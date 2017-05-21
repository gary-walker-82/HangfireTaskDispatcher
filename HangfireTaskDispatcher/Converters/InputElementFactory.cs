using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hangfire.Extension.TaskDispatcher.Interfaces;

namespace Hangfire.Extension.TaskDispatcher.Converters
{
    public class InputElementFactory
    {
        private readonly Dictionary<Type, IInputElement> _inputElements;
        public InputElementFactory()
        {
            _inputElements = Assembly.GetAssembly(typeof(InputElementFactory)).GetTypes()
                .Where(x => typeof(IInputElement).IsAssignableFrom(x) && x.IsClass && x.IsAbstract == false)
                .ToDictionary(x => x.GetInterfaces().FirstOrDefault(y => y.IsGenericType).GenericTypeArguments.First(),
                    x => Activator.CreateInstance(x) as IInputElement);

        }

        public IInputElement GetInputElementWriter(PropertyInfo propertyInfo)
        {
            var type = propertyInfo.PropertyType;
            if (!_inputElements.ContainsKey(type)) throw new Exception("no matching type");
            var input = _inputElements[type];
            input.Property = propertyInfo;
            return input;
        }

    }
}