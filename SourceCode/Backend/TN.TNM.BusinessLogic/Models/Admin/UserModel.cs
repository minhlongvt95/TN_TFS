using System;
using TN.TNM.Common;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.User;

namespace TN.TNM.BusinessLogic.Models.Admin
{
    public class UserModel : BaseModel<User>
    {
        public Guid? UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Guid? EmployeeId { get; set; }
        public bool? Active { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public UserModel() { }
        public UserModel(User entity) : base(entity)
        {
            //Xu ly sau khi lay tu DB len
        }

        public UserModel(UserEntityModel entity)
        {
            Mapper(entity, this);
        }

        public override User ToEntity()
        {
            var entity = new User();
            if (!string.IsNullOrEmpty(this.Password) && this.Password.Length < 15)
            {
                this.Password = AuthUtil.GetHashingPassword(this.Password);
            }
            Mapper(this, entity);
            return entity;
        }
    }
}
