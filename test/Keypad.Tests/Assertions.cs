using System.Runtime.CompilerServices;
using TUnit.Assertions.AssertConditions;
using TUnit.Assertions.AssertConditions.Interfaces;
using TUnit.Assertions.AssertionBuilders;

namespace Keypad.Tests;

public static class Assertions
{
    public static InvokableValueAssertionBuilder<T> Satisfies<T>(
        this IValueSource<T> valueSource,
        Func<T, bool> predicate,
        [CallerArgumentExpression(nameof(predicate))] string doNotPopulateThisValue = "")
    {
        return valueSource.RegisterAssertion(
            assertCondition: new PredicateAssertionCondition<T>(doNotPopulateThisValue, predicate),
            argumentExpressions: [doNotPopulateThisValue]
        );
    }

    private sealed class PredicateAssertionCondition<T>(string expected, Func<T, bool> predicate) : ExpectedValueAssertCondition<T, string>(expected)
    {
        private readonly string expected = expected;

        protected override string GetExpectation() => $"to satisfy '{expected}'";

        protected override ValueTask<AssertionResult> GetResult(T? actualValue, string? expectedValue)
        {
            return actualValue == null
                ? AssertionResult.Fail("it was null")
                : AssertionResult.FailIf(!predicate(actualValue), "it did not");
        }
    }
}
