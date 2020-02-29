using Sitecore.ExperienceForms.Models;
using Sitecore.ExperienceForms.Mvc.Models.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GOTO_Usergroup.Feature.Account.Helper
{
    public static class FieldHelper
    {
        public static IViewModel GetFieldById(Guid id, IList<IViewModel> fields)
        {
            return fields.FirstOrDefault(f => Guid.Parse(f.ItemId) == id);
        }

        public static IViewModel GetFieldValueByName(string fieldName, IList<IViewModel> fields)
        {            
            return (fields.FirstOrDefault(f => f.Name == fieldName));
        }

        public static string GetValue(object field)
        {
            return field?.GetType().GetProperty("Value")?.GetValue(field, null)?.ToString() ?? string.Empty;
        }
    }
}