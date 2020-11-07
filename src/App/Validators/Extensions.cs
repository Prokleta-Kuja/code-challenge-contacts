using FluentValidation;

namespace PublicContacts.App.Validators
{
    public static class ValidatorExtensions
    {
        public static IRuleBuilderOptions<T, string?> PhoneNumber<T>(this IRuleBuilder<T, string?> ruleBuilder)
            => ruleBuilder.SetValidator(new PhoneNumberValidator());
    }
}