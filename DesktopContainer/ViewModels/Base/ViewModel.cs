using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Xaml;
using DesktopContainer.Annotations;

namespace DesktopContainer.ViewModels.Base
{
    public abstract class ViewModel : MarkupExtension, INotifyPropertyChanged, IDisposable
    {
        private bool _disposed;

        private WeakReference _targetRef;
        private WeakReference _rootRef;

        public object TargetObject => _targetRef.Target;
        public object RootObject => _rootRef.Target;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
            var handlers = PropertyChanged;
            if (handlers is null) return;

            var invocationList = handlers.GetInvocationList();

            var arg = new PropertyChangedEventArgs(propertyName);
            for (var i = 0; i < invocationList.Length; i++)
            {
                var action = invocationList[i];
                if (action.Target is DispatcherObject dispObject)
                    dispObject.Dispatcher.Invoke(action, this, arg);
                else
                    action.DynamicInvoke(this, arg);
            }
        }

        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public override object ProvideValue(IServiceProvider sp)
        {
            var valueTargetService = sp.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            var rootObjectService = sp.GetService(typeof(IRootObjectProvider)) as IRootObjectProvider;

            OnInitialized(
                valueTargetService?.TargetObject,
                valueTargetService?.TargetProperty,
                rootObjectService?.RootObject);

            return this;
        }

        protected virtual void OnInitialized(object target, object property, object root)
        {
            _targetRef = new WeakReference(target);
            _rootRef = new WeakReference(root);
        }

        //~ViewModel()
        //{
        //    Dispose(false);
        //}

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _disposed) return;
            _disposed = true;
            // Освобождение управляемых ресурсов
        }
    }
}