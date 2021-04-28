using System;

namespace TN.TNM.BusinessLogic.Messages.Requests
{
    abstract public class BaseRequest<T>
    {
        public Guid UserId { get; set; }
        abstract public T ToParameter();
    }
}