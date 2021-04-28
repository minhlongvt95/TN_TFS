using System.Collections.Generic;
using System.Linq;

namespace TN.TNM.BusinessLogic.Models
{
    abstract public class BaseModel<T>
    {
       
        public BaseModel() { }
        public BaseModel(T entity)
        {
            if (entity != null) Mapper(entity, this);
        }

       
        abstract public T ToEntity();

        protected void Mapper<T1, T2>(T1 fromObj, T2 toObj)
        {
            var _toProperties = toObj.GetType().GetProperties().ToList();
            var _fromProperties = fromObj.GetType().GetProperties().ToList();
            _toProperties.ForEach(_03P =>
            {
                var _01P = _fromProperties.FirstOrDefault(p => p.Name == _03P.Name);
                if (_01P != null)
                {
                    if (_01P.PropertyType.ToString().Contains("System.Collections.Generic.List"))
                    {
                    }
                    else
                    {
                        _03P.SetValue(toObj, _01P.GetValue(fromObj));

                    }
                }
            });
        }

        private void Convert<T1, T2>(List<T1> from, List<T2> to)
        {

        }
    }
}
