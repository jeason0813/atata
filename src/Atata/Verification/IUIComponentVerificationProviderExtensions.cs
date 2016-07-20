﻿using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atata
{
    public static class IUIComponentVerificationProviderExtensions
    {
        public static TOwner Exist<TControl, TOwner>(this IUIComponentVerificationProvider<TControl, TOwner> should)
            where TControl : IUIComponent<TOwner>
            where TOwner : PageObject<TOwner>
        {
            should.CheckNotNull(nameof(should));

            ATContext.Current.Log.StartVerificationSection($"{should.Component.ComponentFullName} {should.GetShouldText()} exist");

            SearchOptions searchOptions = new SearchOptions
            {
                IsSafely = false,
                Timeout = should.Timeout ?? RetrySettings.Timeout,
                RetryInterval = should.RetryInterval ?? RetrySettings.RetryInterval
            };

            if (should.IsNegation)
                should.Component.Missing(searchOptions);
            else
                should.Component.Exists(searchOptions);

            ATContext.Current.Log.EndSection();

            return should.Owner;
        }

        public static TOwner BeEnabled<TControl, TOwner>(this IUIComponentVerificationProvider<TControl, TOwner> should)
            where TControl : Control<TOwner>
            where TOwner : PageObject<TOwner>
        {
            should.CheckNotNull(nameof(should));

            var dataShould = should.Component.IsEnabled.Should;
            return should.IsNegation ? dataShould.Not.BeTrue() : dataShould.BeTrue();
        }

        public static TOwner BeDisabled<TControl, TOwner>(this IUIComponentVerificationProvider<TControl, TOwner> should)
            where TControl : Control<TOwner>
            where TOwner : PageObject<TOwner>
        {
            should.CheckNotNull(nameof(should));

            var dataShould = should.Component.IsEnabled.Should;
            return should.IsNegation ? dataShould.Not.BeFalse() : dataShould.BeFalse();
        }

        public static TOwner BeReadOnly<TData, TControl, TOwner>(this IFieldVerificationProvider<TData, TControl, TOwner> should)
            where TControl : EditableField<TData, TOwner>
            where TOwner : PageObject<TOwner>
        {
            should.CheckNotNull(nameof(should));

            var dataShould = should.Component.IsReadOnly.Should;
            return should.IsNegation ? dataShould.Not.BeTrue() : dataShould.BeTrue();
        }

        public static TOwner BeChecked<TControl, TOwner>(this IUIComponentVerificationProvider<TControl, TOwner> should)
            where TControl : Field<bool, TOwner>, ICheckable<TOwner>
            where TOwner : PageObject<TOwner>
        {
            should.CheckNotNull(nameof(should));

            var dataShould = should.Component.Should;
            return should.IsNegation ? dataShould.Not.BeTrue() : dataShould.BeTrue();
        }

        public static TOwner BeUnchecked<TControl, TOwner>(this IUIComponentVerificationProvider<TControl, TOwner> should)
            where TControl : Field<bool, TOwner>, ICheckable<TOwner>
            where TOwner : PageObject<TOwner>
        {
            should.CheckNotNull(nameof(should));

            var dataShould = should.Component.Should;
            return should.IsNegation ? dataShould.Not.BeFalse() : dataShould.BeFalse();
        }

        public static TOwner HaveChecked<TData, TControl, TOwner>(this IFieldVerificationProvider<TData, TControl, TOwner> should, TData value)
            where TControl : CheckBoxList<TData, TOwner>
            where TOwner : PageObject<TOwner>
        {
            should.CheckNotNull(nameof(should));

            IEnumerable<TData> expectedIndividualValues = should.Component.GetIndividualValues(value);
            string expectedIndividualValuesAsString = should.Component.ConvertIndividualValuesToString(expectedIndividualValues, true);

            string expectedMessage = new StringBuilder().
                Append("have checked").
                AppendIf(expectedIndividualValues.Count() > 1, ":").
                Append($" {expectedIndividualValuesAsString}").ToString();

            ATContext.Current.Log.StartVerificationSection(
                $"{should.Component.ComponentFullName} {should.GetShouldText()} {expectedMessage}");

            IEnumerable<TData> actualIndividualValues = null;

            bool doesSatisfy = ATContext.Current.Driver.Try().Until(_ =>
            {
                actualIndividualValues = should.Component.GetIndividualValues(should.Component.Get());
                int intersectionsCount = expectedIndividualValues.Intersect(actualIndividualValues).Count();
                return should.IsNegation ? intersectionsCount == 0 : intersectionsCount == expectedIndividualValues.Count();
            }, should.Timeout, should.RetryInterval);

            if (!doesSatisfy)
                throw should.CreateAssertionException(
                    expectedMessage,
                    should.Component.ConvertIndividualValuesToString(actualIndividualValues, true));

            ATContext.Current.Log.EndSection();

            return should.Owner;
        }
    }
}