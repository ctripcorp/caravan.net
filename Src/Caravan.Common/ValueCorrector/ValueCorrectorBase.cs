using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.ValueCorrector
{
    public abstract class ValueCorrectorBase<T> : IValueCorrector<T>
    {
        private IValueCorrector<T> innerValueCorrector;

        public ValueCorrectorBase(IValueCorrector<T> innerValueCorrector)
        {
            this.innerValueCorrector = innerValueCorrector;
        }

        public T Correct(T value)
        {
            if (innerValueCorrector != null)
                value = innerValueCorrector.Correct(value);

            return CorrectValue(value);
        }

        protected abstract T CorrectValue(T value);
    }
}
