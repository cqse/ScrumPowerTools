using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace ScrumPowerTools.Framework.Presentation
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public void NotifyOfPropertyChange(string propertyName)
        {
            Executor.ExecuteOnUIThread(() => PropertyChanged(this, new PropertyChangedEventArgs(propertyName)));
        }

        public void NotifyOfPropertyChange<TProperty>(Expression<Func<TProperty>> property)
        {
            var lambda = (LambdaExpression)property;
            MemberExpression memberExpression;

            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)lambda.Body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else
            {
                memberExpression = (MemberExpression)lambda.Body;
            }

            NotifyOfPropertyChange(memberExpression.Member.Name);
        }
    }
}