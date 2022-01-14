using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployees.API.ModelBinders
{
    public class ArrayModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (!bindingContext.ModelMetadata.IsEnumerableType)
            {
                bindingContext.Result = ModelBindingResult.Failed();

                return Task.CompletedTask;
            }

            var providedValue = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).ToString();

            if (string.IsNullOrEmpty(providedValue))
            {
                bindingContext.Result = ModelBindingResult.Success(null);

                return Task.CompletedTask;
            }

            // using System.Reflection;
            // genericType = GUID
            var genericType = bindingContext.ModelType.GetTypeInfo().GenericTypeArguments[0];

            // using System.ComponentModel;
            // converter to GUID
            var converter = TypeDescriptor.GetConverter(genericType);

            // object array of GUIDs
            var objectArray = providedValue.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(e => converter.ConvertFromString(e.Trim())).ToArray();

            // GUID array
            var guidArray = Array.CreateInstance(genericType, objectArray.Length);

            objectArray.CopyTo(guidArray, 0);

            bindingContext.Model = guidArray;

            bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);

            return Task.CompletedTask;
        }
    }
}
