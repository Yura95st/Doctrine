namespace Doctrine.Domain.Tests.TestHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class ExpressionHelper
    {
        /// <summary>Checks whether expression arrays are equal.</summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="expressionArrayOne">The expression array one.</param>
        /// <param name="expressionArrayTwo">The expression array two.</param>
        /// <returns>True if expression arrays are equal, otherwise - false.</returns>
        public static bool AreExpressionArraysEqual<TSource, TProperty>(
        Expression<Func<TSource, TProperty>>[] expressionArrayOne,
        params Expression<Func<TSource, TProperty>>[] expressionArrayTwo)
        {
            if (expressionArrayOne == null || expressionArrayTwo == null)
            {
                return expressionArrayOne == null && expressionArrayTwo == null;
            }

            if (expressionArrayOne.Count() != expressionArrayTwo.Count())
            {
                return false;
            }

            IEnumerable<PropertyInfo> selectorOneProperties =
            expressionArrayOne.Select(expression => ExpressionHelper.GetPropertyInfo(expression));

            IEnumerable<PropertyInfo> selectorTwoProperties =
            expressionArrayTwo.Select(expression => ExpressionHelper.GetPropertyInfo(expression));

            return new HashSet<PropertyInfo>(selectorOneProperties).SetEquals(selectorTwoProperties);
        }

        private static PropertyInfo GetPropertyInfo<TSource, TProperty>(Expression<Func<TSource, TProperty>> propertyLambda)
        {
            Type type = typeof(TSource);

            Expression body = propertyLambda.Body;

            MemberExpression member;

            if (body.NodeType == ExpressionType.Convert)
            {
                member = ((UnaryExpression)body).Operand as MemberExpression;
            }
            else
            {
                member = body as MemberExpression;
            }

            if (member == null)
            {
                throw new ArgumentException(string.Format("Expression '{0}' refers to a method, not a property.",
                propertyLambda));
            }

            PropertyInfo propertyInfo = member.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException(string.Format("Expression '{0}' refers to a field, not a property.",
                propertyLambda));
            }

            if (!propertyInfo.ReflectedType.IsAssignableFrom(type))
            {
                throw new ArgumentException(string.Format(
                "Expression '{0}' refers to a property that is not from type {1}.", propertyLambda, type));
            }

            return propertyInfo;
        }
    }
}