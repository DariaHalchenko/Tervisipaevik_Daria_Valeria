using Microsoft.Maui.Controls;
using Tervisipaevik_Daria_Valeria.View;

namespace Tervisipaevik_Daria_Valeria
{
    public partial class Flyout_Page : FlyoutPage
    {
        public Flyout_Page()
        {
            InitializeComponent();
        }

        private void btnStartPage_Clicked(object sender, EventArgs e)
        {
            Detail = new NavigationPage(new StartPage1());
            if (!((IFlyoutPageController)this).ShouldShowSplitMode)
                IsPresented = false;
        }

        private void btnOhtusookPage_Clicked(object sender, EventArgs e)
        {
            Detail = new NavigationPage(new OhtusookPage());
            if (!((IFlyoutPageController)this).ShouldShowSplitMode)
                IsPresented = false;
        }

        private void btnVeejalgiminePage_Clicked(object sender, EventArgs e)
        {
            Detail = new NavigationPage(new VeejalgiminePage());
            if (!((IFlyoutPageController)this).ShouldShowSplitMode)
                IsPresented = false;
        }

        private void btnTreeningudPage_Clicked(object sender, EventArgs e)
        {
            Detail = new NavigationPage(new TreeningudPage());
            if (!((IFlyoutPageController)this).ShouldShowSplitMode)
                IsPresented = false;
        }
        private void btnTreeningudFotoPage_Clicked(object sender, EventArgs e)
        {
            Detail = new NavigationPage(new TreeningudFotoPage());
            if (!((IFlyoutPageController)this).ShouldShowSplitMode)
                IsPresented = false;
        }
    }
}