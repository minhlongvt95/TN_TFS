using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.MenuBuild;
using TN.TNM.DataAccess.Messages.Results.MenuBuild;
using TN.TNM.DataAccess.Models.MenuBuild;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class MenuBuildDAO : BaseDAO, IMenuBuildDataAccess
    {
        public MenuBuildDAO(Databases.TNTN8Context _context)
        {
            this.context = _context;
        }

        public GetMenuBuildResult GetMenuBuild(GetMenuBuildParameter parameter)
        {
            try
            {
                var ListMenuBuild = new List<MenuBuildEntityModel>();


                return new GetMenuBuildResult()
                {
                    Status = true,
                    Message = "Success",
                    ListMenuBuild = ListMenuBuild
                };
            }
            catch (Exception e)
            {
                return new GetMenuBuildResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetMenuModuleResult GetMenuModule(GetMenuModuleParameter parameter)
        {
            try
            {
                var ListMenuModule = new List<MenuBuildEntityModel>();
                ListMenuModule = context.MenuBuild.Where(x => x.Level == 0).Select(y => new MenuBuildEntityModel
                {
                    MenuBuildId = y.MenuBuildId,
                    ParentId = y.ParentId,
                    Code = y.Code,
                    CodeParent = y.CodeParent,
                    Name = y.Name,
                    Path = y.Path,
                    NameIcon = y.NameIcon,
                    Level = y.Level,
                    IndexOrder = y.IndexOrder
                }).OrderBy(z => z.IndexOrder).ToList();

                return new GetMenuModuleResult()
                {
                    Status = true,
                    Message = "Success",
                    ListMenuModule = ListMenuModule
                };
            }
            catch (Exception e)
            {
                return new GetMenuModuleResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public CreateMenuBuildResult CreateMenuBuild(CreateMenuBuildParameter parameter)
        {
            try
            {
                var menuBuild = new MenuBuild();
                menuBuild.MenuBuildId = Guid.NewGuid();
                menuBuild.ParentId = parameter.MenuBuild.ParentId;
                menuBuild.Name = parameter.MenuBuild.Name?.Trim();
                menuBuild.Code = parameter.MenuBuild.Code?.Trim();
                menuBuild.CodeParent = parameter.MenuBuild.CodeParent?.Trim();
                menuBuild.Level = parameter.MenuBuild.Level;
                menuBuild.Path = parameter.MenuBuild.Path?.Trim();
                menuBuild.NameIcon = parameter.MenuBuild.NameIcon?.Trim();
                menuBuild.IndexOrder = parameter.MenuBuild.IndexOrder;
                menuBuild.IsPageDetail = parameter.MenuBuild.IsPageDetail;

                context.MenuBuild.Add(menuBuild);

                #region Tạo thêm item mask cho sub menu module khi tạo menu module

                if (parameter.MenuBuild.Level == 0)
                {
                    var subMenuMask = new MenuBuild();
                    subMenuMask.MenuBuildId = Guid.NewGuid();
                    subMenuMask.ParentId = menuBuild.MenuBuildId;
                    subMenuMask.Name = menuBuild.Name;
                    subMenuMask.Code = menuBuild.Code + "_mask";
                    subMenuMask.CodeParent = menuBuild.Code;
                    subMenuMask.Level = 1;
                    subMenuMask.Path = null;
                    subMenuMask.NameIcon = null;
                    subMenuMask.IndexOrder = 1;

                    context.MenuBuild.Add(subMenuMask);
                }

                #endregion

                context.SaveChanges();

                return new CreateMenuBuildResult()
                {
                    Status = true,
                    Message = "Success",
                };
            }
            catch (Exception e)
            {
                return new CreateMenuBuildResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetSubMenuModuleByMenuModuleCodeResult GetSubMenuModuleByMenuModuleCode(
            GetSubMenuModuleByMenuModuleCodeParameter parameter)
        {
            try
            {
                var ListSubMenuModule = new List<MenuBuildEntityModel>();
                ListSubMenuModule = context.MenuBuild.Where(x => x.CodeParent == parameter.MenuModuleCode).Select(y =>
                    new MenuBuildEntityModel
                    {
                        MenuBuildId = y.MenuBuildId,
                        ParentId = y.ParentId,
                        Code = y.Code,
                        CodeParent = y.CodeParent,
                        Name = y.Name,
                        Path = y.Path,
                        NameIcon = y.NameIcon,
                        Level = y.Level,
                        IndexOrder = y.IndexOrder
                    }).OrderBy(z => z.IndexOrder).ToList();

                return new GetSubMenuModuleByMenuModuleCodeResult()
                {
                    Status = true,
                    Message = "Success",
                    ListSubMenuModule = ListSubMenuModule
                };
            }
            catch (Exception e)
            {
                return new GetSubMenuModuleByMenuModuleCodeResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public GetMenuPageBySubMenuCodeResult GetMenuPageBySubMenuCode(GetMenuPageBySubMenuCodeParameter parameter)
        {
            try
            {
                var ListMenuPage = new List<MenuBuildEntityModel>();
                ListMenuPage = context.MenuBuild.Where(x => x.CodeParent == parameter.SubMenuCode).Select(y =>
                    new MenuBuildEntityModel
                    {
                        MenuBuildId = y.MenuBuildId,
                        ParentId = y.ParentId,
                        Code = y.Code,
                        CodeParent = y.CodeParent,
                        Name = y.Name,
                        Path = y.Path,
                        NameIcon = y.NameIcon,
                        Level = y.Level,
                        IndexOrder = y.IndexOrder,
                        IsPageDetail = y.IsPageDetail
                    }).OrderBy(z => z.IndexOrder).ToList();

                return new GetMenuPageBySubMenuCodeResult()
                {
                    Status = true,
                    Message = "Success",
                    ListMenuPage = ListMenuPage
                };
            }
            catch (Exception e)
            {
                return new GetMenuPageBySubMenuCodeResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public UpdateIsPageDetailResult UpdateIsPageDetail(UpdateIsPageDetailParameter parameter)
        {
            try
            {
                var menuBuild = context.MenuBuild.FirstOrDefault(x => x.MenuBuildId == parameter.MenuBuildId);

                if (menuBuild == null)
                {
                    return new UpdateIsPageDetailResult()
                    {
                        Status = false,
                        Message = "Menu Build không tồn tại trên hệ thống"
                    };
                }

                menuBuild.IsPageDetail = parameter.IsPageDetail;
                context.MenuBuild.Update(menuBuild);
                context.SaveChanges();

                return new UpdateIsPageDetailResult()
                {
                    Status = true,
                    Message = "Success"
                };
            }
            catch (Exception e)
            {
                return new UpdateIsPageDetailResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }

        public UpdateMenuBuildResult UpdateMenuBuild(UpdateMenuBuildParameter parameter)
        {
            try
            {
                var menuBuild = new MenuBuild();

                menuBuild.MenuBuildId = parameter.MenuBuild.MenuBuildId;
                menuBuild.ParentId = parameter.MenuBuild.ParentId;
                menuBuild.Name = parameter.MenuBuild.Name?.Trim();
                menuBuild.Code = parameter.MenuBuild.Code?.Trim();
                menuBuild.CodeParent = parameter.MenuBuild.CodeParent?.Trim();
                menuBuild.Level = parameter.MenuBuild.Level;
                menuBuild.Path = parameter.MenuBuild.Path?.Trim();
                menuBuild.NameIcon = parameter.MenuBuild.NameIcon?.Trim();
                menuBuild.IndexOrder = parameter.MenuBuild.IndexOrder;
                menuBuild.IsPageDetail = parameter.MenuBuild.IsPageDetail;

                context.MenuBuild.Update(menuBuild);
                context.SaveChanges();

                return new UpdateMenuBuildResult()
                {
                    Status = true,
                    Message = "Success"
                };
            }
            catch (Exception e)
            {
                return new UpdateMenuBuildResult()
                {
                    Status = false,
                    Message = e.Message
                };
            }
        }
        
    }
}
