using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Configuration
{
    public interface IProperty<T> : IProperty
    {
        new T Value { get; }

        new PropertyConfig<T> Config { get; }

        new event EventHandler<PropertyChangedEventArgs<T>> OnChange;
    }
}
