using System;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using Prism.Mvvm;

namespace Mutey.ViewModels
{
    internal class AppViewModel : BindableBase
    {
        private ImageSource? iconImage;

        public AppViewModel()
        {
            SetImage();
        }

        public ImageSource? IconImage
        {
            get => iconImage;
            internal set => SetProperty(ref iconImage, value);
        }

        private void SetImage()
        {
            return;
        }
    }

    internal class ConferenceSoftwareViewModel : BindableBase { }
}