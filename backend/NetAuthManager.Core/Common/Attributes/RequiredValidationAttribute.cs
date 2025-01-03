using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Core.Common.Attributes;

/// <summary>
/// 必填验证属性
/// </summary>
public class RequiredValidationAttribute : RequiredAttribute
{
    /// <summary>
    /// 字段名
    /// </summary>
    public string DisplayName { get; set; }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (
            (value is string && string.IsNullOrEmpty(value as string)) ||
            value == null
        )
        {
            return new ValidationResult($"需要【{DisplayName}】参数 EntityCode");
        }

        return ValidationResult.Success;
    }
}
