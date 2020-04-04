using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace ContestPark.Mobile.ViewModels.Base
{
    public abstract class ExtendedBindableObject : MvvmHelpers.BaseViewModel
    {
        public void RaisePropertyChanged<T>(Expression<Func<T>> property)
        {
            try
            {
                var name = GetMemberInfo(property).Name;
                OnPropertyChanged(name);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private MemberInfo GetMemberInfo(Expression expression)
        {
            MemberExpression operand;
            LambdaExpression lambdaExpression = (LambdaExpression)expression;
            if (lambdaExpression.Body as UnaryExpression != null)
            {
                UnaryExpression body = (UnaryExpression)lambdaExpression.Body;
                operand = (MemberExpression)body.Operand;
            }
            else
            {
                operand = (MemberExpression)lambdaExpression.Body;
            }
            return operand.Member;
        }
    }
}
