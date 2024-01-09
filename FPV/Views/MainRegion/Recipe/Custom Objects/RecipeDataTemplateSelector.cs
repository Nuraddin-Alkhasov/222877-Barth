using HMI.Components;
using System.Windows;
using System.Windows.Controls;

namespace HMI.Views.MainRegion.Recipe
{
    public class RecipeDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate AlphaTemplate { get; set; }
        public DataTemplate BoolTemplate { get; set; }
        public DataTemplate NumTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var editType = item as RecipeVariableInfo;
            if (editType != null)
            {
                switch (editType.Type.ToString())
                {
                    case "String":
                        return this.AlphaTemplate;

                    case "Boolean":
                        return this.BoolTemplate;

                    default:
                        return this.NumTemplate;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}
