using FluentValidation;

namespace StarKindred.API.Extensions;

public static class RuleBuilderInitialExtensions
{
    public static IRuleBuilderOptions<T, int> PageNumber<T>(this IRuleBuilder<T, int> ruleBuilder)
    {
        return ruleBuilder.GreaterThan(0).WithMessage("Page numbers start at 1.");
    }
}