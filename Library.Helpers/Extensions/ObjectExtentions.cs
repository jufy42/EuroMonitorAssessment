using System;
using System.Reflection;

namespace Library.Helpers
{
    public static class ObjectExtentions
    {
        public static T AutoMap<T>(this T itemToUpdate, object viewModel)
        {
            foreach (PropertyInfo property1 in itemToUpdate.GetType().GetProperties())
            {
                object obj1;
                if (viewModel == null)
                {
                    obj1 = null;
                }
                else
                {
                    PropertyInfo property2 = viewModel.GetType().GetProperty(property1.Name);
                    obj1 = (object) property2 != null ? property2.GetValue(viewModel) : null;
                }

                object obj2 = obj1;
                PropertyInfo property3 = itemToUpdate.GetType().GetProperty(property1.Name);
                bool flag1 = Nullable.GetUnderlyingType(((object) property3 != null ? property3.PropertyType : null)!) != null;
                if (obj2 != null || flag1)
                {
                    PropertyInfo property2 = itemToUpdate.GetType().GetProperty(property1.Name);
                    object obj3 = (object) property2 != null ? property2.GetValue(itemToUpdate) : null;
                    PropertyInfo property4 = itemToUpdate.GetType().GetProperty(property1.Name);
                    Type type1 = (object) property4 != null ? property4.PropertyType : null;
                    Type type2;
                    if (viewModel == null)
                    {
                        type2 = null;
                    }
                    else
                    {
                        PropertyInfo property5 = viewModel.GetType().GetProperty(property1.Name);
                        type2 = (object) property5 != null ? property5.PropertyType : null;
                    }

                    bool flag2 = type1 == type2;
                    if (obj3 != obj2 & flag2)
                    {
                        PropertyInfo property5 = viewModel.GetType().GetProperty(property1.Name);
                        if (((object) property5 != null ? property5.PropertyType.Name : null) == "Guid")
                        {
                            PropertyInfo property6 = viewModel.GetType().GetProperty(property1.Name);
                            if ((Guid) (((object) property6 != null ? property6.GetValue(viewModel) : null) ?? Guid.Empty) != Guid.Empty)
                            {
                                PropertyInfo property7 = itemToUpdate.GetType().GetProperty(property1.Name);
                                if ((object) property7 != null)
                                    property7.SetValue(itemToUpdate, obj2);
                            }
                        }
                        else
                        {
                            PropertyInfo property6 = itemToUpdate.GetType().GetProperty(property1.Name);
                            if ((object) property6 != null)
                                property6.SetValue(itemToUpdate, obj2);
                        }
                    }
                }
            }

            return itemToUpdate;
        }
    }
}
